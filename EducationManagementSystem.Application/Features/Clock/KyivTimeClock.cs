namespace EducationManagementSystem.Application.Features.Clock;

public class KyivTimeClock : IClock
{
    private readonly TimeZoneInfo _kyivTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kiev");

    public DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _kyivTimeZone);
    public DateTime Today => Now.Date;
}