using System;
using System.Globalization;

namespace SmartDateRangeParser;

public readonly struct SmartDateRange(DateTime start, DateTime end)
{
    public DateTime Start { get; } = start;
    public DateTime End { get; } = end;

    public static SmartDateRange Parse(string input)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(input);

        input = input.Trim().ToLowerInvariant();

        return input switch
        {
            "today" => new(DateTime.Today, DateTime.Today),
            "yesterday" => new(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-1)),
            _ when input.StartsWith("last ") && input.Contains("business day", StringComparison.OrdinalIgnoreCase)
                => ParseLastBusinessDays(input),
            _ => throw new NotSupportedException($"Unsupported date range expression: '{input}'")
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
}