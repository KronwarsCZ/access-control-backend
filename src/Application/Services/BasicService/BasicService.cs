namespace Application.Services;

public class BasicService : IBasicService
{
    protected readonly IUnitOfWork.IUnitOfWork UnitOfWork;

    protected BasicService(IUnitOfWork.IUnitOfWork unitOfWork)
    {
        this.UnitOfWork = unitOfWork;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
