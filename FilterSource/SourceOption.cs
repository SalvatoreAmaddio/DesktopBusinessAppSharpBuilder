using Backend.Model;
using Backend.Source;
using FrontEnd.Controller;
using FrontEnd.Model;
using FrontEnd.Source;
using MvvmHelpers;
using Backend.Enums;
using FrontEnd.Forms;

namespace FrontEnd.FilterSource
{
    /// <summary>
    /// Represents a collection that extends <see cref="ObservableRangeCollection{T}"/> with elements of type <see cref="IFilterOption"/> and implements <see cref="IChildSource"/>.
    /// <para/>
    /// This class is designed to work in conjunction with the <see cref="HeaderFilter"/> class.
    /// </summary>
    public class SourceOption : ObservableRangeCollection<IFilterOption>, IChildSource, IDisposable
    {
        #region Variables
        /// <summary>
        /// A list of <see cref="IUIControl"/> associated with this data source.
        /// </summary>
        protected List<IUIControl>? uiControls;
        protected IDataSource? source;
        protected string _displayProperty = string.Empty;
        protected OrderBy _orderBy;
        private readonly string _orderByProperty = string.Empty;
        #endregion

        public IParentSource? ParentSource { get; set; }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceOption"/> class.
        /// </summary>
        internal SourceOption() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceOption"/> class with the specified data source, display property, ordering, and ordering property.
        /// </summary>
        /// <param name="source">The data source providing items for the collection.</param>
        /// <param name="displayProperty">The property of the data source to display in the option list.</param>
        /// <param name="orderBy">The ordering direction for the collection.</param>
        /// <param name="orderByProperty">The property used for ordering the collection.</param>
        public SourceOption(IDataSource source, string displayProperty, OrderBy orderBy = OrderBy.ASC, string orderByProperty = "") : base(source.Cast<IAbstractModel>().Select(s => new FilterOption(s, displayProperty)))
        {
            _orderByProperty = orderByProperty;
            _orderBy = orderBy;
            _displayProperty = displayProperty;
            this.source = source;
            this.source.ParentSource?.AddChild(this);
            ReplaceRange(OrderSource());
        }
        #endregion

        /// <summary>
        /// Orders the source collection based on the specified ordering criteria specified in the constructor.
        /// </summary>
        protected virtual IEnumerable<IFilterOption> OrderSource()
        {
            if (_orderBy == OrderBy.ASC)
                return this.OrderBy(s => s.Record.GetPropertyValue((string.IsNullOrEmpty(_orderByProperty)) ? _displayProperty : _orderByProperty)).ToList();
            else
                return this.OrderByDescending(s => s.Record.GetPropertyValue((string.IsNullOrEmpty(_orderByProperty)) ? _displayProperty : _orderByProperty)).ToList();
        }

        /// <summary>
        /// Retrieves all selected filter options from the collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{IFilterOption}"/> representing selected options.</returns>
        public IEnumerable<IFilterOption> SelectedOptions() => this.Where(s => s.IsSelected);

        /// <summary>
        /// Builds SQL logic to filter the select statement based on selected filter options.
        /// </summary>
        /// <typeparam name="T">The type of clause to construct.</typeparam>
        /// <param name="abstractClause">The abstract clause used to build SQL conditions.</param>
        public void Conditions<T>(AbstractClause abstractClause, string alias = "") where T : AbstractConditionalClause, IQueryClause, new()
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
                    ForEachItem(abstractClause, conditionalClause, item, i, alias);
                }
            }

            if (selectedCount > 0)
            {
                conditionalClause?.RemoveLastChange();
                conditionalClause?.CloseBracket();
            }
        }

        protected virtual void ForEachItem(AbstractClause abstractClause, AbstractConditionalClause? conditionalClause, IFilterOption item, int i, string alias = "")
        {
            string? tableName = item?.Record.GetTableName();
            string? fieldName = null;
            fieldName = item?.Record?.GetPrimaryKey()?.Name;
            abstractClause.AddParameter($"{fieldName}{i}", item?.Record?.GetPrimaryKey()?.GetValue());
            if (conditionalClause is HavingClause) 
                conditionalClause?.EqualsTo($"{(string.IsNullOrEmpty(alias) ? fieldName : alias)}", $"@{fieldName}{i}").OR();
            else
                if (string.IsNullOrEmpty(alias))
                    conditionalClause?.EqualsTo($"{tableName}.{fieldName}", $"@{fieldName}{i}").OR();
                else
                    conditionalClause?.EqualsTo($"{alias}", $"@{fieldName}{i}").OR();
        }

        public void AddUIControlReference(IUIControl control)
        {
            uiControls ??= [];
            uiControls.Add(control);
        }

        /// <summary>
        /// Notifies all <see cref="IUIControl"/> objects in the <see cref="uiControls"/> collection of updates to their ItemsSource property, which is an instance of <see cref="IUISource"/>.
        /// </summary>
        protected void NotifyUIControl(object[] args)
        {
            if (uiControls != null && uiControls.Count > 0)
                foreach (IUIControl control in uiControls) control.OnItemSourceUpdated(args);
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
            source?.ParentSource?.RemoveChild(this);
            uiControls?.Clear();

            foreach(IFilterOption option in this)
                option.Dispose();

            Clear();
            source?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Represents a specialized collection of filter options derived from <see cref="SourceOption"/>. 
    /// This class provides additional functionality for handling distinct selection and ordering based on primitive data types.
    /// <para/>
    /// This class is designed to work in conjunction with the <see cref="HeaderFilter"/> class.
    /// </summary>
    public class PrimitiveSourceOption : SourceOption
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveSourceOption"/> class using the specified controller and display property for ordering.
        /// </summary>
        /// <param name="controller">The controller containing the data source.</param>
        /// <param name="displayProperty">The property of the data source to display in the option list.</param>
        /// <param name="orderby">The ordering direction for the collection.</param>
        public PrimitiveSourceOption(IAbstractFormController controller, string displayProperty, OrderBy orderby = OrderBy.ASC) : this(controller.Source, displayProperty, orderby)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveSourceOption"/> class using the specified data source and display property for ordering.
        /// </summary>
        /// <param name="source">The data source providing items for the collection.</param>
        /// <param name="displayProperty">The property of the data source to display in the option list.</param>
        /// <param name="orderby">The ordering direction for the collection.</param>
        public PrimitiveSourceOption(IDataSource source, string displayProperty, OrderBy orderby = OrderBy.ASC)
        {
            this._orderBy = orderby;
            _displayProperty = displayProperty;
            base.source = source;
            base.source.ParentSource?.AddChild(this);
            IEnumerable<IFilterOption> options = OrderSource();
            ReplaceRange(options);
        }
        #endregion

        protected override void ForEachItem(AbstractClause abstractClause, AbstractConditionalClause? conditionalClause, IFilterOption item, int i, string alias = "")
        {
            string? tableName = item?.Record.GetTableName();
            string? fieldName = null;
            fieldName = _displayProperty;
            abstractClause.AddParameter($"{fieldName}{i}", item?.Record?.GetPropertyValue(_displayProperty));
            if (conditionalClause is HavingClause)
                conditionalClause?.EqualsTo($"{(string.IsNullOrEmpty(alias) ? fieldName : alias)}", $"@{fieldName}{i}").OR();
            else
                if (string.IsNullOrEmpty(alias))
                    conditionalClause?.EqualsTo($"{tableName}.{fieldName}", $"@{fieldName}{i}").OR();
                else
                    conditionalClause?.EqualsTo($"{alias}", $"@{fieldName}{i}").OR();
        }

        /// <summary>
        /// Selects distinct filter options from the collection and updates their selection state based on previously selected options.
        /// </summary>
        private void SelectDistinct()
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
        
        /// <summary>
        /// Compares two values for equality, handling specific primitive data types such as DateTime and TimeSpan.
        /// </summary>
        /// <param name="value1">The first value to compare.</param>
        /// <param name="value2">The second value to compare.</param>
        /// <returns>True if the values are equal; otherwise, false.</returns>
        private static bool CompareValues(object? value1, object? value2)
        {
            if (value1 == null || value2 == null) return false;

            if (value1 is DateTime date1 && value2 is DateTime date2)
                return date1.Date == date2.Date;

            if (value1 is TimeSpan time1 && value2 is TimeSpan time2)
                return time1 == time2;

            return value1 == value2;
        }

        protected override IEnumerable<IFilterOption> OrderSource()
        {
            IEnumerable<IAbstractModel?>? range = source?.Cast<IAbstractModel>().GroupBy(s => s.GetPropertyValue(_displayProperty)).Select(s => s.FirstOrDefault()).Distinct();
            if (range == null) throw new NullReferenceException();
            if (_orderBy == OrderBy.ASC)
                return range.Select(s => new FilterOption(s, _displayProperty)).OrderBy(s => s.Value).ToList();
            else
                return range.Select(s => new FilterOption(s, _displayProperty)).OrderByDescending(s => s.Value).ToList();
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

    /// <summary>
    /// Enum type used in <see cref="SourceOption"/>
    /// </summary>
    public enum OrderBy
    { 
        ASC = 0,
        DESC = 1,
    }
}
