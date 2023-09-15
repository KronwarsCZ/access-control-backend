using ApiShared.Dto.Owners;
using Core.Entities.Owners;
using Framework.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [AllowAnonymous]
    [Route("registerNewOwner")]
    public async Task<ActionResult<RegisterNewOwnerResponse>> RegisterNewOwner([FromBody] RegisterNewOwnerParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(!string.IsNullOrWhiteSpace(parameters.Name), $"Invalid {nameof(parameters.Name)}");
        var owner = Owner.Create(parameters.Name);
        
        
        // uprav dole:
        await ownerService.RegisterNewOwnerAndAccountAsync(owner, parameters.Email, parameters.Password, cancellationToken);
        
        ;
        await accountService.SaveChangesAsync(cancellationToken);

        var registerNewOwnerResponse = new RegisterNewOwnerResponse
        {
            OwnerId = owner.Id
        };

        return Ok(registerNewOwnerResponse);
    }

}
