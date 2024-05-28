using System.Windows;
using FrontEnd.Forms.FormComponents;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This class initiates a Form object meant to deal with a <see cref="Lista"/> object.
    /// <para/>
    /// A Form List object comes with a <see cref="RecordTracker"/>
    /// </summary>
    public class FormList : AbstractForm
    {
        static FormList() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FormList), new FrameworkPropertyMetadata(typeof(FormList)));

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            if (newContent is not Lista) throw new Exception();
            base.OnContentChanged(oldContent, newContent);
        }

        ~FormList() { }

    }
}
