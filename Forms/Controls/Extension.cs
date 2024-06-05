using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FrontEnd.Forms
{
    public static class Extension
    {
        public static readonly DependencyProperty IsWithinListProperty =
        DependencyProperty.RegisterAttached("IsWithinList", typeof(bool), typeof(Extension),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnIsWithinListPropertyChanged));

        public static bool GetIsWithinList(DependencyObject obj) => (bool)obj.GetValue(IsWithinListProperty);
        public static void SetIsWithinList(DependencyObject obj, bool value) => obj.SetValue(IsWithinListProperty, value);

        private static void OnIsWithinListPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool IsWithinList = (bool)e.NewValue;
            Button btn = (Button)d;

            if (IsWithinList)
            {
                btn.SetBinding(Button.DataContextProperty, new Binding("DataContext")
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Lista), 1)
                });
            }
            else BindingOperations.ClearBinding(btn, Button.DataContextProperty);
        }
    }
}
