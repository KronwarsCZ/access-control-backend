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

        return Ok(result.Item1);
    }

    [HttpPut]
    [Route("keys/updateKey")]
    public async Task<ActionResult<Key>> UpdateKeyAsync(Guid id, [FromBody] Dto.KeyAddParameters keyParams, CancellationToken cancellationToken)
    {
        if (keyParams is null)
        {
            return BadRequest();
        }

        var keyFromParams = keyParams.Keys.First();

        var key = _keyService.Create(keyFromParams.Id, keyFromParams.Name, keyFromParams.IsDeleted, keyFromParams.Pass);
        
        var result = await _keyService.UpdateAsync(id, key, cancellationToken);

        if (result is null)
        {
            return BadRequest();
        }

        return Ok(result);
    }

    [HttpPost]
    [Route("keys/addKeys")]
    public async Task<ActionResult<IEnumerable<Key>>> AddKeys( [FromBody] Dto.KeyAddParameters keysParameters,
        CancellationToken cancellationToken)
    {
        if (keysParameters is null || !keysParameters.Keys.Any())
        {
            return BadRequest();
        }

        var keys = keysParameters.Keys
            .Select(k => _keyService.Create(k.Id, k.Name, k.IsDeleted, k.Pass));
        
        var result = await _keyService.AddAsync(keys, cancellationToken);

        return result is null ? Problem() : Ok(result);
    }


}