using AccessControl.Web.Api.V1.Dto.Owners;
using Core.Entities.Owners;
using Framework.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("registerNewOwner")]
    public async Task<ActionResult<RegisterNewOwnerResponse>> RegisterNewOwner([FromBody] RegisterNewOwnerParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(!string.IsNullOrEmpty(parameters.Name), $"Invalid {nameof(parameters.Name)}");
        var owner = Owner.Create(parameters.Name);
        await ownerService.RegisterNewOwnerAsync(owner, cancellationToken);
        await accountService.AddAccountUnderOwner(owner, parameters.Email, parameters.Password, cancellationToken);
        await accountService.SaveChangesAsync(cancellationToken);

        var registerNewOwnerResponse = new RegisterNewOwnerResponse
        {
            OwnerId = owner.Id
        };

        return Ok(registerNewOwnerResponse);
    }

}
