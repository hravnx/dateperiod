using System;

using Xunit;

namespace DatePeriod.UnitTests;

public class ToAndFromStrings
{
    [Theory]
    [InlineData("2022-01-01/2022-02-01")]
    [InlineData("2022-01-01/2022-03-14")]
    public void ToString_converts_to_iso_period_format(string input)
    {
        var period = DatePeriod.Parse(input);
        Assert.Equal(input, period.ToString());
    }

    [Fact]
    public void Can_parse_a_period_string()
    {
        var period = DatePeriod.Parse("2022-01-01/2022-02-01");
        Assert.Equal(new DatePeriod(new DateOnly(2022,1,1), new DateOnly(2022,2,1)), period);
    }

    [Fact]
    public void TryParse_returns_true_if_parsing_succeeds()
    {
        var actual = DatePeriod.TryParse("2022-01-01/2022-02-01", out var result);
        Assert.True(actual);
        Assert.Equal(new DatePeriod(new DateOnly(2022,1,1), new DateOnly(2022,2,1)), result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("2022-01-02/2022-01-01")]
    [InlineData("2022-01-01-2022-01-02")]
    public void Parse_throws_exception_if_given_invalid_input(string input)
    {
        Assert.ThrowsAny<Exception>(() => DatePeriod.Parse(input));
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("2022-01-02/2022-01-01")]
    [InlineData("2022-01-01-2022-01-02")]
    public void TryParse_returns_false_if_parsing_fails(string input)
    {
        var actual = DatePeriod.TryParse(input, out var result);
        Assert.False(actual);
        Assert.True(result.IsEmpty);
    }
}