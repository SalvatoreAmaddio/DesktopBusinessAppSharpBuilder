using FrontEnd.Events;
using FrontEnd.Model;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FrontEnd.Controller;

namespace FrontEnd.Forms.Calendar
{
    /// <summary>
    /// Represents a calendar form control that displays a calendar with selectable day slots.
    /// </summary>
    public class CalendarForm : AbstractControl
    {
        /// <summary>
        /// Gets the currently selected <see cref="CalendarDaySlot"/>.
        /// </summary>
        public CalendarDaySlot SelectedSlot { get => _currentSlots.First(s => s.IsSelected); }

        #region Slots
        private readonly List<CalendarDaySlot> _currentSlots = [];
        private StackPanel? PART_Mondays;
        private StackPanel? PART_Tuesdays;
        private StackPanel? PART_Wednesdays;
        private StackPanel? PART_Thursdays;
        private StackPanel? PART_Fridays;
        private StackPanel? PART_Saturdays;
        private StackPanel? PART_Sundays;
        #endregion

        #region RequeryCMD
        /// <summary>
        /// Gets the command to requery the calendar.
        /// </summary>
        public ICommand RequeryCMD
        {
            get => (ICommand)GetValue(RequeryCMDProperty);
            private set => SetValue(RequeryCMDProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RequeryCMD"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RequeryCMDProperty =
        DependencyProperty.Register(nameof(RequeryCMD), typeof(ICommand), typeof(CalendarForm), new PropertyMetadata());
        #endregion

        #region TodayCMD
        /// <summary>
        /// Gets the command to navigate to today's date.
        /// </summary>
        public ICommand TodayCMD
        {
            get => (ICommand)GetValue(TodayCMDProperty);
            private set => SetValue(TodayCMDProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TodayCMD"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TodayCMDProperty =
        DependencyProperty.Register(nameof(TodayCMD), typeof(ICommand), typeof(CalendarForm), new PropertyMetadata());
        #endregion

        #region PreviousYearCMD
        /// <summary>
        /// Gets the command to navigate to the previous year.
        /// </summary>
        public ICommand PreviousYearCMD
        {
            get => (ICommand)GetValue(PreviousYearCMDProperty);
            private set => SetValue(PreviousYearCMDProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PreviousYearCMD"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PreviousYearCMDProperty =
        DependencyProperty.Register(nameof(PreviousYearCMD), typeof(ICommand), typeof(CalendarForm), new PropertyMetadata());
        #endregion

        #region NextYearCMD
        /// <summary>
        /// Gets the command to navigate to the next year.
        /// </summary>
        public ICommand NextYearCMD
        {
            get => (ICommand)GetValue(NextYearCMDProperty);
            private set => SetValue(NextYearCMDProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="NextYearCMD"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NextYearCMDProperty =
        DependencyProperty.Register(nameof(NextYearCMD), typeof(ICommand), typeof(CalendarForm), new PropertyMetadata());
        #endregion

        #region NextMonthCMD
        /// <summary>
        /// Gets the command to navigate to the next month.
        /// </summary>
        public ICommand NextMonthCMD
        {
            get => (ICommand)GetValue(NextMonthCMDProperty);
            private set => SetValue(NextMonthCMDProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="NextMonthCMD"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NextMonthCMDProperty =
        DependencyProperty.Register(nameof(NextMonthCMD), typeof(ICommand), typeof(CalendarForm), new PropertyMetadata());
        #endregion

        #region PreviousMonthCMD
        /// <summary>
        /// Gets the command to navigate to the previous month.
        /// </summary>
        public ICommand PreviousMonthCMD
        {
            get => (ICommand)GetValue(PreviousMonthCMDProperty);
            private set => SetValue(PreviousMonthCMDProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PreviousMonthCMD"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PreviousMonthCMDProperty =
        DependencyProperty.Register(nameof(PreviousMonthCMD), typeof(ICommand), typeof(CalendarForm), new PropertyMetadata());
        #endregion

        #region PreviousWeekCMD
        /// <summary>
        /// Gets the command to navigate to the previous week.
        /// </summary>
        public ICommand PreviousWeekCMD
        {
            get => (ICommand)GetValue(PreviousWeekCMDProperty);
            private set => SetValue(PreviousWeekCMDProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PreviousWeekCMD"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PreviousWeekCMDProperty =
        DependencyProperty.Register(nameof(PreviousWeekCMD), typeof(ICommand), typeof(CalendarForm), new PropertyMetadata());
        #endregion

        #region NextWeekCMD
        /// <summary>
        /// Gets the command to navigate to the next week.
        /// </summary>
        public ICommand NextWeekCMD
        {
            get => (ICommand)GetValue(NextWeekCMDProperty);
            private set => SetValue(NextWeekCMDProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="NextWeekCMD"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NextWeekCMDProperty =
        DependencyProperty.Register(nameof(NextWeekCMD), typeof(ICommand), typeof(CalendarForm), new PropertyMetadata());
        #endregion

        #region CurrentDate
        /// <summary>
        /// Gets or sets the current date displayed in the calendar.
        /// </summary>
        public DateTime CurrentDate
        {
            get => (DateTime)GetValue(CurrentDateProperty);
            set => SetValue(CurrentDateProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CurrentDate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentDateProperty =
        DependencyProperty.Register(nameof(CurrentDate), typeof(DateTime), typeof(CalendarForm), new PropertyMetadata(DateTime.Today));
        #endregion

        #region DisplayMode
        /// <summary>
        /// Gets or sets the display mode of the calendar.
        /// </summary>
        public int DisplayMode
        {
            get => (int)GetValue(DisplayModeProperty);
            set => SetValue(DisplayModeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DisplayMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayModeProperty =
        DependencyProperty.Register(nameof(DisplayMode), typeof(int), typeof(CalendarForm), new PropertyMetadata(0, OnDisplayModePropertyChanged));
        #endregion

        #region EnableWeekButton
        /// <summary>
        /// Gets or sets a value indicating whether the week button is enabled.
        /// </summary>
        public bool EnableWeekButton
        {
            get => (bool)GetValue(EnableWeekButtonProperty);
            set => SetValue(EnableWeekButtonProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="EnableWeekButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableWeekButtonProperty =
        DependencyProperty.Register(nameof(EnableWeekButton), typeof(bool), typeof(CalendarForm), new PropertyMetadata(false));
        #endregion

        #region OnDayClickEvent
        /// <summary>
        /// Identifies the OnDayClick routed event.
        /// </summary>
        public static readonly RoutedEvent OnDayClickEvent = EventManager.RegisterRoutedEvent(
        nameof(OnDayClick), RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(CalendarForm));

        /// <summary>
        /// Occurs when a day slot is clicked.
        /// </summary>
        public event MouseButtonEventHandler OnDayClick
        {
            add { AddHandler(OnDayClickEvent, value); }
            remove { RemoveHandler(OnDayClickEvent, value); }
        }
        #endregion

        #region OnPreparingEvent
        /// <summary>
        /// Identifies the OnPreparing routed event.
        /// </summary>
        public static readonly RoutedEvent OnPreparingEvent = EventManager.RegisterRoutedEvent(
        nameof(OnPreparing), RoutingStrategy.Bubble, typeof(OnPreparingCalendarFormEventHandler), typeof(CalendarForm));

        /// <summary>
        /// Occurs when the calendar form is preparing to update.
        /// </summary>
        public event OnPreparingCalendarFormEventHandler OnPreparing
        {
            add { AddHandler(OnPreparingEvent, value); }
            remove { RemoveHandler(OnPreparingEvent, value); }
        }
        #endregion

        /// <summary>
        /// Initializes the <see cref="CalendarForm"/> class.
        /// </summary>
        static CalendarForm() => DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarForm), new FrameworkPropertyMetadata(typeof(CalendarForm)));

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarForm"/> class.
        /// </summary>
        public CalendarForm()
        {
            RequeryCMD = new CMDAsync(OnDateUpdate);
            TodayCMD = new CMDAsync(() => Go(TimeTravel.TODAY));
            PreviousYearCMD = new CMDAsync(()=>Go(TimeTravel.PREV_YEAR));
            NextYearCMD = new CMDAsync(() => Go(TimeTravel.NEXT_YEAR));
            NextMonthCMD = new CMDAsync(() => Go(TimeTravel.NEXT_MONTH));
            PreviousMonthCMD = new CMDAsync(() => Go(TimeTravel.PREV_MONTH));
            PreviousWeekCMD = new CMDAsync(()=>Go(TimeTravel.PREV_WEEK));
            NextWeekCMD = new CMDAsync(() => Go(TimeTravel.NEXT_WEEK));
        }

        public override async void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Mondays = (StackPanel?)GetTemplateChild(nameof(PART_Mondays));
            PART_Tuesdays = (StackPanel?)GetTemplateChild(nameof(PART_Tuesdays));
            PART_Wednesdays = (StackPanel?)GetTemplateChild(nameof(PART_Wednesdays));
            PART_Thursdays = (StackPanel?)GetTemplateChild(nameof(PART_Thursdays));
            PART_Fridays = (StackPanel?)GetTemplateChild(nameof(PART_Fridays));
            PART_Saturdays = (StackPanel?)GetTemplateChild(nameof(PART_Saturdays));
            PART_Sundays = (StackPanel?)GetTemplateChild(nameof(PART_Sundays));
            await OnDateUpdate();
        }

        /// <summary>
        /// Handles changes to the <see cref="CalendarForm.DisplayMode"/> property.
        /// </summary>
        private static async void OnDisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        await ((CalendarForm)(d)).OnDateUpdate();

        /// <summary>
        /// Updates the calendar form with the current date and display mode.
        /// </summary>
        private async Task OnDateUpdate()
        {
            EnableWeekButton = (DisplayMode == 0) ? false : true;
            DateAnalyser dateAnalyser = new(CurrentDate, DisplayMode);
            await Task.Run(dateAnalyser.Analyse);

            ClearCalendar();

            FillSlots(dateAnalyser.Mondays, PART_Mondays);
            FillSlots(dateAnalyser.Tuesdays, PART_Tuesdays);
            FillSlots(dateAnalyser.Wednesdays, PART_Wednesdays);
            FillSlots(dateAnalyser.Thursdays, PART_Thursdays);
            FillSlots(dateAnalyser.Fridays, PART_Fridays);
            FillSlots(dateAnalyser.Saturdays, PART_Saturdays, true);
            FillSlots(dateAnalyser.Sundays, PART_Sundays, true);
        }

        /// <summary>
        /// Navigates to the specified time travel option and updates the date.
        /// </summary>
        /// <param name="timeTravel">The time travel option.</param>
        private async Task Go(TimeTravel timeTravel)
        {
            DateTime startOfWeek; 
            DateTime endOfWeek;
            
            switch (timeTravel)
            {
                case TimeTravel.TODAY:
                    CurrentDate = DateTime.Today;
                    break;
                case TimeTravel.PREV_YEAR:
                    CurrentDate = CurrentDate.AddYears(-1);
                    break;
                case TimeTravel.PREV_MONTH:
                    CurrentDate = CurrentDate.AddMonths(-1);
                    break;
                case TimeTravel.PREV_WEEK:
                    (startOfWeek, endOfWeek) = DateAnalyser.GetWeekRange(CurrentDate);
                    startOfWeek = startOfWeek.AddDays(-1);
                    CurrentDate = startOfWeek;
                    break;
                case TimeTravel.NEXT_YEAR:
                    CurrentDate = CurrentDate.AddYears(1);
                    break;
                case TimeTravel.NEXT_MONTH:
                    CurrentDate = CurrentDate.AddMonths(1);
                    break;
                case TimeTravel.NEXT_WEEK:
                    (startOfWeek, endOfWeek) = DateAnalyser.GetWeekRange(CurrentDate);
                    endOfWeek = endOfWeek.AddDays(1);
                    CurrentDate = endOfWeek;
                    break;
            }

            await OnDateUpdate();
        }

        /// <summary>
        /// Handles the mouse up event on a <see cref="CalendarDaySlot"/>.
        /// </summary>
        private void OnCalendarDaySlotMouseUp(object sender, MouseButtonEventArgs e)
        {
            CalendarDaySlot? prevSelectedSlot = _currentSlots.FirstOrDefault(s => s.IsSelected);
            if (prevSelectedSlot != null) prevSelectedSlot.IsSelected = false;

            CalendarDaySlot selectedSlot = (CalendarDaySlot)sender;
            selectedSlot.IsSelected = true;
            CurrentDate = selectedSlot.Date;

            RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
            {
                RoutedEvent = OnDayClickEvent,
                Source = this
            });
        }

        /// <summary>
        /// Fills the calendar slots with the specified dates.
        /// </summary>
        /// <param name="days">The list of dates to fill.</param>
        /// <param name="column">The column to fill the slots into.</param>
        /// <param name="weekends">Indicates whether the days are weekends.</param>
        private void FillSlots(List<DateTime?> days, StackPanel? column, bool weekends = false)
        {
            foreach (DateTime? date in days)
               column?.Children.Add((date is null) ? new CalendarSlot() : CreateSlot(date.Value, weekends));
        }

        /// <summary>
        /// Creates a new <see cref="CalendarDaySlot"/> for the specified date.
        /// </summary>
        /// <param name="date">The date to create the slot for.</param>
        /// <param name="weekends">Indicates whether the date is a weekend.</param>
        /// <returns>A new instance of <see cref="CalendarDaySlot"/>.</returns>
        private CalendarDaySlot CreateSlot(DateTime date, bool weekends = false) 
        {
            CalendarDaySlot slot = new(date) { IsFestive = weekends };
            slot.MouseUp += OnCalendarDaySlotMouseUp;
            slot.Records = RaiseOnPreparing(slot.Date);
            _currentSlots.Add(slot);
            return slot;
        }

        /// <summary>
        /// Raises the OnPreparing event for the specified date.
        /// </summary>
        /// <param name="date">The date to raise the event for.</param>
        /// <returns>A collection of records associated with the date.</returns>
        private IEnumerable<IAbstractModel>? RaiseOnPreparing(DateTime date)
        {
            OnPreparingCalendarFormEventArgs args = new(date, OnPreparingEvent, this);
            RaiseEvent(args);
            return args.Records;
        }

        public override void OnUnloaded(object sender, RoutedEventArgs e) { }
        public override void OnClosed(object? sender, EventArgs e) => Dispose();
        public override void DisposeEvents()
        {
            base.DisposeEvents();
            ClearCalendar();
        }

        /// <summary>
        /// Disposes the specified <see cref="CalendarDaySlot"/>.
        /// </summary>
        /// <param name="currentSlot">The slot to dispose.</param>
        private void DisposeCalendarDaySlot(CalendarDaySlot currentSlot)
        {
            currentSlot.Records = null;
            currentSlot.MouseUp -= OnCalendarDaySlotMouseUp;
        }

        /// <summary>
        /// Clears the calendar of all slots.
        /// </summary>
        public void ClearCalendar()
        {
            foreach (var slot in _currentSlots)
                DisposeCalendarDaySlot(slot);

            PART_Mondays?.Children.Clear();
            PART_Tuesdays?.Children.Clear();
            PART_Wednesdays?.Children.Clear();
            PART_Thursdays?.Children.Clear();
            PART_Fridays?.Children.Clear();
            PART_Saturdays?.Children.Clear();
            PART_Sundays?.Children.Clear();
            _currentSlots.Clear();
        }

        /// <summary>
        /// Analyzes the dates for the calendar form.
        /// </summary>
        internal class DateAnalyser(DateTime date, int displayMode = 0) : IDisposable
        {
            public int DisplayMode { get; } = displayMode;
            public DateTime Date { get; } = date;
            public List<DateTime?> Mondays { get; private set; } = [];
            public List<DateTime?> Tuesdays { get; private set; } = [];
            public List<DateTime?> Wednesdays { get; private set; } = [];
            public List<DateTime?> Thursdays { get; private set; } = [];
            public List<DateTime?> Fridays { get; private set; } = [];
            public List<DateTime?> Saturdays { get; private set; } = [];
            public List<DateTime?> Sundays { get; private set; } = [];
            private DayOfWeek MonthStartOn => new DateTime(this.Date.Year, this.Date.Month, 1).DayOfWeek;
            private int TotalDays => DateTime.DaysInMonth(this.Date.Year, this.Date.Month);

            /// <summary>
            /// Analyzes the dates for the calendar form based on the current display mode.
            /// </summary>
            /// <remarks>
            /// This method populates the lists of dates (Mondays, Tuesdays, etc.) for each day of the week.
            /// If the <see cref="DisplayMode"/> is 0, it fetches the dates for the entire month.
            /// If the <see cref="DisplayMode"/> is not 0, it fetches the dates for the current week.
            /// The method first clears the existing data by calling <see cref="Dispose"/>.
            /// It then populates the date lists either for the entire month or for the current week.
            /// Finally, it fills any voids in the calendar where there are no dates.
            /// </remarks>
            /// <returns>A task that represents the asynchronous operation.</returns>
            public async Task Analyse()
            {
                Dispose();
                List<Task<List<DateTime?>>> tasks = [];
                if (DisplayMode == 0) 
                {
                    tasks.Add(GetDays(DayOfWeek.Monday));
                    tasks.Add(GetDays(DayOfWeek.Tuesday));
                    tasks.Add(GetDays(DayOfWeek.Wednesday));
                    tasks.Add(GetDays(DayOfWeek.Thursday));
                    tasks.Add(GetDays(DayOfWeek.Friday));
                    tasks.Add(GetDays(DayOfWeek.Saturday));
                    tasks.Add(GetDays(DayOfWeek.Sunday));
                }
                else 
                {
                    (DateTime startOfWeek, DateTime endOfWeek) = GetWeekRange(Date);
                    Sundays.Add(endOfWeek);
                    Saturdays.Add(endOfWeek.AddDays(-1));
                    Fridays.Add(endOfWeek.AddDays(-2));
                    Thursdays.Add(endOfWeek.AddDays(-3));
                    Wednesdays.Add(endOfWeek.AddDays(-4));
                    Tuesdays.Add(endOfWeek.AddDays(-5));
                    Mondays.Add(endOfWeek.AddDays(-6));
                    return;
                }

                var results = await Task.WhenAll(tasks);

                Mondays = results[0];
                Tuesdays = results[1];
                Wednesdays = results[2];
                Thursdays = results[3];
                Fridays = results[4];
                Saturdays = results[5];
                Sundays = results[6];

                await FillVoids();
            }

            /// <summary>
            /// Fills voids in the calendar where there are no dates to ensure a consistent layout.
            /// </summary>
            /// <remarks>
            /// This method checks which day of the week the month starts on and adds null entries
            /// to the beginning of the date lists (Mondays, Tuesdays, etc.) to align the first date
            /// of the month correctly within the calendar grid. It ensures that each week starts from
            /// Monday, providing a complete and properly aligned calendar view.
            /// </remarks>
            /// <returns>A task that represents the asynchronous operation.</returns>
            private async Task FillVoids()
            {
                List<Task> tasks = [];

                if (StartsOnTuesday())
                    tasks.Add(Task.Run(Mondays.AddNull));

                if (StartsOnWednesday())
                {
                    tasks.Add(Task.Run(Mondays.AddNull));
                    tasks.Add(Task.Run(Tuesdays.AddNull));
                }

                if (StartsOnThursday())
                {
                    tasks.Add(Task.Run(Mondays.AddNull));
                    tasks.Add(Task.Run(Tuesdays.AddNull));
                    tasks.Add(Task.Run(Wednesdays.AddNull));
                }

                if (StartsOnFriday())
                {
                    tasks.Add(Task.Run(Mondays.AddNull));
                    tasks.Add(Task.Run(Tuesdays.AddNull));
                    tasks.Add(Task.Run(Wednesdays.AddNull));
                    tasks.Add(Task.Run(Thursdays.AddNull));
                }

                if (StartsOnSaturday())
                {
                    tasks.Add(Task.Run(Mondays.AddNull));
                    tasks.Add(Task.Run(Tuesdays.AddNull));
                    tasks.Add(Task.Run(Wednesdays.AddNull));
                    tasks.Add(Task.Run(Thursdays.AddNull));
                    tasks.Add(Task.Run(Fridays.AddNull));
                }

                if (StartsOnSunday())
                {
                    tasks.Add(Task.Run(Mondays.AddNull));
                    tasks.Add(Task.Run(Tuesdays.AddNull));
                    tasks.Add(Task.Run(Wednesdays.AddNull));
                    tasks.Add(Task.Run(Thursdays.AddNull));
                    tasks.Add(Task.Run(Fridays.AddNull));
                    tasks.Add(Task.Run(Saturdays.AddNull));
                }

                await Task.WhenAll(tasks);
            }

            /// <summary>
            /// Determines if the month starts on a Tuesday.
            /// </summary>
            public bool StartsOnTuesday() => MonthStartOn == DayOfWeek.Tuesday;

            /// <summary>
            /// Determines if the month starts on a Wednesday.
            /// </summary>
            public bool StartsOnWednesday() => MonthStartOn == DayOfWeek.Wednesday;

            /// <summary>
            /// Determines if the month starts on a Thursday.
            /// </summary>
            public bool StartsOnThursday() => MonthStartOn == DayOfWeek.Thursday;

            /// <summary>
            /// Determines if the month starts on a Friday.
            /// </summary>
            public bool StartsOnFriday() => MonthStartOn == DayOfWeek.Friday;

            /// <summary>
            /// Determines if the month starts on a Saturday.
            /// </summary>
            public bool StartsOnSaturday() => MonthStartOn == DayOfWeek.Saturday;

            /// <summary>
            /// Determines if the month starts on a Sunday.
            /// </summary>
            public bool StartsOnSunday() => MonthStartOn == DayOfWeek.Sunday;

            /// <summary>
            /// Gets the list of dates for a specified day of the week.
            /// </summary>
            /// <param name="dayName">The day of the week.</param>
            /// <returns>A task that represents the asynchronous operation. The task result contains the list of dates.</returns>
            private Task<List<DateTime?>> GetDays(DayOfWeek dayName)
            {
                List<DateTime?> days = [];
                DateTime date = new(this.Date.Year, this.Date.Month, 1);

                for (int i = 1; i <= TotalDays; i++)
                {
                    DayOfWeek dayOfWeek = date.DayOfWeek;
                    if (dayOfWeek == dayName)
                        days.Add(date);

                    date = date.AddDays(1);
                }

                return Task.FromResult(days);
            }

            /// <summary>
            /// Gets the start and end dates of the week that contains the specified date.
            /// </summary>
            /// <param name="date">The date.</param>
            /// <returns>A tuple containing the start and end dates of the week.</returns>
            public static (DateTime, DateTime) GetWeekRange(DateTime date)
            {
                CultureInfo cultureInfo = CultureInfo.CurrentCulture;
                DayOfWeek firstDayOfWeek = DayOfWeek.Monday;

                // Calculate the start of the week
                int diff = (7 + (date.DayOfWeek - firstDayOfWeek)) % 7;
                DateTime startOfWeek = date.AddDays(-1 * diff).Date;

                // Calculate the end of the week
                DateTime endOfWeek = startOfWeek.AddDays(6);

                return (startOfWeek, endOfWeek);
            }
            
            public void Dispose()
            {
                Mondays.Clear();
                Tuesdays.Clear();
                Wednesdays.Clear();
                Thursdays.Clear();
                Fridays.Clear();
                Saturdays.Clear();
                Sundays.Clear();
            }
        }

        /// <summary>
        /// TimeTravel enums.
        /// </summary>
        internal enum TimeTravel
        {
            PREV_YEAR = 0,
            PREV_MONTH = 1,
            PREV_WEEK = 2,
            NEXT_YEAR = 3,
            NEXT_MONTH = 4,
            NEXT_WEEK = 5,
            TODAY = 6,
        }
    }

    /// <summary>
    /// Provides extension methods for lists of nullable <see cref="DateTime"/> objects.
    /// </summary>
    public static class CalendarListExtension
    {
        /// <summary>
        /// Inserts a null entry at the beginning of the list.
        /// </summary>
        /// <param name="list">The list of nullable <see cref="DateTime"/> objects.</param>
        /// <remarks>
        /// This method is useful for aligning calendar dates within a grid, ensuring that the first date
        /// of the month starts on the correct day of the week. By adding null entries at the beginning,
        /// the calendar can maintain a consistent layout with proper spacing for days without dates.
        /// </remarks>
        public static void AddNull(this List<DateTime?> list) => list.Insert(0, null);
    }
}