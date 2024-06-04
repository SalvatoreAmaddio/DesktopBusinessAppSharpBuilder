using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FrontEnd.Forms.Calendar
{
    public delegate void MyClickHandler(object sender, MouseButtonEventArgs e);
    public class CalendarForm : Control, IDisposable
    {
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

        #region CurrentDate
        public DateTime CurrentDate
        {
            get => (DateTime)GetValue(CurrentDateProperty);
            set => SetValue(CurrentDateProperty, value);
        }

        public static readonly DependencyProperty CurrentDateProperty =
        DependencyProperty.Register(nameof(CurrentDate), typeof(DateTime), typeof(CalendarForm), new PropertyMetadata(OnCurrentDatePropertyChanged));

        private static async void OnCurrentDatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        await ((CalendarForm)d).OnDateUpdate();
        #endregion

        static CalendarForm() => DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarForm), new FrameworkPropertyMetadata(typeof(CalendarForm)));

        public CalendarForm() 
        {
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e) => Dispose();

        public override void OnApplyTemplate()
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

            CurrentDate = DateTime.Today;
        }

        private void OnTodayClicked(object sender, RoutedEventArgs e)
        {
            CurrentDate = DateTime.Today;
        }

        private void OnPreviousMonthClicked(object sender, RoutedEventArgs e)
        {
            CurrentDate = CurrentDate.AddMonths(-1);
        }

        private void OnNextMonthClicked(object sender, RoutedEventArgs e)
        {
            CurrentDate = CurrentDate.AddMonths(1);
        }

        private void OnPreviousYearClicked(object sender, RoutedEventArgs e)
        {
            CurrentDate = CurrentDate.AddYears(-1);
        }

        private void OnNextYearClicked(object sender, RoutedEventArgs e)
        {
            CurrentDate = CurrentDate.AddYears(1);
        }

        private async Task OnDateUpdate() 
        {
            DateAnalyser dateAnalyser = new(CurrentDate);
            await dateAnalyser.Analyse();

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
                    column?.Children.Add(new CalendarDaySlot(day, date, null) { IsFestive = weekends });
            }
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
        }
    }

    public class DateAnalyser(DateTime date) : IDisposable
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

    public static class CalendarListExtension
    {
        public static void AddZero(this List<int> list) => list.Insert(0, 0);
    }

}
