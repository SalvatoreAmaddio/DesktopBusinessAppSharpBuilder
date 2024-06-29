using Backend.Model;
using FrontEnd.Dialogs;
using FrontEnd.Events;
using System.ComponentModel;
using System.Data.Common;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FrontEnd.Model
{
    /// <summary>
    /// Extends <see cref="AbstractSQLModel"/> and provides additional UI functionalities.
    /// Implements <see cref="IAbstractModel"/>.
    /// </summary>
    /// <typeparam name="M">The type of model that extends <see cref="ISQLModel"/> and has a parameterless constructor.</typeparam>
    public abstract class AbstractModel<M> : AbstractSQLModel, IAbstractModel where M : ISQLModel, new()
    {
        bool _isDirty = false;

        #region Properties
        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                _isDirty = value;
                RaisePropertyChanged(nameof(IsDirty));
                if (!value) OnDirtyChanged?.Invoke(this, new(this));
            }
        }

        /// <summary>
        /// Gets a list of all properties marked with <see cref="AbstractField"/> attribute.
        /// </summary>
        protected List<SimpleTableField> AllFields { get; }
        #endregion

        #region Events
        public event OnDirtyChangedEventHandler? OnDirtyChanged;
        public event PropertyChangedEventHandler? PropertyChanged;
        public event AfterUpdateEventHandler? AfterUpdate;
        public event BeforeUpdateEventHandler? BeforeUpdate;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractModel{M}"/> class.
        /// </summary>
        public AbstractModel() : base() => AllFields = new(GetAllTableFields());

        /// <summary>
        /// Sets the <see cref="IsDirty"/> property to true.
        /// </summary>
        public void Dirt() => IsDirty = true;

        /// <summary>
        /// Sets the <see cref="IsDirty"/> property to false.
        /// </summary>
        public void Clean() => IsDirty = false;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propName">The name of the property that changed.</param>
        public void RaisePropertyChanged(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        /// <summary>
        /// Updates the specified property and raises <see cref="BeforeUpdate"/> and <see cref="AfterUpdate"/> events.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="value">The new value of the property.</param>
        /// <param name="backProp">The old value of the property.</param>
        /// <param name="propName">The name of the property being updated.</param>
        public void UpdateProperty<T>(ref T value, ref T backProp, [CallerMemberName] string propName = "")
        {
            SimpleTableField? field = AllFields.Find(s => s.Name.Equals(propName));
            field?.SetValue(backProp);
            field?.Change(true);
            BeforeUpdateArgs args = new(value, backProp, propName);
            BeforeUpdate?.Invoke(this, args);
            if (args.Cancel) return;
            backProp = value;
            IsDirty = true;
            AfterUpdate?.Invoke(this, args);
            RaisePropertyChanged(propName);
        }

        /// <summary>
        /// Sets all changed properties back to their previous values and resets <see cref="IsDirty"/> to false.
        /// </summary>
        public void Undo()
        {
            foreach (var field in AllFields.Where(s => s.Changed))
            {
                field.Property.SetValue(this, field.GetValue());
                field.Change(false);
            }
            IsDirty = false;
        }

        /// <summary>
        /// Validates whether an update operation is allowed.
        /// </summary>
        /// <returns>True if update is allowed; otherwise, false.</returns>
        public override bool AllowUpdate()
        {
            bool result = base.AllowUpdate();
            if (!result)
                Failure.Allert($"Please fill all mandatory fields:\n{GetEmptyMandatoryFields()}");
            return result;
        }

        /// <summary>
        /// Retrieves all properties marked with the <see cref="AbstractField"/> attribute in the current instance's type.
        /// </summary>
        /// <returns>An enumerable collection of <see cref="SimpleTableField"/> instances representing the marked properties.</returns>
        private IEnumerable<SimpleTableField> GetAllTableFields()
        {
            Type type = GetType();
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                AbstractField? field = prop.GetCustomAttribute<AbstractField>();
                if (field != null)
                {
                    yield return new SimpleTableField(prop.Name, null, prop);
                }
            }
        }

        /// <summary>
        /// Reads and creates an instance of <see cref="ISQLModel"/> from a database record.
        /// </summary>
        /// <param name="reader">The database reader containing the record data.</param>
        /// <returns>An instance of <see cref="ISQLModel"/> created from the database record.</returns>
        /// <remarks>
        /// This method is called through reflection in <see cref="CreateFromDbRecord(DbDataReader)"/>
        /// </remarks>
        public override ISQLModel Read(DbDataReader reader) => CreateFromDbRecord(reader);

        /// <summary>
        /// Creates an instance of <typeparamref name="M"/> from a database record.
        /// </summary>
        /// <param name="reader">The database reader containing the record data.</param>
        /// <returns>An instance of <typeparamref name="M"/> created from the database record.</returns>
        public M CreateFromDbRecord(DbDataReader reader)
        {
            Type myClassType = typeof(M);
            ConstructorInfo? constructor = myClassType.GetConstructor(new[] { typeof(DbDataReader) }) ?? throw new NullReferenceException($"Class {myClassType.Name} is missing a constructor that takes a DbDataReader object as a parameter!");
            object myClassInstance = constructor.Invoke(new object[] { reader });
            return (M)myClassInstance;
        }

        /// <summary>
        /// Disposes the object and clears event subscriptions.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            PropertyChanged = null;
            AfterUpdate = null;
            BeforeUpdate = null;
            GC.SuppressFinalize(this);
        }
    }
}