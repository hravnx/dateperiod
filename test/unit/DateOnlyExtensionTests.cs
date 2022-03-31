using System;

using Xunit;

namespace DatePeriod.UnitTests;

public class DateOnlyExtensionTests
{
    [Fact]
    public void ToISOString_works()
    {
        var p1 = new DateOnly(2022, 1, 1);
        Assert.Equal("2022-01-01", p1.ToISOString());
    }
    
    [Fact]
    public void MinMax_works()
    {
        var p1 = new DateOnly(2022, 1, 1);
        var p2 = new DateOnly(2022, 1, 2);
        Assert.Equal(new DateOnly(2022, 1, 1), p1.Min(p2));
        Assert.Equal(new DateOnly(2022, 1, 2), p1.Max(p2));
    }
}