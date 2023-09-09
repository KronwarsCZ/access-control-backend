using AccessControl.Web.Api.V1.Dto.Owners;
using Core.Entities;
using Core.Entities.Owners;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("registerNewOwner")]
    public async Task<ActionResult<RegisterNewOwnerResponse>> RegisterNewOwner([FromBody] RegisterNewOwnerParameters parameters, CancellationToken cancellationToken)
    {
        var owner = Owner.Create(parameters.Name);
        await ownerService.RegisterNewOwnerAsync(owner, cancellationToken);

        var registerNewOwnerResponse = new RegisterNewOwnerResponse
        {
            Id = owner.Id
        };
        return Ok(registerNewOwnerResponse);
    }

}
