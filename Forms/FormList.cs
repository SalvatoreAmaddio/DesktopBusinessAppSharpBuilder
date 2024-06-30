using System.Windows;

namespace FrontEnd.Forms
{
    /// <summary>
    /// Represents a form specifically designed to handle <see cref="Lista"/> objects.
    /// <para/>
    public class FormList : AbstractForm
    {
        /// <summary>
        /// Initializes static members of the <see cref="FormList"/> class.
        /// Overrides the default style key property metadata for the <see cref="FormList"/> class.
        /// </summary>
        static FormList() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FormList), new FrameworkPropertyMetadata(typeof(FormList)));

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            if (newContent is not Lista) throw new Exception();
            base.OnContentChanged(oldContent, newContent);
        }

    }
}
