using System;
using Xunit;
using SmartDateRangeParser;

namespace SmartDateRangeParser.Tests
{
    public class SmartDateRangeTests
    {
        [Fact]
        public void ParsesTodayCorrectly()
        {
            var range = SmartDateRange.Parse("today");
            Assert.Equal(DateTime.Today, range.Start);
            Assert.Equal(DateTime.Today, range.End);
        }
    }
}
