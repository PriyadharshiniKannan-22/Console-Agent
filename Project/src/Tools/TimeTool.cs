using TimeZoneConverter;

namespace ToolForge.Tools;

public class TimeTool
{
    public string GetTime(string timezone)
    {
        try
        {
            var tz = TZConvert.GetTimeZoneInfo(timezone);
            var now = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                tz
            );

            return $"{timezone}: {now:yyyy-MM-dd HH:mm:ss}";
        }
        catch
        {
            return $"Invalid timezone: {timezone}";
        }
    }
}