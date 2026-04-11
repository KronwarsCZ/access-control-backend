using Application.Services.Accounts;
using Core.Entities.Accounts;
using Core.Entities.Owners;
using Framework.Extensions;

namespace Application.Services.Owners;

public class OwnerService : BasicService.BasicService, IOwnerService
{
    private IAccountService accountService;
    public OwnerService(IUnitOfWork.IUnitOfWork unitOfWork, IAccountService accountService)
        : base(unitOfWork)
    {
        this.accountService = accountService;
    }
    
    public async Task<Owner> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await UnitOfWork.OwnerRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task RegisterNewOwnerAndAccountAsync(Owner owner, string email, string password, CancellationToken cancellationToken)
    {
        UnitOfWork.OwnerRepository.Add(owner.ToEnumerable());
        if (owner.Accounts.IsNotNull() && owner.Accounts.Any()) return;
        await accountService.AddAccountUnderOwner(owner, email, password, cancellationToken, AccountRole.Owner);
    }
}
