namespace Application.Services.Time;

public class CurrentUtc : ITimeService
{
    public DateTime GetUtcTime() => DateTime.UtcNow;
}