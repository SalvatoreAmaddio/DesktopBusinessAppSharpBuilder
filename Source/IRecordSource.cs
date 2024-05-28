using Backend.Source;
using FrontEnd.Model;
using FrontEnd.Events;
using Backend.Controller;
using FrontEnd.Controller;

namespace FrontEnd.Source
{
    /// <summary>
    /// This interface serves as a bridge between a <see cref="RecordSource"/> object and a Combo object in the contest of GUI Development.
    /// The main purpose of this interface is to update the ComboBox control to reflect changes that occured in the ComboBox's ItemSource which is an instance of <see cref="RecordSource"/>
    /// <para/>
    /// Indeed, the <see cref="OnItemSourceUpdated"/> is called in <see cref="RecordSource.Update(CRUD, ISQLModel)"/>.
    /// </summary>
    public interface IUIControl
    {
        /// <summary>
        /// Whenever the Parent RecordSource is updated, it notifies this object to reflect the update.
        /// </summary>
        /// <param name="args"></param>
        public void OnItemSourceUpdated(object[] args);

    }

    public interface IRecordSource<M> : ICollection<M>, IRecordSource where M : AbstractModel, new()
    {
        /// <summary>
        /// This delegate works as a bridge between the <see cref="Controller.IAbstractSQLModelController"/> and this <see cref="Backend.Source.RecordSource"/>.
        /// If any filter operations has been implemented in the Controller, The RecordSource can trigger them.
        /// </summary>
        public event FilterEventHandler? RunFilter;


    }

    public interface IUISource 
    {
        public void NotifyUIControl(object[] args);

        /// <summary>
        /// It adds a <see cref="IUIControl"/> object to the <see cref="UIControls"/>.
        /// <para/>
        /// If <see cref="UIControls"/> is null, it gets initialised.
        /// </summary>
        /// <param name="control">An object implementing <see cref="IUIControl"/></param>
        public void AddUIControlReference(IUIControl control);
    }


}
