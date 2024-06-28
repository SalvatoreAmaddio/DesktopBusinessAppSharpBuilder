using FrontEnd.Model;
using System.Windows;

namespace FrontEnd.Forms.Calendar
{
    public class CalendarDaySlot : CalendarSlot
    {
        static CalendarDaySlot() => DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarDaySlot), new FrameworkPropertyMetadata(typeof(CalendarDaySlot)));

        public DateTime Date { get; set; }

        #region HasAppointment
        public Visibility HasAppointment
        {
            get => (Visibility)GetValue(HasAppointmentProperty);
            set => SetValue(HasAppointmentProperty, value);
        }

        public static readonly DependencyProperty HasAppointmentProperty =
            DependencyProperty.Register(nameof(HasAppointment), typeof(Visibility), typeof(CalendarDaySlot), new PropertyMetadata(Visibility.Hidden));
        #endregion

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

        private IEnumerable<IAbstractModel>? _model;
        
        /// <summary>
        /// Gets a <see cref="IEnumerable{AbstractModel}"/> containing the records associated with this Slot.
        /// </summary>
        public IEnumerable<IAbstractModel>? Records
        { 
            get => _model;
            set 
            { 
                _model = value;
                if (_model?.Count() > 0)
                    HasAppointment = Visibility.Visible;
            } 
        }
        
        /// <summary>
        /// Returns the number of <see cref="AbstractModel"/> associated with this Slot.
        /// </summary>
        public int Count
        {
            get 
            { 
                if (Records == null) return 0;
                return Records.Count();
            }
        }
        public bool IsToday { get => this.Date == DateTime.Now; }

        public CalendarDaySlot() { }
        public CalendarDaySlot(DateTime date) 
        {
            this.Date = date;
            if (this.Date == DateTime.Today) IsSelected = true;
            Content = $"{Date.Day})";
        }

      
    }
}
