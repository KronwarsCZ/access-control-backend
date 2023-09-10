using Core.Entities.Devices;
using Framework.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpGet]
    [Route("contractException")]
    public async Task<ActionResult<IEnumerable<Device>>> ContractException(CancellationToken cancellationToken)
    {
        Check.Contract(false, "tenhle text byl poslany do konstruktoru ContractException, ktera byla thrownuta");
        return Ok("uspesna response");
    }
}
