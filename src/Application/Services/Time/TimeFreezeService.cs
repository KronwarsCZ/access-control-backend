namespace Application.Services.Time;

public class TimeFreezeService : ITimeService
{
    public TimeFreezeService(ITimeService timeService)
    {
        _time = timeService.GetUtcTime();
    }

    public TimeFreezeService(DateTime utcTime)
    {
        _time =
            utcTime.Kind is DateTimeKind.Utc
            ? utcTime
            : throw new ArgumentException($"Time must be UTC, {nameof(utcTime)}");
    }

    private DateTime _time;

    public DateTime GetUtcTime() => _time;
}
