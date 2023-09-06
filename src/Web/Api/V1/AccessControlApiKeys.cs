using System.Collections.Immutable;
using AccessControl.Web.Api.V1.Dto.Keys;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    // get all keys
    [HttpGet]
	[Route("keys/getAll")]
	public async Task<ActionResult<IEnumerable<Key>>> KeysGetAllAsync(CancellationToken cancellationToken)
    {
        return Ok();
    }
    

    [HttpDelete]
    [Route("keys/deleteKeys")]
    public async Task<ActionResult> DeleteKeysAsync([FromBody] KeyDeleteParameters keyDeleteParameters, CancellationToken cancellationToken)
    {
        return Ok();

    }

    [HttpPut]
    [Route("keys/updateKey")]
    public async Task<ActionResult> UpdateKeyAsync([FromBody] KeyUpdateParameters keyUpdateParams, CancellationToken cancellationToken)
    {
        return Ok();

    }

    [HttpPost]
    [Route("keys/addKeys")]
    public async Task<ActionResult<IEnumerable<Guid>>> AddKeys([FromBody] KeyAddParameters keysParameters,
        CancellationToken cancellationToken)
    {

        return Ok();
    }
}