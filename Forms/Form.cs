using System.Windows;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This class initiates a Form object.
    /// <para/>
    /// A Form List object comes with a <see cref="FormComponents.RecordTracker"/> and a <see cref="FormComponents.RecordStatus"/> object.
    /// </summary>
    public class Form : AbstractForm
    {

        #region RecordStatusRow
        /// <summary>
        /// Gets and Sets a <see cref="GridLength"/> object which regulates the width of a <see cref="FormComponents.RecordStatus"/> object.
        /// </summary>
        public GridLength RecordStatusColumn
        {
            get => (GridLength)GetValue(RecordStatusColumnProperty);
            set => SetValue(RecordStatusColumnProperty, value);
        }

        public static readonly DependencyProperty RecordStatusColumnProperty =
            DependencyProperty.Register(nameof(RecordStatusColumn), typeof(GridLength), typeof(Form), new PropertyMetadata(new GridLength(23), null));
        #endregion

        static Form() => DefaultStyleKeyProperty.OverrideMetadata(typeof(Form), new FrameworkPropertyMetadata(typeof(Form)));

    }
}