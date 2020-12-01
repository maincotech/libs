namespace System
{
    public static class DateTimeExtensions
    {
        public static int DaysLeft(this DateTime date)
        {
            return date.Subtract(DateTime.Today).Days;
        }

        public static int HolidayDaysLeft(int month, int day)
        {
            var year = DateTime.Today.Year;

            var xdate = new DateTime(year, month, day);

            if (xdate <= DateTime.Today.Date)
            {
                xdate = xdate.AddYears(1);
            }

            return xdate.DaysLeft();
        }

        public static DateTime GetDate(int year, int month, DayOfWeek dayOfWeek, int weekOfMonth)
        {
            // TODO: some range checking (>0, for example)
            var day = new DateTime(year, month, 1);
            while (day.DayOfWeek != dayOfWeek) day = day.AddDays(1);
            if (weekOfMonth > 0)
            {
                return day.AddDays(7 * (weekOfMonth - 1));
            }
            // treat as last
            var last = day;
            while ((day = day.AddDays(7)).Month == last.Month)
            {
                last = day;
            }
            return last;
        }
    }
}