using System.Globalization;


namespace DatePeriod;

/// <summary>
/// Encapsulates the concept of a date period, i.e. a continuous sequence of
/// dates, starting at StartOn and ending the day before EndDate.
/// <example>
///     var period = DatePeriod.Parse("2022-01-01/2022-02-01");
///     var (startOn, endBefore) = period;
///     Debug.Assert(startOn == new DateOnly(2022,1,1));
///     Debug.Assert(endBefore == new DateOnly(2022,2,1));
///     Debug.Assert(period.Duration.TotalDays == 31);
///     var allDaysInJanuary = period.AllDays().ToList();
///     Debug.Assert(allDaysInJanuary.Count == 31);
/// </example>
/// </summary>
public sealed class DatePeriod : IComparable<DatePeriod>, IComparable, IEquatable<DatePeriod>
{
    // The empty period
    private static readonly DatePeriod Empty = new(DateOnly.MinValue, DateOnly.MinValue);

    /// <summary>
    /// The date the period starts
    /// </summary>
    public DateOnly StartOn { get; }

    /// <summary>
    /// The date the period ended, non-inclusive
    /// </summary>
    public DateOnly EndBefore { get; }

    #region Lifecycle

    /// <summary>
    /// Creates a new instance of a <c>DatePeriod</c>, based on a start and end date
    /// </summary>
    /// <param name="startOn">The date the period starts on</param>
    /// <param name="endBefore">The date the period ended, non-inclusive</param>
    /// <exception cref="ArgumentOutOfRangeException">If <c>endBefore</c> is before <c>startOn</c></exception>
    public DatePeriod(DateOnly startOn, DateOnly endBefore)
    {
        if (endBefore < startOn)
        {
            throw new ArgumentOutOfRangeException(nameof(endBefore),
                string.Format(
                    CultureInfo.InvariantCulture,
                    "startOn `{0:yyyy-MM-dd}` must not be later than endBefore `{1:yyyy-MM-dd}`",
                    startOn,
                    endBefore));
        }

        StartOn = startOn;
        EndBefore = endBefore;
    }

    /// <summary>
    /// Deconstructs a <c>DatePeriod</c> to a pair of <c>DateOnly</c> instances
    /// </summary>
    /// <param name="startOn">The date the period startsOn</param>
    /// <param name="endBefore">The date the period ended, non-inclusive</param>
    public void Deconstruct(out DateOnly startOn, out DateOnly endBefore)
    {
        startOn = StartOn;
        endBefore = EndBefore;
    }

    /// <summary>
    /// Creates a singleton <c>DatePeriod</c> instance, i.e. a single day 
    /// </summary>
    /// <param name="startOn">The date of the singleton period</param>
    /// <returns></returns>
    public static DatePeriod OneDay(DateOnly startOn)
        => new(startOn, startOn.AddDays(1));

    #endregion

    #region To and from strings

    /// <summary>
    /// Returns an ISO 8601 string representation of this <c>DatePeriod</c> instance
    /// </summary>
    /// <returns>The string in ISO 8601 format</returns>
    public override string ToString()
        => string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd}/{1:yyyy-MM-dd}", StartOn, EndBefore);

    /// <summary>
    /// Parse a string in ISO 8601 period format into a new <c>DatePeriod</c> instance
    /// </summary>
    /// <param name="input">The string to parse</param>
    /// <returns>The new <c>DatePeriod</c> instance</returns>
    /// <exception cref="ArgumentNullException">If <c>input</c> is null</exception>
    /// <exception cref="FormatException">If <c>input</c> is not in the correct format</exception>
    /// <remarks>Note that only a very limited form of ISO 8601 periods are supported,
    /// namely yyyy</remarks>
    public static DatePeriod Parse(string? input)
    {
        ArgumentNullException.ThrowIfNull(input);
        if (input.Length != 21 || input[10] != '/')
        {
            throw new FormatException("Input must be in the ISO 8601 period format `yyyy-MM-dd/yyyy-MM-dd`");
        }

        var startOn = DateOnly.ParseExact(input[..10], "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var endBefore = DateOnly.ParseExact(input[11..], "yyyy-MM-dd", CultureInfo.InvariantCulture);
        return new DatePeriod(startOn, endBefore);
    }

    /// <summary>
    /// Try to parse a string in ISO 8601 period format into a new <c>DatePeriod</c> instance
    /// </summary>
    /// <param name="input">The string to parse</param>
    /// <param name="period">The resulting <c>DatePeriod</c> instance</param>
    /// <returns><c>True</c> if parsing succeeded, <c>false</c> otherwise</returns>
    public static bool TryParse(string? input, out DatePeriod period)
    {
        period = Empty;
        if (input is null || input.Length != 21 || input[10] != '/') return false;
        try
        {
            var startOn = DateOnly.ParseExact(input.Substring(0, 10), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var endBefore = DateOnly.ParseExact(input.Substring(11), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            period = new DatePeriod(startOn, endBefore);
            return true;
        }
        catch (Exception)
        {
            return false;            
        }
    }

    #endregion

    #region Misc

    /// <summary>
    /// The number of days in the period
    /// </summary>
    public int Length => EndBefore.DayNumber - StartOn.DayNumber;

    /// <summary>
    /// The duration of the period
    /// </summary>
    public TimeSpan Duration => TimeSpan.FromDays(Length);

    /// <summary>
    /// Returns all dates in the period in order, lazily evaluated 
    /// </summary>
    /// <returns>The sequence of dates</returns>
    public IEnumerable<DateOnly> AllDays()
    {
        var current = StartOn;
        while (current < EndBefore)
        {
            yield return current;
            current = current.AddDays(1);
        }
    }

    /// <summary>
    /// Tests if two periods overlap
    /// </summary>
    /// <param name="other">The other period</param>
    /// <returns><c>True</c> if the periods overlap, <c>false</c> otherwise</returns>
    public bool OverlapsWith(DatePeriod other)
        => StartOn < other.EndBefore && EndBefore > other.StartOn;

    /// <summary>
    /// Compute the union of two periods
    /// </summary>
    /// <param name="other">The other period</param>
    /// <returns>The union of the two periods</returns>
    public DatePeriod UnionWith(DatePeriod other)
    {
        if (other.IsEmpty) return this;
        return IsEmpty 
            ? other 
            : new DatePeriod(StartOn.Min(other.StartOn), EndBefore.Max(other.EndBefore));
    }

    /// <summary>
    /// Compute the intersection of two periods
    /// </summary>
    /// <param name="other">The other period</param>
    /// <returns>A new <c>DatePeriod</c> that contains the intersection of the periods.
    /// If there is no overlap between the periods, an empty period is returned.</returns>
    public DatePeriod IntersectionWith(DatePeriod other)
        => OverlapsWith(other) ? new DatePeriod(StartOn.Max(other.StartOn), EndBefore.Min(other.EndBefore)) : Empty;

    #endregion

    #region Equality

    /// <summary>
    /// <c>True</c> if the periods spans zero days, <c>false</c> otherwise
    /// </summary>
    public bool IsEmpty => StartOn == EndBefore;

    public bool Equals(DatePeriod? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return StartOn.Equals(other.StartOn) && EndBefore.Equals(other.EndBefore);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((DatePeriod)obj);
    }

    public override int GetHashCode() => HashCode.Combine(StartOn, EndBefore);

    public static bool operator ==(DatePeriod? left, DatePeriod? right) => Equals(left, right);

    public static bool operator !=(DatePeriod? left, DatePeriod? right) => !Equals(left, right);

    #endregion

    #region Comparison

    public int CompareTo(DatePeriod? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        var startOnComparison = StartOn.CompareTo(other.StartOn);
        return startOnComparison != 0
            ? startOnComparison
            : EndBefore.CompareTo(other.EndBefore);
    }

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        if (ReferenceEquals(this, obj)) return 0;
        return obj is DatePeriod other
            ? CompareTo(other)
            : throw new ArgumentException($"Object must be of type {nameof(DatePeriod)}");
    }

    public static bool operator <(DatePeriod? left, DatePeriod? right)
        => Comparer<DatePeriod>.Default.Compare(left, right) < 0;

    public static bool operator >(DatePeriod? left, DatePeriod? right)
        => Comparer<DatePeriod>.Default.Compare(left, right) > 0;

    public static bool operator <=(DatePeriod? left, DatePeriod? right)
        => Comparer<DatePeriod>.Default.Compare(left, right) <= 0;

    public static bool operator >=(DatePeriod? left, DatePeriod? right)
        => Comparer<DatePeriod>.Default.Compare(left, right) >= 0;

    #endregion
}