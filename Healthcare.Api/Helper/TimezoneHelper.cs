namespace Healthcare.Api.Helper
{
    public class TimezoneHelper
    {
        public static TimeZoneInfo GetDoctorTz(string tz)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(tz);
        }

        public static DateTime ToUtc(DateTime local, TimeZoneInfo tz)
        {
            return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(local, DateTimeKind.Unspecified), tz);
        }

        public static DateTime ToLocal(DateTime utc, TimeZoneInfo tz)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), tz);
        }
    }
}