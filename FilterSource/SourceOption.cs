using Backend.Database;
using Backend.Model;
using Backend.Source;
using FrontEnd.Controller;
using FrontEnd.Model;
using FrontEnd.Source;
using MvvmHelpers;

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
        /// <summary>
        /// A list of <see cref="IUIControl"/> associated to this <see cref="RecordSource"/>.
        /// </summary>
        protected List<IUIControl>? UIControls;
        protected IRecordSource? Source;
        public IParentSource? ParentSource { get; set; }
        protected string _displayProperty = string.Empty;

        public SourceOption() { }
        public SourceOption(IEnumerable<IFilterOption> source) : base(source) { }

        public SourceOption(IRecordSource source, string displayProperty) : base(source.Cast<AbstractModel>().Select(s => new FilterOption(s, displayProperty)))
        {
            Source = source;
            _displayProperty = displayProperty;
            Source.ParentSource?.AddChild(this);
        }


        /// <summary>
        /// Returns all selected options.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{IFilterOption}"/></returns>
     //   public IEnumerable<ISQLModel> SelectedRecords() => this.Where(s => s.IsSelected).Select(s => s.Record);
        public IEnumerable<IFilterOption> SelectedOptions() => this.Where(s => s.IsSelected);
        /// <summary>
        /// It loops through the List and builds the SQL logic to filter the Select the statement.
        /// </summary>
        /// <param name="filterQueryBuilder"></param>
        /// <returns>A string</returns>
        public virtual void Conditions(IWhereClause filterQueryBuilder)
        {
            int i = 0;
            int selectedCount = SelectedOptions().Count();
            
            if (selectedCount > 0)
            {
                if (filterQueryBuilder.HasWhereConditions())
                    filterQueryBuilder.AND();

                filterQueryBuilder.OpenBracket();
            }

            foreach (var item in this)
            {
                if (item.IsSelected)
                {
                    i++;
                    string? tableName = item?.Record.GetTableName();
                    string? fieldName = null;
                    fieldName = item?.Record?.GetTablePK()?.Name;
                    filterQueryBuilder.AddParameter($"{fieldName}{i}", item?.Record?.GetTablePK()?.GetValue());
                    filterQueryBuilder.EqualsTo($"{tableName}.{fieldName}", $"@{fieldName}{i}").OR();
                }
            }

            if (selectedCount > 0)
            {
                filterQueryBuilder.RemoveLastChange();
                filterQueryBuilder.CloseBracket();
            }
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
        public PrimitiveSourceOption(IAbstractFormController controller, string displayProperty) : this(controller.Source, displayProperty)
        { }

        public PrimitiveSourceOption(IRecordSource source, string displayProperty)
        {
            IEnumerable<IAbstractModel?> range = source.Cast<AbstractModel>().GroupBy(s => s.GetPropertyValue(displayProperty)).Select(s => s.FirstOrDefault()).Distinct();
            IEnumerable<IFilterOption> options = range.Select(s => new FilterOption(s, displayProperty));
            ReplaceRange(options);
            Source = source;
            _displayProperty = displayProperty;
            Source.ParentSource?.AddChild(this);
        }

        public override void Conditions(IWhereClause filterQueryBuilder)
        {
            int i = 0;
            int selectedCount = SelectedOptions().Count();

            if (selectedCount > 0)
            {
                if (filterQueryBuilder.HasWhereConditions())
                    filterQueryBuilder.AND();

                filterQueryBuilder.OpenBracket();
            }

            foreach (var item in this)
            {
                if (item.IsSelected)
                {
                    i++;
                    string? tableName = item?.Record.GetTableName();
                    string? fieldName = null;
                    fieldName = _displayProperty;
                    filterQueryBuilder.AddParameter($"{fieldName}{i}", item?.Record?.GetPropertyValue(_displayProperty));
                    filterQueryBuilder.EqualsTo($"{tableName}.{fieldName}", $"@{fieldName}{i}").OR();
                }
            }

            if (selectedCount > 0)
            {
                filterQueryBuilder.RemoveLastChange();
                filterQueryBuilder.CloseBracket();
            }
        }

        public void SelectDistinct()
        {
            IEnumerable<IAbstractModel?>? range = Source?.Cast<AbstractModel>().GroupBy(s => s.GetPropertyValue(_displayProperty)).Select(s => s.FirstOrDefault()).Distinct();
            IEnumerable<IFilterOption>? options = range?.Select(s=> new FilterOption(s, _displayProperty)).OrderBy(s => s.Value);
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
}
