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
        var keys = await keyService.GetAllAsync(cancellationToken);

		return Ok(keys);
	}
    
    // get all keys that can open a device with given device id
    [HttpGet]
    [Route("keys/getKeysForDevice")]
    public async Task<ActionResult<IEnumerable<Key>>> GetKeysForDeviceAsync(Guid id, CancellationToken cancellationToken)
    {
        var keys = await keyService.GetKeysForDeviceAsync(id, cancellationToken);

        return Ok(keys);
    }

    [HttpDelete]
    [Route("keys/deleteKeys")]
    public async Task<ActionResult> DeleteKeysAsync([FromBody] KeyDeleteParameters keyDeleteParameters, CancellationToken cancellationToken)
    {
        var allIds = keyDeleteParameters.Ids;
        var keys = await keyService.GetAllAsync(cancellationToken);
        var keysToDelete = keys.Where(k => allIds.Contains(k.Id)).ToImmutableArray();

        keyService.Delete(keysToDelete, cancellationToken);

        return Ok();
    }

    [HttpPut]
    [Route("keys/updateKey")]
    public async Task<ActionResult> UpdateKeyAsync([FromBody] KeyUpdateParameters keyUpdateParams, CancellationToken cancellationToken)
    {
        var allIds = keyUpdateParams.Keys.Select(k => k.Id);
        var keys = await keyService.GetAllAsync(cancellationToken);
        var selectedKeys = keys.Where(k => allIds.Contains(k.Id)).ToImmutableArray();

        foreach (var key in selectedKeys)
        {
            key.Name = "updated name";
        }
        
        await keyService.Update(selectedKeys, cancellationToken);

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