using Xunit;

namespace DatePeriod.UnitTests;

public class ComparisonTests
{
    [Fact]
    public void CompareTo_DatePeriods_work()
    {
        var p1 = DatePeriod.Parse("2022-01-01/2022-02-01");
        var p2 = DatePeriod.Parse("2022-02-01/2022-03-01");
        DatePeriod? nullPeriod = null;
        Assert.Equal(-1, p1.CompareTo(p2));
        Assert.Equal(1, p1.CompareTo(nullPeriod));
        Assert.Equal(0, p1.CompareTo(p1));
    }

    [Fact]
    public void CompareTo_objects_work()
    {
        var p1 = DatePeriod.Parse("2022-01-01/2022-02-01");
        object p2 = DatePeriod.Parse("2022-02-01/2022-03-01");
        object? nullObj = null;
        Assert.Equal(-1, p1.CompareTo(p2));
        Assert.Equal(1, p1.CompareTo(nullObj));
        Assert.Equal(0, p1.CompareTo((object)p1));
    }

    [Fact]
    public void Comparison_operators_work()
    {
        var p1 = DatePeriod.Parse("2022-01-01/2022-02-01");
        var p12 = DatePeriod.Parse("2022-01-01/2022-02-01");
        var p2 = DatePeriod.Parse("2022-02-01/2022-03-01");
        var p22 = DatePeriod.Parse("2022-02-01/2022-03-01");
        Assert.True(p1 < p2);
        Assert.False(p2 < p1);
        
        Assert.True(p2 > p1);
        Assert.False(p1 > p2);
        
        Assert.True(p1 >= p12);
        Assert.True(p2 <= p22);
    }
}