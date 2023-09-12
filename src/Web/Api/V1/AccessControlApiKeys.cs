using ApiShared.Dto.Keys;
using Core.Entities.Keys;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("keys/issueNewKey")]
    public async Task<ActionResult<IssueNewKeyResult>> IssueNewKey([FromBody] IssueNewKeyParameters parameters, CancellationToken cancellationToken)
    {
        var key = Key.Create(parameters.Name);
        var roles = await roleService.FindByConditionAsync(r => parameters.RoleIds.Contains(r.Id), cancellationToken);
        await keyService.IssueNewKey(key, roles, cancellationToken);
        await keyService.SaveChangesAsync(cancellationToken);

        var issueNewKeyResponse = new IssueNewKeyResult
        {
            KeyId = key.Id
        };

        return Ok(issueNewKeyResponse);
    }
}