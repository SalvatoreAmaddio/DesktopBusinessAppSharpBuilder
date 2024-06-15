using Backend.Database;
using FrontEnd.Model;
using System.Globalization;
using System.Windows.Data;

namespace FrontEnd.Converters
{
    /// <summary>
    /// This class implements <see cref="IValueConverter"/> and is designed to help fetch records from a Database.
    /// The generic <c>M</c> represents the Model to be converted, the generic <c>D</c> represents the Database where the records needs to be fetched from. 
    /// </summary>
    /// <typeparam name="M"></typeparam>
    /// <typeparam name="D"></typeparam>
    public abstract class AbstractFetchModel<M, D> : IValueConverter where M : AbstractModel, new() where D : AbstractModel, new()
    {
        protected M? Record;
        protected IAbstractDatabase? Db => DatabaseManager.Find<D>();
        protected abstract string Sql { get; }
        protected readonly List<QueryParameter> para = [];
        public abstract object? Convert(object value, Type targetType, object parameter, CultureInfo culture);
        public virtual object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Record;

    }
}
