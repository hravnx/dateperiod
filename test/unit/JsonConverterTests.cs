using System.Text.Json;
using System.Text.Json.Serialization;

using Xunit;

namespace DatePeriod.UnitTests;

public class JsonConverterTests
{
    private readonly JsonSerializerOptions _options;

    public JsonConverterTests()
    {
        _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
    
    [Theory, FileData("period-test-data.json")]
    public void Can_deserialize_object_containing_date_period(string json)
    {
        var container = JsonSerializer.Deserialize<DatePeriodTestContainer>(json, _options); 
        Assert.NotNull(container);
        Assert.Equal(1, container!.Id);
        Assert.Equal(DatePeriod.Parse("2022-03-01/2022-03-02"), container.Period);
        Assert.Equal("Hello", container.Name);
    }

    [Fact]
    public void Can_serialize_object_containing_date_period()
    {
        var expected = new DatePeriodTestContainer
        {
            Id = 2,
            Period = DatePeriod.Parse("2022-03-01/2022-03-02"),
            Name = "Well"
        };
        var json = JsonSerializer.Serialize(expected, _options);
        var actual = JsonSerializer.Deserialize<DatePeriodTestContainer>(json, _options)!;
        Assert.Equal(expected.Period, actual.Period);
    }

    [Theory]
    [FileData("unexpected-format-01.json")]
    [FileData("unexpected-format-02.json")]
    [FileData("unexpected-format-03.json")]
    public void Throws_when_given_unexpected_json(string json)
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<DatePeriodTestContainer>(json, _options));
    }
}


internal class DatePeriodTestContainer
{
    public int Id { get; set; }

    [JsonConverter(typeof(DatePeriodJsonConverter))]
    public DatePeriod Period { get; set; } = null!;

    public string Name { get; set; } = null!;
}