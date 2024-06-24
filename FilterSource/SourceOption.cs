using Backend.Model;
using Backend.Source;
using FrontEnd.Controller;
using FrontEnd.Model;
using FrontEnd.Source;
using MvvmHelpers;
using Backend.Enums;

namespace FrontEnd.FilterSource
{
    /// <summary>
    /// This class extends ObservableRangeCollection&lt;<see cref="IFilterOption"/>> and implements <see cref="IChildSource"/>.
    /// <para/>
    /// This class works in conjunction with the <see cref="HeaderFilter"/> class.
    /// </summary>
    /// <param name="source">A RecordSource object</param>
    /// <param name="displayProperty">The Record's property to display in the option list.</param>
    public class SourceOption : ObservableRangeCollection<IFilterOption>, IChildSource, IDisposable
    {
        #region Variables
        /// <summary>
        /// A list of <see cref="IUIControl"/> associated to this <see cref="RecordSource"/>.
        /// </summary>
        protected List<IUIControl>? UIControls;
        protected IRecordSource? Source;
        protected string _displayProperty = string.Empty;
        protected OrderBy _orderBy;
        private string _orderByProperty = string.Empty;
        #endregion
        public IParentSource? ParentSource { get; set; }

        #region Constructors
        public SourceOption() { }
      //  public SourceOption(IEnumerable<IFilterOption> source) : base(source) { }

        public SourceOption(IRecordSource source, string displayProperty, OrderBy orderBy = OrderBy.ASC, string orderByProperty = "") : base(source.Cast<AbstractModel>().Select(s => new FilterOption(s, displayProperty)))
        {
            _orderByProperty = orderByProperty;
            _orderBy = orderBy;
            _displayProperty = displayProperty;
            Source = source;
            Source.ParentSource?.AddChild(this);
            ReplaceRange(OrderSource());
        }
        #endregion
        protected virtual IEnumerable<IFilterOption> OrderSource()
        {
            if (_orderBy == OrderBy.ASC)
                return this.OrderBy(s => s.Record.GetPropertyValue((string.IsNullOrEmpty(_orderByProperty)) ? _displayProperty : _orderByProperty)).ToList();
            else
                return this.OrderByDescending(s => s.Record.GetPropertyValue((string.IsNullOrEmpty(_orderByProperty)) ? _displayProperty : _orderByProperty)).ToList();
        }

        /// <summary>
        /// Returns all selected options.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{IFilterOption}"/></returns>
        public IEnumerable<IFilterOption> SelectedOptions() => this.Where(s => s.IsSelected);
        /// <summary>
        /// It loops through the List and builds the SQL logic to filter the Select the statement.
        /// </summary>
        /// <param name="abstractClause"></param>
        /// <returns>A string</returns>
        public virtual void Conditions<T>(AbstractClause abstractClause) where T : AbstractConditionalClause, IQueryClause, new()
        {
            int selectedCount = SelectedOptions().Count();
            if (selectedCount == 0) return;

            int i = 0;
            T? conditionalClause = abstractClause.GetClause<T>();

            if (conditionalClause == null)
                conditionalClause = abstractClause.OpenClause<T>();

            if (selectedCount > 0)
            {
                if (conditionalClause.HasConditions())
                    conditionalClause?.AND();

                conditionalClause?.OpenBracket();
            }

            foreach (var item in this)
            {
                if (item.IsSelected)
                {
                    i++;
                    ForEachItem(abstractClause, conditionalClause, item, i);
                }
            }

            if (selectedCount > 0)
            {
                conditionalClause?.RemoveLastChange();
                conditionalClause?.CloseBracket();
            }
        }
        protected virtual void ForEachItem(AbstractClause abstractClause, AbstractConditionalClause? conditionalClause, IFilterOption item, int i)
        {
            string? tableName = item?.Record.GetTableName();
            string? fieldName = null;
            fieldName = item?.Record?.GetPrimaryKey()?.Name;
            abstractClause.AddParameter($"{fieldName}{i}", item?.Record?.GetPrimaryKey()?.GetValue());
            if (conditionalClause is HavingClause) 
                conditionalClause?.EqualsTo($"{fieldName}", $"@{fieldName}{i}").OR();
            else 
                conditionalClause?.EqualsTo($"{tableName}.{fieldName}", $"@{fieldName}{i}").OR();
        }

        /// <summary>
        /// It adds a <see cref="IUIControl"/> object to the <see cref="UIControls"/>.
        /// <para/>
        /// If <see cref="UIControls"/> is null, it gets initialised.
        /// </summary>
        /// <param name="control">An object implementing <see cref="IUIControl"/></param>
        public void AddUIControlReference(IUIControl control)
        {
            UIControls ??= [];
            UIControls.Add(control);
        }

        /// <summary>
        /// This method is called in <see cref="Update(CRUD, ISQLModel)"/>.
        /// It loops through the <see cref="UIControls"/> to notify the <see cref="IUIControl"/> object to reflect changes that occured to their ItemsSource which is an instance of <see cref="RecordSource"/>.
        /// </summary>
        protected void NotifyUIControl(object[] args)
        {
            if (UIControls != null && UIControls.Count > 0)
                foreach (IUIControl control in UIControls) control.OnItemSourceUpdated(args);
        }

        public virtual void Update(CRUD crud, ISQLModel model)
        {
            FilterOption option = new(model, _displayProperty);
            if (option == null) return;
            switch (crud)
            {
                case CRUD.INSERT:
                    Add(option);
                    NotifyUIControl([option]);
                    break;
                case CRUD.UPDATE:
                    int index = IndexOf(option);
                    IFilterOption oldValue = this[index];
                    oldValue.Copy(option);
                    NotifyUIControl([]);
                    break;
                case CRUD.DELETE:
                    Remove(option);
                    break;
            }

            ReplaceRange(OrderSource());
        }

        public void Dispose()
        {
            Source?.ParentSource?.RemoveChild(this);
            UIControls?.Clear();

            foreach(IFilterOption option in this)
                option.Dispose();

            Clear();
            Source?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    public class PrimitiveSourceOption : SourceOption
    {
        #region Constructors
        public PrimitiveSourceOption(IAbstractFormController controller, string displayProperty, OrderBy orderby = OrderBy.ASC) : this(controller.Source, displayProperty, orderby)
        { }

        public PrimitiveSourceOption(IRecordSource source, string displayProperty, OrderBy orderby = OrderBy.ASC)
        {
            this._orderBy = orderby;
            _displayProperty = displayProperty;
            Source = source;
            Source.ParentSource?.AddChild(this);
            IEnumerable<IFilterOption> options = OrderSource();
            ReplaceRange(options);
        }
        #endregion

        protected override void ForEachItem(AbstractClause abstractClause, AbstractConditionalClause? conditionalClause, IFilterOption item, int i)
        {
            string? tableName = item?.Record.GetTableName();
            string? fieldName = null;
            fieldName = _displayProperty;
            abstractClause.AddParameter($"{fieldName}{i}", item?.Record?.GetPropertyValue(_displayProperty));
            if (conditionalClause is HavingClause)
                conditionalClause?.EqualsTo($"{fieldName}", $"@{fieldName}{i}").OR();
            else
                conditionalClause?.EqualsTo($"{tableName}.{fieldName}", $"@{fieldName}{i}").OR();
        }
        public void SelectDistinct()
        {

            IEnumerable<IFilterOption> options = OrderSource();

            if (options!=null) 
            {
                IEnumerable<IFilterOption> previouslySelected = SelectedOptions().ToList();
                ReplaceRange(options);
                foreach(var option in this) 
                {
                    if (previouslySelected.Any(s=> CompareValues(s.Value, option.Value))) 
                        option.IsSelected = true;
                }
            }

            NotifyUIControl(["UPDATE"]);
        }

        protected override IEnumerable<IFilterOption> OrderSource()
        {
            IEnumerable<IAbstractModel?>? range = Source?.Cast<AbstractModel>().GroupBy(s => s.GetPropertyValue(_displayProperty)).Select(s => s.FirstOrDefault()).Distinct();
            if (range == null) throw new NullReferenceException();
            if (_orderBy == OrderBy.ASC)
                return range.Select(s => new FilterOption(s, _displayProperty)).OrderBy(s => s.Value).ToList();
            else
                return range.Select(s => new FilterOption(s, _displayProperty)).OrderByDescending(s => s.Value).ToList();
        }

        private bool CompareValues(object? value1, object? value2)
        {
            if (value1 == null || value2 == null) return false;

            if (value1 is DateTime date1 && value2 is DateTime date2) 
                return date1.Date == date2.Date;

            if (value1 is TimeSpan time1 && value2 is TimeSpan time2)
                return time1 == time2;

            return value1 == value2;
        }

        public override void Update(CRUD crud, ISQLModel model)
        {
            FilterOption option = new(model, _displayProperty);
            if (option == null) return;
            switch (crud)
            {
                case CRUD.INSERT:
                    Add(option);
                    break;
                case CRUD.UPDATE:
                    Update(CRUD.INSERT, model);
                    break;
                case CRUD.DELETE:
                    Remove(option);
                    break;
            }
            SelectDistinct();
        }
    }

    public enum OrderBy 
    { 
        ASC = 0,
        DESC = 1,
    }
}
