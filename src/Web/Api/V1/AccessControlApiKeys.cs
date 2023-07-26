using Application.Services;
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
        var keys = await _keyService.GetAllAsync(cancellationToken);

		return Ok(keys);
	}
    
    // get all keys that can open a device with given device id
    [HttpGet]
    [Route("keys/getKeysForDevice")]
    public async Task<ActionResult<IEnumerable<Key>>> GetKeysForDeviceAsync(Guid id, CancellationToken cancellationToken)
    {
        var keys = await _keyService.GetKeysForDeviceAsync(id, cancellationToken);

        return Ok(keys);
    }

    [HttpDelete]
    [Route("keys/deleteKeys")]
    public async Task<ActionResult> DeleteKeysAsync([FromBody] Dto.KeyDeleteParameters keyDeleteParameters, CancellationToken cancellationToken)
    {
        var allIds = keyDeleteParameters.Ids;
        var keys = await _keyService.GetAllAsync(cancellationToken);
        var keysToDelete = keys.Where(k => allIds.Contains(k.Id));

        await _keyService.Delete(keysToDelete, cancellationToken);

        return Ok();
    }

    [HttpPut]
    [Route("keys/updateKey")]
    public async Task<ActionResult<Key>> UpdateKeyAsync([FromBody] Dto.KeyUpdateParameters keyUpdateParams, CancellationToken cancellationToken)
    {
        var allIds = keyUpdateParams.Keys.Select(k => k.Id);
        var keys = await _keyService.GetAllAsync(cancellationToken);
        var selectedKeys = keys.Where(k => allIds.Contains(k.Id));

        foreach (var key in selectedKeys)
        {
            key.Name = "updated name";
        }
        
        await _keyService.Update(selectedKeys, cancellationToken);

        return Ok();
    }

    [HttpPost]
    [Route("keys/addKeys")]
    public async Task<ActionResult<IEnumerable<Key>>> AddKeys([FromBody] Dto.KeyAddParameters keysParameters,
        CancellationToken cancellationToken)
    {
        var keys = keysParameters.Keys.Select(k => _keyService.Create( k.Name, k.Pass));
        
        var result = await _keyService.AddAsync(keys, cancellationToken);

        return Ok(result);
    }


}