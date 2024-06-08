﻿using Backend.Database;
using Backend.Model;
using Backend.Source;
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
        private string _groupByProp = string.Empty;
        private bool _isPrimitiveData => (string.IsNullOrEmpty(_groupByProp)) ? false : true;
        private readonly string _displayProperty = string.Empty;
        /// <summary>
        /// A list of <see cref="IUIControl"/> associated to this <see cref="RecordSource"/>.
        /// </summary>
        private List<IUIControl>? UIControls;
        private IRecordSource? Source;
        private bool _unique = false;
        public IParentSource? ParentSource { get; set; }

        public SourceOption(IEnumerable<IFilterOption> source) : base(source) { }

        public SourceOption(IRecordSource source, string groupByProp, string displayProperty, bool unique = true) 
        {
            IEnumerable<IAbstractModel?> range = source.Cast<AbstractModel>().GroupBy(s => s.GetPropertyValue(displayProperty)).Select(s => s.FirstOrDefault()).Distinct();
            IEnumerable<IFilterOption> options = range.Select(s => new FilterOption(s, displayProperty));
            ReplaceRange(options);
            Source = source;
            _unique = unique;
            _groupByProp = groupByProp;
            _displayProperty = displayProperty;
            Source.ParentSource?.AddChild(this);
        }

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
        public IEnumerable<ISQLModel> Selected() => this.Where(s => s.IsSelected).Select(s=>s.Record);

        /// <summary>
        /// It loops through the List and builds the SQL logic to filter the Select the statement.
        /// </summary>
        /// <param name="filterQueryBuilder"></param>
        /// <returns>A string</returns>
        public void Conditions(IWhereClause filterQueryBuilder)
        {
            int i = 0;
            int selectedCount = Selected().Count();

            if (selectedCount > 0)
            {
                if (filterQueryBuilder.HasWhereClause())
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
                    if (_isPrimitiveData) 
                    {
                        fieldName = _displayProperty;
                        filterQueryBuilder.AddParameter($"{fieldName}{i}", item?.Record?.GetPropertyValue(_displayProperty));
                    }
                    else
                    {
                        fieldName = item?.Record?.GetTablePK()?.Name;
                        filterQueryBuilder.AddParameter($"{fieldName}{i}", item?.Record?.GetTablePK()?.GetValue());
                    }
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
        private void NotifyUIControl(object[] args)
        {
            if (UIControls != null && UIControls.Count > 0)
                foreach (IUIControl control in UIControls) control.OnItemSourceUpdated(args);
        }

        public void Update(CRUD crud, ISQLModel model)
        {
            FilterOption option = new(model, _displayProperty);
            if (_isPrimitiveData) 
                UpdatePrimitiveData(crud, option);
            else 
                UpdateRecord(crud,option);
        }

        private void UpdatePrimitiveData(CRUD crud, FilterOption option)
        {
            bool exist = this.Any(s => s.Equals(option));
            if (exist) 
            {
                UpdateRecord(crud, option);
                if (_unique) 
                    OnUnique(option);
            }
            else UpdateRecord(CRUD.INSERT, option);            
        }

        private void OnUnique(IFilterOption option)
        {
            if (_unique)
            {
                int count = this.Count(s => CountValues(s, option.Value));
                if (count > 1)
                    UpdateRecord(CRUD.DELETE, option);

                //further check
                IEnumerable<IFilterOption> options = DistinctOptions();

                if (this.Count != options.Count())
                {
                    List<IFilterOption> extra = options.Where(s => FilterValues(s, option.Value)).ToList();
                    foreach (var e in extra)
                        UpdateRecord(CRUD.INSERT, e);
                }
            }
        }

        private IEnumerable<IFilterOption> DistinctOptions() 
        {
            if (Source == null) throw new NullReferenceException();
            IEnumerable<IAbstractModel?> range = Source.Cast<AbstractModel>().GroupBy(s => s.GetPropertyValue(_displayProperty)).Select(s => s.FirstOrDefault()).Distinct();
            return range.Select(s => new FilterOption(s, _displayProperty));
        }

        private static bool FilterValues(IFilterOption record, object? value)
        {
            if (record == null || record.Value == null) return false;
            return !record.Value.Equals(value);
        }

        private static bool CountValues(IFilterOption record, object? value)
        {
            if (record == null || record.Value == null) return false;
            return record.Value.Equals(value);
        }

        private void UpdateRecord(CRUD crud, IFilterOption? option) 
        {
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
            Clear();
            Source?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
