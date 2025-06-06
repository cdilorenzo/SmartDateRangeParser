using System;
using System.Linq;

namespace SmartDateRangeParser
{
    public class SmartDateRange
    {
        public DateTime Start { get; }
        public DateTime End { get; }

        private SmartDateRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public static SmartDateRange Parse(string input)
        {
            input = input.ToLowerInvariant();

            if (input == "today")
            {
                var today = DateTime.Today;
                return new SmartDateRange(today, today);
            }

            if (input.StartsWith("last "))
            {
                if (input.Contains("business days"))
                {
                    int days = ExtractNumber(input);
                    var end = DateTime.Today.AddDays(-1);
                    var start = SubtractBusinessDays(end, days - 1);
                    return new SmartDateRange(start, end);
                }
            }

            throw new NotSupportedException($"Unsupported format: {input}");
        }

        private static int ExtractNumber(string input)
        {
            return int.TryParse(new string(input.Where(char.IsDigit).ToArray()), out int result)
                ? result
                : throw new FormatException("Could not extract number from input.");
        }

        private static DateTime SubtractBusinessDays(DateTime from, int days)
        {
            while (days > 0)
            {
                from = from.AddDays(-1);
                if (from.DayOfWeek != DayOfWeek.Saturday && from.DayOfWeek != DayOfWeek.Sunday)
                    days--;
            }
            return from;
        }
    }
}
