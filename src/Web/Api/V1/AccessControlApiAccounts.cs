using ApiShared.Dto.Accounts;
using ApiShared.Dto.Auth;
using Core.Entities.Accounts;
using Framework.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("account/changePassword")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters, "Payload cannot be null");
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);

        await accountService.ChangePasswordAsync(auth.Account, parameters.CurrentPassword, parameters.NewPassword, cancellationToken);

        return Ok();
    }

    [HttpPost]
    [Route("accounts/create")]
    public async Task<ActionResult<CreateAccountResult>> CreateAccount([FromBody] CreateAccountParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters, "Payload cannot be null");
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);
        RequireAdmin(auth);

        Check.Contract(!string.IsNullOrWhiteSpace(parameters.Email), "Email cannot be empty");
        Check.Contract(!string.IsNullOrWhiteSpace(parameters.Password), "Password cannot be empty");

        var role = parameters.IsAdmin ? AccountRole.Admin : AccountRole.User;
        await accountService.AddAccountUnderOwner(auth.Owner, parameters.Email, parameters.Password, cancellationToken, role);
        await accountService.SaveChangesAsync(cancellationToken);

        var createdAccount = await accountService.GetAccountByEmailAsync(parameters.Email, cancellationToken);
        Check.Contract(createdAccount, "Failed to retrieve created account");

        return Ok(new CreateAccountResult
        {
            AccountId = createdAccount!.Id,
            Email = createdAccount.Email,
            Role = createdAccount.Role.ToString()
        });
    }

    [HttpPost]
    [Route("accounts/getAll")]
    public async Task<ActionResult<GetAllAccountsResult>> GetAllAccounts(CancellationToken cancellationToken)
    {
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);
        RequireAdmin(auth);

        var accounts = await accountService.GetAccountsByOwnerIdAsync(auth.Owner.Id, cancellationToken);

        return Ok(new GetAllAccountsResult
        {
            Accounts = accounts.Select(a => new AccountResult
            {
                Id = a.Id,
                Email = a.Email,
                Role = a.Role.ToString(),
                CreatedUtc = a.CreatedUtc
            })
        });
    }

    [HttpPost]
    [Route("accounts/delete")]
    public async Task<ActionResult> DeleteAccount([FromBody] DeleteAccountParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters, "Payload cannot be null");
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);
        RequireAdmin(auth);

        Check.Contract(parameters.AccountId != Guid.Empty, "Invalid AccountId");
        Check.Contract(parameters.AccountId != auth.Account.Id, "Cannot delete your own account");

        var account = await accountService.GetByIdAsync(parameters.AccountId, cancellationToken);
        Check.Contract(account, "Account not found");
        Check.Contract(account!.OwnerId == auth.Owner.Id, "Invalid account for this owner");

        await accountService.DeleteAccountAsync(account, cancellationToken);

        return NoContent();
    }
}
