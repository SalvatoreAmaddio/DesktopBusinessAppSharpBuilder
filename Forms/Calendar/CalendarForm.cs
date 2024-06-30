using FrontEnd.Events;
using FrontEnd.Model;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FrontEnd.Controller;

namespace FrontEnd.Forms.Calendar
{
    public class CalendarForm : AbstractControl
    {
        public CalendarDaySlot SelectedSlot { get => CurrentSlots.First(s => s.IsSelected); }
        private readonly List<CalendarDaySlot> CurrentSlots = [];
        private StackPanel? PART_Mondays;
        private StackPanel? PART_Tuesdays;
        private StackPanel? PART_Wednesdays;
        private StackPanel? PART_Thursdays;
        private StackPanel? PART_Fridays;
        private StackPanel? PART_Saturdays;
        private StackPanel? PART_Sundays;

        #region RequeryCMD
        public static readonly DependencyProperty RequeryCMDProperty =
        DependencyProperty.Register(nameof(RequeryCMD), typeof(ICommand), typeof(CalendarForm), new PropertyMetadata());

        public ICommand RequeryCMD 
        {
            get => (ICommand) GetValue(RequeryCMDProperty);
            private set => SetValue(RequeryCMDProperty, value);
        }
        #endregion

        #region TodayCMD
        public static readonly DependencyProperty TodayCMDProperty =
        DependencyProperty.Register(nameof(TodayCMD), typeof(ICommand), typeof(CalendarForm), new PropertyMetadata());

        public ICommand TodayCMD
        {
            get => (ICommand)GetValue(TodayCMDProperty);
            private set => SetValue(TodayCMDProperty, value);
        }
        #endregion

        #region PreviousYearCMD
        public static readonly DependencyProperty PreviousYearCMDProperty =
        DependencyProperty.Register(nameof(PreviousYearCMD), typeof(ICommand), typeof(CalendarForm), new PropertyMetadata());

        public ICommand PreviousYearCMD
        {
            get => (ICommand)GetValue(PreviousYearCMDProperty);
            private set => SetValue(PreviousYearCMDProperty, value);
        }
        #endregion

        #region NextYearCMD
        public static readonly DependencyProperty NextYearCMDProperty =
        DependencyProperty.Register(nameof(NextYearCMD), typeof(ICommand), typeof(CalendarForm), new PropertyMetadata());

        public ICommand NextYearCMD
        {
            get => (ICommand)GetValue(NextYearCMDProperty);
            private set => SetValue(NextYearCMDProperty, value);
        }
        #endregion

        #region NextMonthCMD
        public static readonly DependencyProperty NextMonthCMDProperty =
        DependencyProperty.Register(nameof(NextMonthCMD), typeof(ICommand), typeof(CalendarForm), new PropertyMetadata());

        public ICommand NextMonthCMD
        {
            get => (ICommand)GetValue(NextMonthCMDProperty);
            private set => SetValue(NextMonthCMDProperty, value);
        }
        #endregion

        #region PreviousMonthCMD
        public static readonly DependencyProperty PreviousMonthCMDProperty =
        DependencyProperty.Register(nameof(PreviousMonthCMD), typeof(ICommand), typeof(CalendarForm), new PropertyMetadata());

        public ICommand PreviousMonthCMD
        {
            get => (ICommand)GetValue(PreviousMonthCMDProperty);
            private set => SetValue(PreviousMonthCMDProperty, value);
        }
        #endregion

        #region PreviousWeekCMD
        public static readonly DependencyProperty PreviousWeekCMDProperty =
        DependencyProperty.Register(nameof(PreviousWeekCMD), typeof(ICommand), typeof(CalendarForm), new PropertyMetadata());

        public ICommand PreviousWeekCMD
        {
            get => (ICommand)GetValue(PreviousWeekCMDProperty);
            private set => SetValue(PreviousWeekCMDProperty, value);
        }
        #endregion

        #region NextWeekCMD
        public static readonly DependencyProperty NextWeekCMDProperty =
        DependencyProperty.Register(nameof(NextWeekCMD), typeof(ICommand), typeof(CalendarForm), new PropertyMetadata());

        public ICommand NextWeekCMD
        {
            get => (ICommand)GetValue(NextWeekCMDProperty);
            private set => SetValue(NextWeekCMDProperty, value);
        }
        #endregion

        #region CurrentDate
        public DateTime CurrentDate
        {
            get => (DateTime)GetValue(CurrentDateProperty);
            set => SetValue(CurrentDateProperty, value);
        }

        public static readonly DependencyProperty CurrentDateProperty =
        DependencyProperty.Register(nameof(CurrentDate), typeof(DateTime), typeof(CalendarForm), new PropertyMetadata(DateTime.Today));
        #endregion

        #region DisplayMode
        public int DisplayMode
        {
            get => (int)GetValue(DisplayModeProperty);
            set => SetValue(DisplayModeProperty, value);
        }

        public static readonly DependencyProperty DisplayModeProperty =
        DependencyProperty.Register(nameof(DisplayMode), typeof(int), typeof(CalendarForm), new PropertyMetadata(0, OnDisplayModePropertyChanged));

        private static async void OnDisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        await ((CalendarForm)(d)).OnDateUpdate();
        #endregion

        #region EnableWeekButton
        public bool EnableWeekButton
        {
            get => (bool)GetValue(EnableWeekButtonProperty);
            set => SetValue(EnableWeekButtonProperty, value);
        }

        public static readonly DependencyProperty EnableWeekButtonProperty =
            DependencyProperty.Register(nameof(EnableWeekButton), typeof(bool), typeof(CalendarForm), new PropertyMetadata(false));
        #endregion

        #region OnDayClickEvent
        public static readonly RoutedEvent OnDayClickEvent = EventManager.RegisterRoutedEvent(
        nameof(OnDayClick), RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(CalendarForm));

        public event MouseButtonEventHandler OnDayClick
        {
            add { AddHandler(OnDayClickEvent, value); }
            remove { RemoveHandler(OnDayClickEvent, value); }
        }
        #endregion

        #region OnPreparingEvent
        public static readonly RoutedEvent OnPreparingEvent = EventManager.RegisterRoutedEvent(
        nameof(OnPreparing), RoutingStrategy.Bubble, typeof(OnPreparingCalendarFormEventHandler), typeof(CalendarForm));

        public event OnPreparingCalendarFormEventHandler OnPreparing
        {
            add { AddHandler(OnPreparingEvent, value); }
            remove { RemoveHandler(OnPreparingEvent, value); }
        }
        #endregion

        static CalendarForm() => DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarForm), new FrameworkPropertyMetadata(typeof(CalendarForm)));

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
        public override void OnUnloaded(object sender, RoutedEventArgs e) { }
        public override void OnClosed(object? sender, EventArgs e) => Dispose();
        public override void DisposeEvents()
        {
            base.DisposeEvents();
            ClearCalendar();
        }

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

        #region Events
        private void OnCalendarDaySlotMouseUp(object sender, MouseButtonEventArgs e)
        {
            CalendarDaySlot? prevSelectedSlot = CurrentSlots.FirstOrDefault(s => s.IsSelected);
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
        #endregion

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

        private void FillSlots(List<DateTime?> days, StackPanel? column, bool weekends = false)
        {
            foreach (DateTime? date in days)
               column?.Children.Add((date is null) ? new CalendarSlot() : CreateSlot(date.Value, weekends));
        }
        private CalendarDaySlot CreateSlot(DateTime date, bool weekends = false) 
        {
            CalendarDaySlot slot = new(date) { IsFestive = weekends };
            slot.MouseUp += OnCalendarDaySlotMouseUp;
            slot.Records = RaiseOnPreparing(slot.Date);
            CurrentSlots.Add(slot);
            return slot;
        }
        private IEnumerable<IAbstractModel>? RaiseOnPreparing(DateTime date)
        {
            OnPreparingCalendarFormEventArgs args = new(date, OnPreparingEvent, this);
            RaiseEvent(args);
            return args.Records;
        }

        private void DisposeCalendarDaySlot(CalendarDaySlot currentSlot)
        {
            currentSlot.Records = null;
            currentSlot.MouseUp -= OnCalendarDaySlotMouseUp;
        }
        public void ClearCalendar()
        {
            foreach (var slot in CurrentSlots)
                DisposeCalendarDaySlot(slot);

            PART_Mondays?.Children.Clear();
            PART_Tuesdays?.Children.Clear();
            PART_Wednesdays?.Children.Clear();
            PART_Thursdays?.Children.Clear();
            PART_Fridays?.Children.Clear();
            PART_Saturdays?.Children.Clear();
            PART_Sundays?.Children.Clear();
            CurrentSlots.Clear();
        }

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
            public bool StartsOnTuesday() => MonthStartOn == DayOfWeek.Tuesday;
            public bool StartsOnWednesday() => MonthStartOn == DayOfWeek.Wednesday;
            public bool StartsOnThursday() => MonthStartOn == DayOfWeek.Thursday;
            public bool StartsOnFriday() => MonthStartOn == DayOfWeek.Friday;
            public bool StartsOnSaturday() => MonthStartOn == DayOfWeek.Saturday;
            public bool StartsOnSunday() => MonthStartOn == DayOfWeek.Sunday;
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
    }

    public enum TimeTravel 
    {
        PREV_YEAR = 0,
        PREV_MONTH = 1,
        PREV_WEEK = 2,
        NEXT_YEAR = 3,
        NEXT_MONTH = 4,
        NEXT_WEEK = 5,
        TODAY = 6,
    }

    public static class CalendarListExtension
    {
        public static void AddNull(this List<DateTime?> list) => list.Insert(0, null);
    }

}
