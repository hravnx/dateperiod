# DatePeriod - helper class for DateOnly periods

This project is a single-purpose library that contains a C# class `DatePeriod`
that can help you deal with periods of `DateOnly` end-points.

Since it uses the `DateOnly` BCL type, this library is .Net 6+ only.

### Example 1 - basic usage
You can make a `DatePeriod` instance 

```csharp
using System;
using System.Diagnostics;
using DatePeriod;

var period = new DatePeriod(new DateOnly(2022,1,1), new DateOnly(2022,2,1));

Debug.Assert(period.StartOn   == new DateOnly(2022,1,1));
Debug.Assert(period.EndBefore == new DateOnly(2022,2,1));
Debug.Assert(period.Length    == 31);
Debug.Assert(period.Duration  == TimeSpan.FromDays(31));

foreach(var date in period.AllDays()) 
{
    Console.WriteLine("Date is {0:yyyy-MM-dd}", date);
    // Outputs "Date is 2022-01-01" etc 
}
```

### Example 2 - Parsing and json serialization
The library can parse a string in a subset of the ISO 8601 period format.
The library also includes a custom `JsonConverter` implementation for `DatePeriod`,
use it like this example.

```csharp
using System;
using System.Diagnostics;
using System.Text.Json
using DatePeriod;

public class Data
{
    [JsonConverter(typeof(DatePeriodJsonConverter))]
    public DatePeriod Period { get; set; }
}
// parse from a string, the format is from the ISO 8601 standard
var orgData = DatePeriod.Parse("2022-01-01/2022-02-01");
// serialize to json
var asJson = JsonSerializer.Serialize(orgData);
// deserialize from json
var dataFromJson = JsonSerializer.Deserialize<Data>(asJson);
Debug.Assert(orgData.Period == dataFromJson.Period);
```

See the unit tests for more examples.
