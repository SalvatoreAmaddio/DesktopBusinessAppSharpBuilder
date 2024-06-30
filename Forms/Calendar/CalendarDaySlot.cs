using FrontEnd.Model;
using System.Windows;

namespace FrontEnd.Forms.Calendar
{
    /// <summary>
    /// Represents a day slot in a calendar, inheriting from the <see cref="CalendarSlot"/> class.
    /// </summary>
    public class CalendarDaySlot : CalendarSlot
    {
        private IEnumerable<IAbstractModel>? _records;

        /// <summary>
        /// Gets or sets a collection of records associated with this slot.
        /// </summary>
        public IEnumerable<IAbstractModel>? Records
        {
            get => _records;
            set
            {
                _records = value;
                if (_records?.Count() > 0)
                    HasAppointment = Visibility.Visible;
            }
        }

        /// <summary>
        /// Gets or sets the date associated with this calendar day slot.
        /// </summary>
        public DateTime Date { get; set; }

        #region HasAppointment
        /// <summary>
        /// Gets or sets a value indicating whether this slot has an appointment.
        /// </summary>
        public Visibility HasAppointment
        {
            get => (Visibility)GetValue(HasAppointmentProperty);
            set => SetValue(HasAppointmentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="HasAppointment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HasAppointmentProperty =
        DependencyProperty.Register(nameof(HasAppointment), typeof(Visibility), typeof(CalendarDaySlot), new PropertyMetadata(Visibility.Hidden));
        #endregion

        #region IsSelected
        /// <summary>
        /// Gets or sets a value indicating whether this slot is selected.
        /// </summary>
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsSelected"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(CalendarDaySlot), new PropertyMetadata(false, null));
        #endregion

        #region IsFestive
        /// <summary>
        /// Gets or sets a value indicating whether this slot is marked as festive.
        /// </summary>
        public bool IsFestive
        {
            get => (bool)GetValue(IsFestiveProperty);
            set => SetValue(IsFestiveProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsFestive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFestiveProperty =
        DependencyProperty.Register(nameof(IsFestive), typeof(bool), typeof(CalendarDaySlot), new PropertyMetadata(false, null));
        #endregion

        /// <summary>
        /// Gets the number of records associated with this slot.
        /// </summary>
        public int Count
        {
            get 
            { 
                if (Records == null) return 0;
                return Records.Count();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this slot represents today's date.
        /// </summary>
        public bool IsToday { get => this.Date == DateTime.Now; }

        /// <summary>
        /// Initializes the <see cref="CalendarDaySlot"/> class.
        /// </summary>
        static CalendarDaySlot() => DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarDaySlot), new FrameworkPropertyMetadata(typeof(CalendarDaySlot)));

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarDaySlot"/> class.
        /// </summary>
        public CalendarDaySlot() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarDaySlot"/> class with the specified date.
        /// </summary>
        /// <param name="date">The date to associate with this slot.</param>
        public CalendarDaySlot(DateTime date) 
        {
            this.Date = date;
            if (this.Date == DateTime.Today) IsSelected = true;
            Content = $"{Date.Day})";
        }     
    }
}