using FrontEnd.Events;
using FrontEnd.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FrontEnd.Forms.Calendar
{
    public class CalendarForm : Control, IDisposable
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
        private Button? PART_PreviousYear;
        private Button? PART_NextYear;
        private Button? PART_PreviousMonth;
        private Button? PART_NextMonth;
        private Button? PART_NextWeek;
        private Button? PART_PreviousWeek;
        private Button? PART_Today;
        private Button? PART_Requery;

        #region CurrentDate
        public DateTime CurrentDate
        {
            get => (DateTime)GetValue(CurrentDateProperty);
            set => SetValue(CurrentDateProperty, value);
        }

        public static readonly DependencyProperty CurrentDateProperty =
        DependencyProperty.Register(nameof(CurrentDate), typeof(DateTime), typeof(CalendarForm), new PropertyMetadata(DateTime.Today));
        #endregion

        static CalendarForm() => DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarForm), new FrameworkPropertyMetadata(typeof(CalendarForm)));

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

        public CalendarForm() 
        {
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e) => Dispose();

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

            PART_PreviousMonth = (Button?)GetTemplateChild(nameof(PART_PreviousMonth));
            PART_NextMonth = (Button?)GetTemplateChild(nameof(PART_NextMonth));
            PART_PreviousYear = (Button?)GetTemplateChild(nameof(PART_PreviousYear));
            PART_NextYear = (Button?)GetTemplateChild(nameof(PART_NextYear));
            PART_NextWeek = (Button?)GetTemplateChild(nameof(PART_NextWeek));
            PART_PreviousWeek = (Button?)GetTemplateChild(nameof(PART_PreviousWeek));
            PART_Today = (Button?) GetTemplateChild(nameof(PART_Today));
            PART_Requery = (Button?)GetTemplateChild(nameof(PART_Requery));

            if (PART_Requery!=null)
                PART_Requery.Click += OnRequeryClick;

            if (PART_NextYear!=null)
               PART_NextYear.Click += OnNextYearClicked;

            if (PART_PreviousYear != null)
                PART_PreviousYear.Click += OnPreviousYearClicked;

            if (PART_NextMonth != null)
                PART_NextMonth.Click += OnNextMonthClicked;

            if (PART_PreviousMonth != null)
                PART_PreviousMonth.Click += OnPreviousMonthClicked;

            if (PART_Today != null)
                PART_Today.Click += OnTodayClicked;

            await OnDateUpdate();
        }

        private async void OnRequeryClick(object sender, RoutedEventArgs e)
        {
            await OnDateUpdate(); 
        }

        private async void OnTodayClicked(object sender, RoutedEventArgs e)
        {
            CurrentDate = DateTime.Today;
            await OnDateUpdate();
        }

        private async void OnPreviousMonthClicked(object sender, RoutedEventArgs e) 
        {
            CurrentDate = CurrentDate.AddMonths(-1);
            await OnDateUpdate();
        }

        private async void OnNextMonthClicked(object sender, RoutedEventArgs e) 
        {
            CurrentDate = CurrentDate.AddMonths(1);
            await OnDateUpdate();
        }

        private async void OnPreviousYearClicked(object sender, RoutedEventArgs e) 
        {
            CurrentDate = CurrentDate.AddYears(-1);
            await OnDateUpdate();
        } 

        private async void OnNextYearClicked(object sender, RoutedEventArgs e) 
        {
            CurrentDate = CurrentDate.AddYears(1);
            await OnDateUpdate();
        }

        private async Task OnDateUpdate()
        {
            DateAnalyser dateAnalyser = new(CurrentDate);
            await Task.Run(dateAnalyser.Analyse);

            ClearCalendar();

            FillSlots(dateAnalyser.Mondays, dateAnalyser.Date, PART_Mondays);
            FillSlots(dateAnalyser.Tuesdays, dateAnalyser.Date, PART_Tuesdays);
            FillSlots(dateAnalyser.Wednesdays, dateAnalyser.Date, PART_Wednesdays);
            FillSlots(dateAnalyser.Thursdays, dateAnalyser.Date, PART_Thursdays);
            FillSlots(dateAnalyser.Fridays, dateAnalyser.Date, PART_Fridays);
            FillSlots(dateAnalyser.Saturdays, dateAnalyser.Date, PART_Saturdays, true);
            FillSlots(dateAnalyser.Sundays, dateAnalyser.Date, PART_Sundays, true);
        }

        private void FillSlots(List<int> days, DateTime date, StackPanel? column, bool weekends = false)
        {
            foreach (int day in days)
            {
                if (day == 0)
                    column?.Children.Add(new CalendarSlot());
                else 
                {
                    CalendarDaySlot c = new(day, date) { IsFestive = weekends };
                    c.MouseUp += C_MouseUp;
                    c.Unloaded += C_Unloaded;
                    c.Model = RaiseOnPreparing(date);
                    CurrentSlots.Add(c);
                    column?.Children.Add(c);
                }
            }
        }

        private AbstractModel? RaiseOnPreparing(DateTime date) 
        {
            OnPreparingCalendarFormEventArgs args = new(date, OnPreparingEvent, this);
            RaiseEvent(args);
            return args.Model;
        }

        private void C_Unloaded(object sender, RoutedEventArgs e)
        {
            CalendarDaySlot currentSlot = (CalendarDaySlot)sender;
            currentSlot.Unloaded -= C_Unloaded;
            currentSlot.MouseUp -= C_MouseUp;
        }

        private void C_MouseUp(object sender, MouseButtonEventArgs e)
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

        public void ClearCalendar()
        {
            PART_Mondays?.Children.Clear();
            PART_Tuesdays?.Children.Clear();
            PART_Wednesdays?.Children.Clear();
            PART_Thursdays?.Children.Clear();
            PART_Fridays?.Children.Clear();
            PART_Saturdays?.Children.Clear();
            PART_Sundays?.Children.Clear();
            CurrentSlots.Clear();
        }

        public void Dispose()
        {
            Unloaded -= OnUnloaded;
            if (PART_NextYear != null)
                PART_NextYear.Click -= OnNextYearClicked;

            if (PART_PreviousYear != null)
                PART_PreviousYear.Click -= OnPreviousYearClicked;

            if (PART_NextMonth != null)
                PART_NextMonth.Click -= OnNextMonthClicked;

            if (PART_PreviousMonth != null)
                PART_PreviousMonth.Click -= OnPreviousMonthClicked;

            if (PART_Today != null)
                PART_Today.Click -= OnTodayClicked;

            if (PART_Requery != null)
                PART_Requery.Click -= OnRequeryClick;

            CurrentSlots.Clear();
        }

        internal class DateAnalyser(DateTime date) : IDisposable
        {
            public DateTime Date { get; } = date;
            public List<int> Mondays { get; private set; } = [];
            public List<int> Tuesdays { get; private set; } = [];
            public List<int> Wednesdays { get; private set; } = [];
            public List<int> Thursdays { get; private set; } = [];
            public List<int> Fridays { get; private set; } = [];
            public List<int> Saturdays { get; private set; } = [];
            public List<int> Sundays { get; private set; } = [];
            private DayOfWeek MonthStartOn => new DateTime(this.Date.Year, this.Date.Month, 1).DayOfWeek;
            private int TotalDays => DateTime.DaysInMonth(this.Date.Year, this.Date.Month);

            public async Task Analyse()
            {
                Dispose();
                List<Task<List<int>>> tasks = [];
                tasks.Add(GetDays(DayOfWeek.Monday));
                tasks.Add(GetDays(DayOfWeek.Tuesday));
                tasks.Add(GetDays(DayOfWeek.Wednesday));
                tasks.Add(GetDays(DayOfWeek.Thursday));
                tasks.Add(GetDays(DayOfWeek.Friday));
                tasks.Add(GetDays(DayOfWeek.Saturday));
                tasks.Add(GetDays(DayOfWeek.Sunday));

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
                    tasks.Add(Task.Run(Mondays.AddZero));

                if (StartsOnWednesday())
                {
                    tasks.Add(Task.Run(Mondays.AddZero));
                    tasks.Add(Task.Run(Tuesdays.AddZero));
                }

                if (StartsOnThursday())
                {
                    tasks.Add(Task.Run(Mondays.AddZero));
                    tasks.Add(Task.Run(Tuesdays.AddZero));
                    tasks.Add(Task.Run(Wednesdays.AddZero));
                }

                if (StartsOnFriday())
                {
                    tasks.Add(Task.Run(Mondays.AddZero));
                    tasks.Add(Task.Run(Tuesdays.AddZero));
                    tasks.Add(Task.Run(Wednesdays.AddZero));
                    tasks.Add(Task.Run(Thursdays.AddZero));
                }

                if (StartsOnSaturday())
                {
                    tasks.Add(Task.Run(Mondays.AddZero));
                    tasks.Add(Task.Run(Tuesdays.AddZero));
                    tasks.Add(Task.Run(Wednesdays.AddZero));
                    tasks.Add(Task.Run(Thursdays.AddZero));
                    tasks.Add(Task.Run(Fridays.AddZero));
                }

                if (StartsOnSunday())
                {
                    tasks.Add(Task.Run(Mondays.AddZero));
                    tasks.Add(Task.Run(Tuesdays.AddZero));
                    tasks.Add(Task.Run(Wednesdays.AddZero));
                    tasks.Add(Task.Run(Thursdays.AddZero));
                    tasks.Add(Task.Run(Fridays.AddZero));
                    tasks.Add(Task.Run(Saturdays.AddZero));
                }

                await Task.WhenAll(tasks);
            }

            public bool StartsOnTuesday() => MonthStartOn == DayOfWeek.Tuesday;
            public bool StartsOnWednesday() => MonthStartOn == DayOfWeek.Wednesday;
            public bool StartsOnThursday() => MonthStartOn == DayOfWeek.Thursday;
            public bool StartsOnFriday() => MonthStartOn == DayOfWeek.Friday;
            public bool StartsOnSaturday() => MonthStartOn == DayOfWeek.Saturday;
            public bool StartsOnSunday() => MonthStartOn == DayOfWeek.Sunday;

            private Task<List<int>> GetDays(DayOfWeek dayName)
            {
                List<int> days = [];
                DateTime date = new(this.Date.Year, this.Date.Month, 1);

                for (int i = 1; i <= TotalDays; i++)
                {
                    DayOfWeek dayOfWeek = date.DayOfWeek;
                    if (dayOfWeek == dayName)
                        days.Add(i);

                    date = date.AddDays(1);
                }

                return Task.FromResult(days);
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


    public static class CalendarListExtension
    {
        public static void AddZero(this List<int> list) => list.Insert(0, 0);
    }

}
