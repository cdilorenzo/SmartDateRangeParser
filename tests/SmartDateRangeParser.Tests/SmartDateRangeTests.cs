using System;
using Xunit;

namespace SmartDateRangeParser.Tests;

public class SmartDateRangeTests
{
    [Fact]
    public void ParsesTodayCorrectly()
    {
        var today = DateTime.Today;
        var range = SmartDateRange.Parse("today");

        Assert.Equal(today, range.Start);
        Assert.Equal(today, range.End);
    }

    [Fact]
    public void ParsesYesterdayCorrectly()
    {
        var yesterday = DateTime.Today.AddDays(-1);
        var range = SmartDateRange.Parse("yesterday");

        Assert.Equal(yesterday, range.Start);
        Assert.Equal(yesterday, range.End);
    }

    [Theory]
    [InlineData("last 1 business day", 1)]
    [InlineData("last 2 business days", 2)]
    [InlineData("last 5 business days", 5)]
    public void ParsesLastBusinessDaysCorrectly(string input, int days)
    {
        var expectedEnd = DateTime.Today.AddDays(-1);
        var expectedStart = SubtractBusinessDays(expectedEnd, days - 1);

        var range = SmartDateRange.Parse(input);

        Assert.Equal(expectedStart, range.Start);
        Assert.Equal(expectedEnd, range.End);
    }

    [Theory]
    [InlineData("LAST 3 BUSINESS DAYS")]   // Uppercase
    [InlineData(" Last 3 Business Days ")] // Whitespace
    [InlineData("last 03 business day")]   // Leading zero
    public void IgnoresCaseAndWhitespace(string input)
    {
        var result = SmartDateRange.Parse(input);
        Assert.NotEqual(default, result);
    }

    [Theory]
    [InlineData("last x business days")]
    [InlineData("last -3 business days")]
    [InlineData("last 0 business days")]
    public void ThrowsOnInvalidBusinessDayInput(string input)
    {
        Assert.ThrowsAny<Exception>(() => SmartDateRange.Parse(input));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ThrowsOnEmptyOrNullInput(string? input)
    {
        Assert.ThrowsAny<ArgumentException>(() => SmartDateRange.Parse(input!));
    }

    [Fact]
    public void ThrowsOnUnsupportedExpression()
    {
        Assert.Throws<NotSupportedException>(() => SmartDateRange.Parse("last quarter"));
    }

    // Local helper to verify logic correctness
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
