using MvvmHelpers;
using FrontEnd.Model;
using FrontEnd.Events;
using Backend.Source;
using Backend.Controller;
using Backend.Database;
using Backend.Model;
using Backend.Exceptions;
using FrontEnd.Controller;
using Backend.Enums;
using System.Windows;

namespace FrontEnd.Source
{
    /// <summary>
    /// Represents a collection of objects implementing <see cref="IAbstractModel"/> and extends <see cref="ObservableRangeCollection{M}"/>.
    /// Implements <see cref="IRecordSource{M}"/>, <see cref="IChildSource"/>, and <see cref="IUISource"/> interfaces.
    /// </summary>
    /// <typeparam name="M">The type of objects that implement <see cref="IAbstractModel"/> and have a parameterless constructor.</typeparam>
    public class RecordSource<M> : ObservableRangeCollection<M>, IRecordSource<M>, IChildSource, IUISource where M : IAbstractModel, new()
    {
        private Navigator<M>? _navigator;

        /// <summary>
        /// A list of <see cref="IUIControl"/> objects associated with this data source.
        /// </summary>        
        private List<IUIControl>? _uiControls;

        public event FilterEventHandler? RunFilter;
        public IParentSource? ParentSource { get; set; }
        public IAbstractSQLModelController? Controller { get; set; }

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RecordSource{M}"/> class.
        /// </summary>
        public RecordSource() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordSource{M}"/> class filled with the provided source.
        /// </summary>
        /// <param name="source">An enumerable collection of objects implementing <see cref="ISQLModel"/>.</param>
        public RecordSource(IEnumerable<M> source) : base(source) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordSource{M}"/> class filled with the <see cref="MasterSource"/> from the specified <paramref name="database"/>.
        /// This constructor treats this <see cref="IRecordSource{M}"/> as a child of the <see cref="MasterSource"/>.
        /// </summary>
        /// <param name="database">An instance of <see cref="IAbstractDatabase"/>.</param>
        public RecordSource(IAbstractDatabase database) : this(database.MasterSource.Cast<M>())
        {
            database.MasterSource.AddChild(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordSource{M}"/> class filled with the <see cref="MasterSource"/> from the specified <paramref name="database"/>.
        /// This constructor treats this <see cref="IRecordSource{M}"/> as a child of the <see cref="MasterSource"/> and sets the <see cref="Controller"/> property.
        /// </summary>
        /// <param name="database">An instance of <see cref="IAbstractDatabase"/>.</param>
        /// <param name="controller">An instance of <see cref="IAbstractSQLModelController"/>.</param>
        public RecordSource(IAbstractDatabase database, IAbstractFormController controller) : this(database)
        {
            Controller = controller;
        }
        #endregion

        #region Enumerator
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        /// <remarks>
        /// This method uses the <see cref="_navigator"/> as enumerator.
        /// </remarks>
        public new IEnumerator<M> GetEnumerator()
        {
            if (_navigator != null)
            {
                _navigator = new Navigator<M>(this, _navigator.Index, _navigator.AllowNewRecord);
                return _navigator;
            }
            _navigator = new Navigator<M>(this);
            return _navigator;
        }

        public INavigator<M> Navigate() => (INavigator<M>)GetEnumerator();
        #endregion

        #region IChildSource
        public virtual void Update(CRUD crud, ISQLModel model)
        {
            int removedIndex = -1;
            switch (crud)
            {
                case CRUD.INSERT:
                    Add((M)model);
                    Controller?.GoLast();
                    break;
                case CRUD.UPDATE:
                    NotifyUIControl([]);
                    break;
                case CRUD.DELETE:
                    removedIndex = IndexOf((M)model);
                    if (!Remove((M)model)) break;
                    if (_navigator != null)
                    {
                        if (_navigator.BOF && !_navigator.NoRecords) Controller?.GoFirst();
                        if (_navigator.EOF && !_navigator.NoRecords) Controller?.GoPrevious();
                        if (Count == 0) Controller?.GoAt(null);
                        else Controller?.GoAt(removedIndex);
                    }
                    break;
            }

            try 
            {
                RunFilter?.Invoke(this, new());
            }
            catch { }

        }
        #endregion

        #region IUISource
        public void NotifyUIControl(object[] args)
        {
            if (_uiControls != null && _uiControls.Count > 0)
                foreach (IUIControl combo in _uiControls) combo.OnItemSourceUpdated(args);
        }

        public void AddUIControlReference(IUIControl control)
        {
            if (_uiControls == null) _uiControls = [];
            _uiControls.Add(control);
        }
        #endregion

        /// <summary>
        /// Creates a <see cref="RecordSource{M}"/> object asynchronously from an <see cref="IAsyncEnumerable{M}"/>.
        /// </summary>
        /// <param name="source">An <see cref="IAsyncEnumerable{M}"/> source.</param>
        /// <returns>A task representing the asynchronous operation that returns a <see cref="RecordSource{M}"/>.</returns>
        public static async Task<RecordSource<M>> CreateFromAsyncList(IAsyncEnumerable<M> source) =>
        new RecordSource<M>(await source.ToListAsync());

        #region IDataSource
        public virtual string RecordPositionDisplayer()
        {
            if (_navigator == null) throw new NoNavigatorException();
            return true switch
            {
                true when _navigator.NoRecords => "NO RECORDS",
                true when _navigator.IsNewRecord => "New Record",
                _ => $"Record {_navigator?.RecNum} of {_navigator?.RecordCount}",
            };
        }
        #endregion

        public void ReplaceRecords(IEnumerable<M> newSource)
        {
            M? current = default(M?);
            try
            {
                if (_navigator!=null)
                    current = _navigator.CurrentRecord;
            }
            catch { }

            Clear();

            ReplaceRange(newSource);

            if (current != null && Controller != null)
            {
                Controller.SetCurrentRecord(this.FirstOrDefault(s => s.Equals(current)));
                if (Controller.GetCurrentRecord()== null)
                    Controller.GoFirst();
                return;
            }

            Controller?.GoFirst();
        }

        #region Disposers
        /// <summary>
        /// Disposes resources used by the record source, optionally disposing the associated controller.
        /// </summary>
        /// <param name="disposeController">True to dispose the associated controller; otherwise, false.</param>
        public void Dispose(bool disposeController)
        {
            if (disposeController)
                Controller?.Dispose();
            Dispose();
        }

        /// <summary>
        /// Disposes resources used by the record source.
        /// </summary>
        public void Dispose()
        {
            ParentSource?.RemoveChild(this);
            _uiControls?.Clear();
            RunFilter = null;
            try
            {
                Clear();
                _navigator?.Dispose();
            }
            catch
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    Clear();
                    _navigator?.Dispose();
                });
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }
        #endregion

        public override string? ToString() => $"RecordSource<{typeof(M).Name}> - Count: {Count}";

    }
}