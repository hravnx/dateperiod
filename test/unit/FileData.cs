using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Xunit.Sdk;

namespace DatePeriod.UnitTests;

public class FileDataAttribute : DataAttribute
{
    private readonly string _filePath;

    public FileDataAttribute(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        _filePath = filePath;
    }
    
    public override IEnumerable<object[]> GetData(MethodInfo method)
    {
        ArgumentNullException.ThrowIfNull(method);
        var contents = File.ReadAllText(_filePath);
        yield return new [] { (object)contents };
    }
}