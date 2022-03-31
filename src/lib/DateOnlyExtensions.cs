using System.Globalization;

namespace DatePeriod;

public static class DateOnlyExtensions
{
    /// <summary>
    /// Return the earliest of two dates
    /// </summary>
    /// <param name="lhs">The first date</param>
    /// <param name="rhs">The second date</param>
    /// <returns>The earliest date</returns>
    public static DateOnly Min(this DateOnly lhs, DateOnly rhs)
        => lhs < rhs ? lhs : rhs;

    /// <summary>
    /// Return the latest of two dates
    /// </summary>
    /// <param name="lhs">The first date</param>
    /// <param name="rhs">The second date</param>
    /// <returns>The latest date</returns>
    public static DateOnly Max(this DateOnly lhs, DateOnly rhs)
        => lhs > rhs ? lhs : rhs;
    
    /// <summary>
    /// Formats a <c>DateOnly</c> instance as a string in ISO 8601 format
    /// </summary>
    /// <param name="date">The date to format</param>
    /// <returns>The formatted string</returns>
    // ReSharper disable once InconsistentNaming
    public static string ToISOString(this DateOnly date)
        => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    
}