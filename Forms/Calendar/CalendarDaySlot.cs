using FrontEnd.Model;
using System.Windows;

namespace FrontEnd.Forms.Calendar
{
    public class CalendarDaySlot : CalendarSlot
    {
        static CalendarDaySlot() => DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarDaySlot), new FrameworkPropertyMetadata(typeof(CalendarDaySlot)));

        public DateTime Date { get; set; }

        #region IsSelected
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(CalendarDaySlot), new PropertyMetadata(false, null));
        #endregion

        #region IsFestive
        public bool IsFestive
        {
            get => (bool)GetValue(IsFestiveProperty);
            set => SetValue(IsFestiveProperty, value);
        }

        public static readonly DependencyProperty IsFestiveProperty =
            DependencyProperty.Register(nameof(IsFestive), typeof(bool), typeof(CalendarDaySlot), new PropertyMetadata(false, null));
        #endregion

        public AbstractModel? Model { get; set; }
        public bool IsToday { get => this.Date == DateTime.Now; }

        public CalendarDaySlot() { }
        public CalendarDaySlot(int day, DateTime date) 
        { 
            this.Date = new DateTime(date.Year,date.Month,day);
            if (this.Date == DateTime.Today) IsSelected = true;
            Content = $"{day})";
        }

      
    }
}
