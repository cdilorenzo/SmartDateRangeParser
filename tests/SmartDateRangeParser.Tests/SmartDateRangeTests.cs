using System;
using Xunit;

namespace SmartDateRangeParser.Tests
{
    public class SmartDateRangeTests
    {
        [Theory]
        [InlineData("today", 0)]
        [InlineData("yesterday", -1)]
        public void ParsesFixedKeywords(string input, int offset)
        {
            var expected = DateTime.Today.AddDays(offset);
            var range = SmartDateRange.Parse(input);
            Assert.Equal(expected, range.Start);
            Assert.Equal(expected, range.End);
        }

        [Theory]
        [InlineData("last 3 days", 3)]
        [InlineData("last 5 days", 5)]
        public void ParsesLastCalendarDays(string input, int days)
        {
            var end = DateTime.Today.AddDays(-1);
            var start = end.AddDays(-(days - 1));
            var range = SmartDateRange.Parse(input);
            Assert.Equal(start, range.Start);
            Assert.Equal(end, range.End);
        }

        [Theory]
        [InlineData("last 3 business days", 3)]
        [InlineData("last 5 business days", 5)]
        public void ParsesLastBusinessDays(string input, int days)
        {
            var end = DateTime.Today.AddDays(-1);
            var start = end;
            int count = 1;
            while (count < days)
            {
                start = start.AddDays(-1);
                if (start.DayOfWeek != DayOfWeek.Saturday && start.DayOfWeek != DayOfWeek.Sunday)
                    count++;
            }

            var range = SmartDateRange.Parse(input);
            Assert.Equal(start, range.Start);
            Assert.Equal(end, range.End);
        }

        [Fact]
        public void ParsesThisWeek()
        {
            var today = DateTime.Today;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            var start = today.AddDays(-diff).Date;

            var range = SmartDateRange.Parse("this week");
            Assert.Equal(start, range.Start);
            Assert.Equal(today, range.End);
        }

        [Fact]
        public void ParsesThisMonth()
        {
            var today = DateTime.Today;
            var start = new DateTime(today.Year, today.Month, 1);

            var range = SmartDateRange.Parse("this month");
            Assert.Equal(start, range.Start);
            Assert.Equal(today, range.End);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void ThrowsOnEmptyOrNullInput(string input)
        {
            Assert.Throws<NotSupportedException>(() => SmartDateRange.Parse(input));
        }

        [Theory]
        [InlineData("last -5 days")]
        [InlineData("last 0 business days")]
        public void ThrowsOnInvalidDayCount(string input)
        {
            Assert.Throws<NotSupportedException>(() => SmartDateRange.Parse(input));
        }

        [Fact]
        public void ThrowsOnUnsupportedExpression()
        {
            Assert.Throws<NotSupportedException>(() => SmartDateRange.Parse("next week"));
        }

        [Theory]
        [InlineData("today", true)]
        [InlineData("yesterday", true)]
        [InlineData("last 3 days", true)]
        [InlineData("last 3 business days", true)]
        [InlineData("this week", true)]
        [InlineData("this month", true)]
        [InlineData("next week", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void TryParseReturnsExpectedResult(string input, bool expectedResult)
        {
            var result = SmartDateRange.TryParse(input, out var range);
            Assert.Equal(expectedResult, result);
            if (expectedResult)
            {
                Assert.True(range.Start <= range.End);
            }
        }

        [Fact]
        public void TryParseHandlesNullInput()
        {
            var result = SmartDateRange.TryParse(null, out var range);
            Assert.False(result);
        }
    }
}
