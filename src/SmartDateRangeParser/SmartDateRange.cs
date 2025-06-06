using System;
using System.Globalization;

namespace SmartDateRangeParser;

public readonly struct SmartDateRange(DateTime start, DateTime end)
{
    public DateTime Start { get; } = start;
    public DateTime End { get; } = end;

    public static SmartDateRange Parse(string input, string culture = "en")
    {
        if (!TryParse(input, out var range, culture))
            throw new NotSupportedException($"Unsupported date range expression: '{input}'");

        return range;
    }

    public static bool TryParse(string input, out SmartDateRange range, string culture = "en")
    {
        range = default;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        input = input.Trim().ToLowerInvariant();

        try
        {
            range = culture switch
            {
                "en" => ParseEnglish(input),
                // Add future culture-specific parsers here
                _ => throw new NotSupportedException($"Culture '{culture}' is not supported.")
            };
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static SmartDateRange ParseEnglish(string input)
    {
        return input switch
        {
            "today" => new(DateTime.Today, DateTime.Today),
            "yesterday" => new(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-1)),
            "this week" => new(GetStartOfWeek(DateTime.Today), DateTime.Today),
            "this month" => new(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), DateTime.Today),

            _ when input.StartsWith("last ") && input.Contains("business day", StringComparison.OrdinalIgnoreCase)
                => ParseLastBusinessDays(input),

            _ when input.StartsWith("last ") && input.Contains("day", StringComparison.OrdinalIgnoreCase)
                => ParseLastCalendarDays(input),

            _ => throw new NotSupportedException($"Unsupported expression: '{input}'")
        };
    }

    private static SmartDateRange ParseLastBusinessDays(string input)
    {
        int days = ExtractNumber(input);
        if (days <= 0)
            throw new ArgumentOutOfRangeException(nameof(days), "Number of business days must be greater than 0.");

        var end = DateTime.Today.AddDays(-1);
        var start = SubtractBusinessDays(end, days - 1);
        return new(start, end);
    }

    private static SmartDateRange ParseLastCalendarDays(string input)
    {
        int days = ExtractNumber(input);
        if (days <= 0)
            throw new ArgumentOutOfRangeException(nameof(days), "Number of days must be greater than 0.");

        var end = DateTime.Today.AddDays(-1);
        var start = end.AddDays(-(days - 1));
        return new(start, end);
    }

    private static int ExtractNumber(string input)
    {
        foreach (var token in input.Split(' '))
        {
            if (int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out int number))
                return number;
        }

        throw new FormatException("Could not find a valid number in input.");
    }

    private static DateTime SubtractBusinessDays(DateTime from, int count)
    {
        while (count > 0)
        {
            from = from.AddDays(-1);
            if (from.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday)
                count--;
        }
        return from;
    }

    private static DateTime GetStartOfWeek(DateTime date)
    {
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-diff).Date;
    }
}