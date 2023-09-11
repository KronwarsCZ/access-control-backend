namespace Application.Services;

public interface IBasicService
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
