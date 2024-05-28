using Backend.Database;
using Backend.Model;
using Backend.Source;
using FrontEnd.Controller;
using FrontEnd.Model;
using FrontEnd.Source;
using MvvmHelpers;
using System.Text;

namespace FrontEnd.FilterSource
{
    /// <summary>
    /// This class extends ObservableRangeCollection&lt;<see cref="IFilterOption"/>> and implements <see cref="IChildSource"/>.
    /// <para/>
    /// This class works in conjunction with the <see cref="HeaderFilter"/> class.
    /// </summary>
    /// <param name="source">A RecordSource object</param>
    /// <param name="displayProperty">The Record's property to display in the option list.</param>
    public class SourceOption : ObservableRangeCollection<IFilterOption>, IChildSource
    {
        private readonly string _displayProperty;
        /// <summary>
        /// A list of <see cref="IUIControl"/> associated to this <see cref="RecordSource"/>.
        /// </summary>
        private List<IUIControl>? UIControls;

        public IParentSource? ParentSource { get; set; }

        public SourceOption(IRecordSource source, string displayProperty) : base(source.Cast<AbstractModel>().Select(s => new FilterOption(s, displayProperty)))
        {
            _displayProperty = displayProperty;
            source.ParentSource?.AddChild(this);
        }

        /// <summary>
        /// It loops through the List and builds the SQL logic to filter the Select the statement.
        /// </summary>
        /// <param name="filterQueryBuilder"></param>
        /// <returns>A string</returns>
        public string Conditions(FilterQueryBuilder filterQueryBuilder)
        {
            StringBuilder sb = new();
            int i = 0;

            foreach (var item in this)
            {
                if (item.IsSelected)
                {
                    string? fieldName = item?.Record?.GetTablePK()?.Name;
                    sb.Append($"{fieldName} = @{fieldName}{++i} OR ");
                    filterQueryBuilder.AddParameter($"{fieldName}{i}", item?.Record?.GetTablePK()?.GetValue());
                }
            }

            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
                if (sb.ToString(sb.Length - 2, 2) == "OR")
                {
                    sb.Remove(sb.Length - 2, 2);
                    sb.Remove(sb.Length - 1, 1);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// It adds a <see cref="IUIControl"/> object to the <see cref="UIControls"/>.
        /// <para/>
        /// If <see cref="UIControls"/> is null, it gets initialised.
        /// </summary>
        /// <param name="control">An object implementing <see cref="IUIControl"/></param>
        public void AddUIControlReference(IUIControl control)
        {
            if (UIControls == null) UIControls = [];
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
    }
}
