using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms.Calendar
{
    public class CalendarSlot : Label
    {
        static CalendarSlot() => DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarSlot), new FrameworkPropertyMetadata(typeof(CalendarSlot)));

    }
}
