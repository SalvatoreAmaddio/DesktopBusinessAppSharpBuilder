using System.Windows;
using FrontEnd.Forms.FormComponents;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This class instantiate a FormRow object which is used by a <see cref="Lista"/>'s <see cref="DataTemplate"/>.
    /// <para/>
    /// The <see cref="RecordTracker"/> object is disabled by default and should stay so.
    /// </summary>
    public class FormRow : Form
    {
        static FormRow() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FormRow), new FrameworkPropertyMetadata(typeof(FormRow)));

        public FormRow() => RecordTrackerRow = new(0);

        ~FormRow() { }
    }

}
