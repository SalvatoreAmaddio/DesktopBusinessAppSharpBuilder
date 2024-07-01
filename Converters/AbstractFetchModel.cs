using Backend.Database;
using FrontEnd.Model;
using System.Globalization;
using System.Windows.Data;

namespace FrontEnd.Converters
{
    /// <summary>
    /// This class implements <see cref="IValueConverter"/> and is designed to help fetch records from a database.
    /// The generic parameter <typeparamref name="M"/> represents the model to be converted,
    /// and the generic parameter <typeparamref name="D"/> represents the database where the records need to be fetched from.
    /// </summary>
    /// <typeparam name="M">The type of the model to be converted. Must implement <see cref="IAbstractModel"/> and have a parameterless constructor.</typeparam>
    /// <typeparam name="D">The type of the database where records are fetched. Must implement <see cref="IAbstractModel"/> and have a parameterless constructor.</typeparam>
    public abstract class AbstractFetchModel<M, D> : IValueConverter where M : IAbstractModel, new() where D : IAbstractModel, new()
    {
        /// <summary>
        /// The record of type <typeparamref name="M"/> being fetched.
        /// </summary>
        protected M? Record;

        /// <summary>
        /// Gets the database instance of type <typeparamref name="D"/> using the <see cref="DatabaseManager"/>.
        /// </summary>
        protected IAbstractDatabase? Db => DatabaseManager.Find<D>();

        /// <summary>
        /// Gets the SQL query string used to fetch records from the database.
        /// </summary>
        protected abstract string Sql { get; }

        /// <summary>
        /// A list of query parameters used in the SQL query.
        /// </summary>
        protected readonly List<QueryParameter> para = [];

        /// <summary>
        /// Converts the given value to the type <typeparamref name="D"/> by fetching the corresponding record from the database.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The fetched record of type <typeparamref name="D"/>.</returns>
        public D? Convert(object value) => (D?)Convert(value, null!, null!, null!);

        public Task<D?> ConvertAsync(object value) 
        {
            return Task.FromResult((D?)Convert(value, null!, null!, null!));
        }

        public abstract object? Convert(object value, Type targetType, object parameter, CultureInfo culture);
        public virtual object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Record;

    }
}
