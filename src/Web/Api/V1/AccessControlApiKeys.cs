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

        if (keys is null)
        {
            return NoContent();
        }
		return Ok(keys);
	}
    
    // get all keys that can open a device with given device id
    [HttpGet]
    [Route("keys/getKeysForDevice")]
    public async Task<ActionResult<IEnumerable<Key>>> GetKeysForDeviceAsync(Guid id, CancellationToken cancellationToken)
    {
        var keys = await _keyService.GetKeysForDeviceAsync(id, cancellationToken);

        if (keys is null)
        {
            return NoContent();
        }
        
        return Ok(keys);
    }

    [HttpDelete]
    [Route("keys/deleteKeys")]
    public async Task<ActionResult<(int, IEnumerable<string>)>> DeleteKeysAsync(
        [FromBody] Dto.KeyFilterParameters parameters, CancellationToken cancellationToken)
    {
        if (parameters?.Ids is null)
        {
            return UnprocessableEntity();
        }

        var result = await _keyService.DeleteAsync(parameters.Ids, cancellationToken);

        if (result is { Item1: 0, Item2: null })
        {
            return BadRequest();
        }

        return Ok(result);
    }

    [HttpPut]
    [Route("keys/updateKey")]
    public async Task<ActionResult<Key>> UpdateKeyAsync(Guid id, Key key, CancellationToken cancellationToken)
    {
        if (key is null)
        {
            return BadRequest();
        }

        var result = await _keyService.UpdateAsync(id, key, cancellationToken);

        if (result is null)
        {
            return BadRequest();
        }

        return Ok(result);
    }

    [HttpPost]
    [Route("keys/addKey")]
    public async Task<ActionResult<IEnumerable<Key>>> AddKeys(IEnumerable<Key> keys,
        CancellationToken cancellationToken)
    {
        if (keys is null)
        {
            return BadRequest();
        }

        var result = await _keyService.AddAsync(keys, cancellationToken);

        return result is null ? Problem() : Ok(result);
    }


}