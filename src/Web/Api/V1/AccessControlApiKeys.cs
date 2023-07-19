using Application.IRepositories;
using Application.Services;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
	// Get all key ids that can be open me (a device with given id)
	[HttpPost]
	[Route("keys/getAll")]
	public async Task<ActionResult<IEnumerable<Key>>> KeysGetAll([FromBody] Dto.KeyFilterParameters parameters, CancellationToken cancellationToken)
	{
		if (parameters?.Ids is null)
		{
			return UnprocessableEntity();
		}
		
		var id = parameters.Ids.First();
		var filters = new KeyFilters(id);

		var keys = await _keyService.GetAsync(filters, cancellationToken);

		return Ok(keys);
	}
}