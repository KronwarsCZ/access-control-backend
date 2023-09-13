namespace Application.Services.BasicService;

public interface IBasicService
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
