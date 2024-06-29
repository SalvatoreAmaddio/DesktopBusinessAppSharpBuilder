using Backend.Source;
using FrontEnd.Model;
using FrontEnd.Events;
using FrontEnd.Controller;

namespace FrontEnd.Source
{
    /// <summary>
    /// Represents a collection of objects implementing <see cref="IAbstractModel"/> and extends <see cref="IDataSource{M}"/>.
    /// </summary>
    /// <typeparam name="M">The type of objects that implement <see cref="IAbstractModel"/> and have a parameterless constructor.</typeparam>
    public interface IRecordSource<M> : ICollection<M>, IDataSource<M> where M : IAbstractModel, new()
    {
        /// <summary>
        /// Event triggered to initiate filter operations between the <see cref="IAbstractFormListController"/> and this record source.
        /// </summary>
        event FilterEventHandler? RunFilter;

        /// <summary>
        /// Replaces the existing records with the specified new source.
        /// </summary>
        /// <param name="newSource">The new source collection of records.</param>
        void ReplaceRecords(IEnumerable<M> newSource);
    }
}