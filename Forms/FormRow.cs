using System.Windows;
using FrontEnd.Forms.FormComponents;

namespace FrontEnd.Forms
{
    /// <summary>
    /// Represents a form row used within a <see cref="Lista"/> data template.
    /// This class is a specialized version of <see cref="Form"/> designed for use in lists.
    /// <para/>
    /// The <see cref="RecordTracker"/> object is disabled by default and is intended to remain disabled.
    /// </summary>
    public class FormRow : Form
    {
        /// <summary>
        /// Initializes static members of the <see cref="FormRow"/> class.
        /// Overrides the default style key property metadata for the <see cref="FormRow"/> class.
        /// </summary>
        static FormRow() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FormRow), new FrameworkPropertyMetadata(typeof(FormRow)));

        /// <summary>
        /// Initializes a new instance of the <see cref="FormRow"/> class.
        /// Disables the <see cref="RecordTrackerRow"/> by setting its height to 0.
        /// </summary>
        public FormRow() => RecordTrackerRow = new(0);
    }

}
