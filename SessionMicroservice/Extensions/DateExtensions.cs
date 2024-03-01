namespace SessionMicroservice.Extensions;

public static class DateExtensions
{
    // Unix Epoch Time
    public static long ToUnixEpochTime(this DateTime date)
    {
        var timeSpan = date - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return (long)timeSpan.TotalSeconds;
    }
    
    public static DateTime FromUnixEpochTime(this long unixTime)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTime);
    }
}