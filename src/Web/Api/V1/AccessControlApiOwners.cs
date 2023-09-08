using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("registerNewOwner")]
    public async Task<ActionResult<Dto.RegisterNewOwnerResponse>> RegisterNewOwner([FromBody] Dto.RegisterNewOwnerParameters parameters, CancellationToken cancellationToken)
    {
        var owner = Owner.Create(parameters.Name);
        await ownerService.RegisterNewOwnerAsync(owner, cancellationToken);
        return Ok(new Dto.RegisterNewOwnerResponse
        {
            Id = owner.Id
        });
    }

}
