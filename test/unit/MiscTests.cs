using System;
using System.Linq;

using Xunit;

namespace DatePeriod.UnitTests;

public class MiscTests
{
    [Theory, 
     InlineData("2022-01-01/2022-02-01", 31),
     InlineData("2022-01-01/2022-03-01", 31+28)]
    public void Length_returns_the_correct_number_of_days(string input, int expected)
    {
        var period = DatePeriod.Parse(input);
        Assert.Equal(expected, period.Length);
    }

    [Theory, 
     InlineData("2022-01-01/2022-02-01", 31),
     InlineData("2022-01-01/2022-03-01", 31+28)]
    public void Duration_returns_the_correct_timespan(string input, int expectedDays)
    {
        var period = DatePeriod.Parse(input);
        var expected = TimeSpan.FromDays(expectedDays);
        Assert.Equal(expected, period.Duration);
    }

    [Fact]
    public void AllDays_return_a_range_of_days()
    {
        var period = DatePeriod.Parse("2022-01-01/2022-02-01");
        var count = 0;
        var prev = new DateOnly(2021, 12, 31);

        foreach (var date in period.AllDays())
        {
            var diff = date.DayNumber - prev.DayNumber;
            Assert.Equal(1, diff);
            ++count;
            prev = date;
        }
        
        Assert.Equal(31, count);
    }

    [Fact]
    public void AllDays_return_an_empty_sequence_if_called_on_an_empty_period()
    {
        var period = DatePeriod.Parse("2022-01-01/2022-01-01");
        Assert.Empty(period.AllDays());
    }


    [Theory]
    [InlineData("2022-01-01/2022-02-01", "2022-01-01/2022-02-01", true)]
    [InlineData("2022-01-01/2022-02-01", "2022-01-12/2022-02-01", true)]
    [InlineData("2022-01-01/2022-03-01", "2022-01-12/2022-02-01", true)]
    [InlineData("2022-01-01/2022-02-01", "2022-02-01/2022-03-01", false)]
    public void OverlapsWith_works(string a, string b, bool expected)
    {
        var p1 = DatePeriod.Parse(a);
        var p2 = DatePeriod.Parse(b);
        Assert.Equal(expected, p1.OverlapsWith(p2));
        Assert.Equal(expected, p2.OverlapsWith(p1));
    }

    [Theory]
    [InlineData("2022-01-01/2022-02-01", "2022-01-01/2022-02-01", "2022-01-01/2022-02-01")]
    [InlineData("2022-01-01/2022-02-01", "2022-01-12/2022-02-01", "2022-01-01/2022-02-01")]
    [InlineData("2022-01-01/2022-03-01", "2022-01-12/2022-02-01", "2022-01-01/2022-03-01")]
    [InlineData("2022-01-01/2022-02-01", "2022-02-01/2022-03-01", "2022-01-01/2022-03-01")]
    [InlineData("2022-01-01/2022-02-01", "2023-01-01/2023-01-01", "2022-01-01/2022-02-01")]
    public void UnionWith_works(string a, string b, string expected)
    {
        var p1 = DatePeriod.Parse(a);
        var p2 = DatePeriod.Parse(b);
        var expectedPeriod = DatePeriod.Parse(expected);
        Assert.Equal(expectedPeriod, p1.UnionWith(p2));
        Assert.Equal(expectedPeriod, p2.UnionWith(p1));
    }

    [Theory]
    [InlineData("2022-01-01/2022-02-01", "2022-01-01/2022-02-01", "2022-01-01/2022-02-01")]
    [InlineData("2022-01-01/2022-02-01", "2022-01-12/2022-02-01", "2022-01-12/2022-02-01")]
    [InlineData("2022-01-01/2022-03-01", "2022-01-12/2022-02-01", "2022-01-12/2022-02-01")]
    [InlineData("2022-01-01/2022-02-01", "2022-02-01/2022-03-01", "<empty>")]
    [InlineData("2022-01-01/2022-02-01", "2023-01-01/2023-01-01", "<empty>")]
    public void IntersectionWith(string a, string b, string expected)
    {
        var p1 = DatePeriod.Parse(a);
        var p2 = DatePeriod.Parse(b);
        if (expected == "<empty>")
        {
            Assert.True(p1.IntersectionWith(p2).IsEmpty);
            Assert.True(p2.IntersectionWith(p1).IsEmpty);
        }
        else
        {
            var expectedPeriod = DatePeriod.Parse(expected);
            Assert.Equal(expectedPeriod, p1.IntersectionWith(p2));
            Assert.Equal(expectedPeriod, p2.IntersectionWith(p1));
        }
    }
    
    
}