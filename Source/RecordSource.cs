using MvvmHelpers;
using FrontEnd.Model;
using FrontEnd.Events;
using Backend.Source;
using Backend.Controller;
using Backend.Database;
using Backend.Model;
using Backend.Exceptions;
using FrontEnd.Controller;

namespace FrontEnd.Source
{
    public class RecordSource<M> : ObservableRangeCollection<M>, IRecordSource<M>, IChildSource, IUISource, IDisposable where M : AbstractModel, new()
    {
        /// <summary>
        /// This delegate works as a bridge between the <see cref="Controller.IAbstractSQLModelController"/> and this <see cref="Backend.Source.RecordSource"/>.
        /// If any filter operations has been implemented in the Controller, The RecordSource can trigger them.
        /// </summary>
        public event FilterEventHandler? RunFilter;
        private INavigator? navigator;
        public IParentSource? ParentSource { get; set; }
        public IAbstractSQLModelController? Controller { get; set; }

        /// <summary>
        /// A list of <see cref="IUIControl"/> associated to this <see cref="RecordSource"/>.
        /// </summary>
        private List<IUIControl>? UIControls;

        #region Constructor
        /// <summary>
        /// Parameterless Constructor to instantiate a RecordSource object.
        /// </summary>
        public RecordSource() { }

        /// <summary>
        /// It instantiates a RecordSource object filled with the given IEnumerable&lt;<see cref="ISQLModel"/>&gt;.
        /// </summary>
        /// <param name="source">An IEnumerable&lt;<see cref="ISQLModel"/>&gt;</param>
        public RecordSource(IEnumerable<M> source) : base(source) { }

        /// <summary>
        /// It instantiates a RecordSource object filled with the given <see cref="IAbstractDatabase.Records"/> IEnumerable.
        /// This constructor will consider this RecordSource object as a child of the <see cref="IAbstractDatabase.Records"/>
        /// </summary>
        /// <param name="db">An instance of <see cref="IAbstractDatabase"/></param>
        public RecordSource(IAbstractDatabase db) : this(db.Records.Cast<M>()) => db.Records.AddChild(this);

        /// <summary>
        /// It instantiates a RecordSource object filled with the given <see cref="IAbstractDatabase.Records"/> IEnumerable.
        /// This constructor will consider this RecordSource object as a child of the <see cref="IAbstractDatabase.Records"/>
        /// </summary>
        /// <param name="db">An instance of <see cref="IAbstractDatabase"/></param>
        /// <param name="controller">An instance of <see cref="IAbstractSQLModelController"/></param>
        public RecordSource(IAbstractDatabase db, IAbstractFormController controller) : this(db) => Controller = controller;
        #endregion

        #region Enumerator
        /// <summary>
        /// Override the default <c>GetEnumerator()</c> method to replace it with a <see cref="ISourceNavigator"></see> object./>
        /// </summary>
        /// <returns>An Enumerator object.</returns>
        public new IEnumerator<ISQLModel> GetEnumerator()
        {
            if (navigator != null)
            {
                navigator = new Navigator(this, navigator.Index, navigator.AllowNewRecord);
                return navigator;
            }
            navigator = new Navigator(this);
            return navigator;
        }

        public INavigator Navigate() => (INavigator)GetEnumerator();
        #endregion

        public virtual void Update(CRUD crud, ISQLModel model)
        {
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
                    int removedIndex = IndexOf((M)model);
                    if (!Remove((M)model)) break;
                    if (navigator != null)
                    {
                        if (navigator.BOF && !navigator.NoRecords) Controller?.GoFirst();
                        if (navigator.EOF && !navigator.NoRecords) Controller?.GoPrevious();
                        else Controller?.GoAt(removedIndex);
                    }
                    break;
            }

            RunFilter?.Invoke(this, new());
            if (crud == CRUD.UPDATE) //reasset the index if the RunFilter fired.
            {
                int index = IndexOf((M)model);
                Controller?.GoAt(index);
            }
        }

        /// <summary>
        /// This method is called in <see cref="Update(CRUD, ISQLModel)"/>.
        /// It loops through the <see cref="UIControls"/> to notify the <see cref="IUIControl"/> object to reflect changes that occured to their ItemsSource which is an instance of <see cref="RecordSource"/>.
        /// </summary>
        public void NotifyUIControl(object[] args)
        {
            if (UIControls != null && UIControls.Count > 0)
                foreach (IUIControl combo in UIControls) combo.OnItemSourceUpdated(args);
        }

        public void AddUIControlReference(IUIControl control)
        {
            if (UIControls == null) UIControls = [];
            UIControls.Add(control);
        }

        /// <summary>
        /// It takes an IAsyncEnumerable, converts it to a List and returns a RecordSource object.
        /// </summary>
        /// <param name="source"> An IAsyncEnumerable&lt;ISQLModel></param>
        /// <returns>Task&lt;RecordSource></returns>
        public static async Task<RecordSource<M>> CreateFromAsyncList(IAsyncEnumerable<M> source) =>
        new RecordSource<M>(await source.ToListAsync());

        /// <summary>
        /// It takes an IAsyncEnumerable, converts it to a List and returns a RecordSource object.
        /// </summary>
        /// <param name="source"> An IAsyncEnumerable&lt;ISQLModel></param>
        /// <returns>Task&lt;RecordSource></returns>
        public static async Task<IEnumerable<M>> CreateFromAsyncList2(IAsyncEnumerable<M> source) =>
        await source.ToListAsync();

        public virtual string RecordPositionDisplayer()
        {
            if (navigator == null) throw new NoNavigatorException();
            return true switch
            {
                true when navigator.NoRecords => "NO RECORDS",
                true when navigator.IsNewRecord => "New Record",
                _ => $"Record {navigator?.RecNum} of {navigator?.RecordCount}",
            };
        }

        public void Dispose()
        {
            ParentSource?.RemoveChild(this);
            UIControls?.Clear();
            navigator = null;
            RunFilter = null;
            GC.SuppressFinalize(this);
        }

        ~RecordSource()
        {
            Dispose();
        }
    }
}
