using Xunit;

namespace DatePeriod.UnitTests;

public class EqualityTests
{
    [Fact]
    public void Equality_can_be_determined_for_objects()
    {
        var period = DatePeriod.Parse("2022-01-01/2022-01-02");
        Assert.True(period.Equals(period));
        
        object p2 = DatePeriod.Parse("2022-01-01/2022-01-02");
        Assert.True(period.Equals(p2));
        
        object p3 = DatePeriod.Parse("2022-01-01/2022-02-01");
        Assert.False(period.Equals(p3));
    }
    
    [Fact]
    public void Null_is_not_equal_to_an_instance()
    {
        var period = DatePeriod.Parse("2022-01-01/2022-01-02");
        object? nullObj = null; 
        Assert.False(period.Equals(null));
        Assert.False(period!.Equals(nullObj));
    }

    [Fact]
    public void An_instance_is_equal_to_itself()
    {
        var period = DatePeriod.Parse("2022-01-01/2022-01-02");
        Assert.True(period.Equals(period));
        
        var periodAlias = (object)period;
        Assert.True(period.Equals(periodAlias));
    }

    [Fact]
    public void Equality_operators_work()
    {
        var period = DatePeriod.Parse("2022-01-01/2022-01-02");
        Assert.True(period.Equals(period));
        
        var p2 = DatePeriod.Parse("2022-01-01/2022-01-02");
        Assert.True(period == p2);
        Assert.False(period != p2);
        
        var p3 = DatePeriod.Parse("2022-01-01/2022-02-01");
        Assert.False(period == p3);
        Assert.True(period != p3);
    }

    [Fact]
    public void GetHashCode_works()
    {
        var period = DatePeriod.Parse("2022-01-01/2022-01-02");
        var p1 = DatePeriod.Parse("2022-01-01/2022-01-02");
        var p2 = DatePeriod.Parse("2022-01-01/2022-02-01"); 
        Assert.Equal(period.GetHashCode(), p1.GetHashCode());
        Assert.NotEqual(period.GetHashCode(), p2.GetHashCode());
    }
}