using Application.IRepositories;
using Application.Services;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
	[HttpPost]
	[Route("keys/getAll")]
	public async Task<ActionResult<IEnumerable<Key>>> KeysGetAll([FromBody] Dto.KeyFilterParameters parameters, CancellationToken cancellationToken)
	{
		var filters = new KeyFilters();

		var keys = await _keyService.GetAsync(filters, cancellationToken);

		return Ok(keys);
	}
}