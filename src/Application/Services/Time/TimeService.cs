namespace Application.Services;

public class CurrentUtc : ITimeService
{
    public DateTime GetUtcTime() => DateTime.UtcNow;
}