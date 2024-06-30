using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms.Calendar
{
    /// <summary>
    /// Represents an empty slot in a calendar, inheriting from the <see cref="Label"/> class.
    /// </summary>
    public class CalendarSlot : Label
    {
        /// <summary>
        /// Initializes the <see cref="CalendarSlot"/> class by overriding the default style key 
        /// to ensure that styles are correctly applied to instances of <see cref="CalendarSlot"/>.
        /// </summary>
        static CalendarSlot() => DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarSlot), new FrameworkPropertyMetadata(typeof(CalendarSlot)));

    }
}
