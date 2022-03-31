using System;

using Xunit;

namespace DatePeriod.UnitTests;

public class LifecycleTests
{
    [Fact]
    public void Can_construct_date_period()
    {
        var period = new DatePeriod(new DateOnly(2022, 1, 1), new DateOnly(2022, 2, 1));
        Assert.Equal(new DateOnly(2022, 1, 1), period.StartOn);
        Assert.Equal(new DateOnly(2022, 2, 1), period.EndBefore);
        Assert.Equal(31, period.Length);
    }

    [Fact]
    public void Can_construct_empty_period()
    {
        var period = new DatePeriod(new DateOnly(2022, 1, 1), new DateOnly(2022, 1, 1));
        Assert.True(period.IsEmpty);
        Assert.Equal(0, period.Length);
    }

    [Fact]
    public void Cant_construct_period_with_end_before_start()
    {
        Assert.Throws<ArgumentOutOfRangeException>(()
            => new DatePeriod(new DateOnly(2022, 2, 1), new DateOnly(2022, 1, 1)));
    }

    [Fact]
    public void Can_make_a_one_day_period()
    {
        var period = DatePeriod.OneDay(new DateOnly(2022, 1, 1));
        Assert.Equal(new DateOnly(2022, 1, 1), period.StartOn);
        Assert.Equal(new DateOnly(2022, 1, 2), period.EndBefore);
        Assert.Equal(1, period.Length);
    }

    [Fact]
    public void Can_deconstruct_period()
    {
        var period = new DatePeriod(new DateOnly(2022, 1, 1), new DateOnly(2022, 2, 1));
        var (startOn, endBefore) = period;
        Assert.Equal(new DateOnly(2022, 1, 1), startOn);
        Assert.Equal(new DateOnly(2022, 2, 1), endBefore);
    }
}