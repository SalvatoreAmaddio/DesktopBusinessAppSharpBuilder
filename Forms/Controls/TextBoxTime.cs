using Backend.ExtensionMethods;
using System.Windows.Data;
using System.Windows;
using System.Windows.Input;

namespace FrontEnd.Forms
{
    public partial class TextBoxTime : Text
    {
        static TextBoxTime()
        {
            TextProperty.OverrideMetadata(
            typeof(TextBoxTime),
            new FrameworkPropertyMetadata(
                string.Empty,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                null,
                null,
                false,
                UpdateSourceTrigger.PropertyChanged
            ));
        }

        /// <summary>
        /// Allows only numeric input.
        /// </summary>
        /// <param name="e">Provides data about the text composition event.</param>
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);
            bool allowed = Int32.TryParse(e.Text, out _);
            if (!allowed)
            {
                e.Handled = true; //cancel the input
                return;
            }

            if (Text.Length == 5)
            {
                try 
                {
                    if (Text[CaretIndex].Equals(':'))
                    {
                        CaretIndex = CaretIndex + 1;
                        e.Handled = true; //cancel the input
                        return;
                    }
                }
                catch 
                {
                    e.Handled = true;
                    return;
                }

                int position = CaretIndex;
                Text = Text.ReplaceCharAt(position, e.Text[0]);
                CaretIndex = position + 1;

                try
                {
                    if (Text[CaretIndex].Equals(':'))
                    {
                        CaretIndex = CaretIndex + 1;
                    }
                }
                catch { }
                e.Handled = true; //cancel the input
                return;
            }
        }
    }
}
