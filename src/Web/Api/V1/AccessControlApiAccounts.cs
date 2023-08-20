using AccessControl.Web.Api.V1.Dto.Accounts;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("accounts/getAll")]
    public async Task<ActionResult<IEnumerable<Device>>> AccountsGetAll([FromBody] AccountFilterParameters parameters, CancellationToken cancellationToken)
    {

        // var devices = await accountCommandRepository.GetAccountByEmailAsync(parameters.Email, cancellationToken);

        return Ok();
    }

    [HttpPost]
    [Route("accounts/add")]
    public async Task<ActionResult<IEnumerable<Guid>>> AccountsAddAsync([FromBody] AccountAddParameters parameters,
        CancellationToken cancellationToken)
    {
        
        return Ok();
    }
}
