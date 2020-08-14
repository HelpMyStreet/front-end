using System;

namespace HelpMyStreetFE.Helpers
{
    public static class DateTimeHelpers
    {
        public static string FriendlyFutureDate(this DateTime dateTimeDue)
        {
            DateTime dueDate = dateTimeDue.Date;
            DateTime today = DateTime.Now.Date;

            int daysUntilDate = (int)dueDate.Subtract(today).TotalDays;

            return (daysUntilDate switch
            {
                int i when i < -14 => $"{-1 * i / 7} weeks ago",
                int i when i < -1 => $"{-1 * i} days ago",
                -1 => "yesterday",
                0 => "today",
                1 => "tomorrow",
                int i when i <= 6 => $"on {dueDate.DayOfWeek}",
                int i when i <= 13 => $"next {dueDate.DayOfWeek}",
                int i when i >= 14 => $"in {i / 7} weeks",
                _ => $"on {dateTimeDue:dd/MM/yyyy}"
            });
        }

        public static string FriendlyPastDate(this DateTime dateTimeDue)
        {
            DateTime dueDate = dateTimeDue.Date;
            DateTime today = DateTime.Now.Date;

            int daysUntilDue = (int)dueDate.Subtract(today).TotalDays;

            return (daysUntilDue switch
            {
                int i when i < -13 => $"{-1 * i / 7} weeks ago",
                int i when i < -6 => $"last {dueDate.DayOfWeek}",
                int i when i < -1 => $"on {dueDate.DayOfWeek}",
                -1 => "yesterday",
                0 => "today",
                _ => $"on {dateTimeDue:dd/MM/yyyy}"
            });
        }
    }
}
