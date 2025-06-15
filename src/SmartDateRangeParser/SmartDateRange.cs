using System;
using System.Text.RegularExpressions;

namespace SmartDateRangeParser
{
    public static class SmartDateRange
    {
        public static (DateTime? Start, DateTime? End) Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return (null, null);

            text = text.Trim().ToLowerInvariant();

            if (text == "today")
            {
                var today = DateTime.Today;
                return (today, today);
            }

            if (text == "yesterday")
            {
                var yesterday = DateTime.Today.AddDays(-1);
                return (yesterday, yesterday);
            }

            if (text == "last week")
            {
                var end = DateTime.Today.AddDays(-1);
                var start = end.AddDays(-6);
                return (start, end);
            }

            if (text == "last month")
            {
                var end = DateTime.Today.AddDays(-1);
                var start = end.AddMonths(-1).AddDays(1);
                return (start, end);
            }

            // "from 2024-01-01 to 2024-01-31"
            if (text.IndexOf("from", StringComparison.OrdinalIgnoreCase) >= 0 &&
                text.IndexOf("to", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var match = Regex.Match(text, @"from\s+([\d\-\/]+)\s+to\s+([\d\-\/]+)");
                if (match.Success)
                {
                    if (DateTime.TryParse(match.Groups[1].Value, out var start) &&
                        DateTime.TryParse(match.Groups[2].Value, out var end))
                    {
                        return (start, end);
                    }
                }
            }

            return (null, null);
        }
    }
}
