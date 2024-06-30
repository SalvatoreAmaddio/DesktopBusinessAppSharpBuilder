using System.Windows;
using FrontEnd.Forms.FormComponents;

namespace FrontEnd.Forms
{
    /// <summary>
    /// Represents a form that inherits from <see cref="AbstractForm"/> and provides additional functionality specific to forms.
    /// </summary>
    public class Form : AbstractForm
    {

        #region RecordStatusRow
        /// <summary>
        /// Gets or sets the width of the <see cref="RecordStatus"/> column.
        /// </summary>
        public GridLength RecordStatusColumn
        {
            get => (GridLength)GetValue(RecordStatusColumnProperty);
            set => SetValue(RecordStatusColumnProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RecordStatusColumn"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RecordStatusColumnProperty =
            DependencyProperty.Register(nameof(RecordStatusColumn), typeof(GridLength), typeof(Form), new PropertyMetadata(new GridLength(23), null));
        #endregion

        /// <summary>
        /// Initializes static members of the <see cref="Form"/> class.
        /// Overrides the default style key property metadata for the <see cref="Form"/> class.
        /// </summary>
        static Form() => DefaultStyleKeyProperty.OverrideMetadata(typeof(Form), new FrameworkPropertyMetadata(typeof(Form)));
    }
}