using ApiShared.Dto.Keys;
using Core.Entities.Keys;
using Framework.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("keys/issueNewKey")]
    public async Task<ActionResult<IssueNewKeyResult>> IssueNewKey([FromBody] IssueNewKeyParameters parameters, CancellationToken cancellationToken)
    {
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);
        Check.Contract(auth.Owner, $"Invalid {nameof(auth.Owner.Id)}");
        Check.Contract(auth.Owner.Id != Guid.Empty, $"Invalid {nameof(auth.Owner.Id)}");
        
        var key = Key.Create(auth.Owner, parameters.Name);
        
        Check.Contract(!string.IsNullOrWhiteSpace(parameters.Name), $"Invalid {nameof(parameters.Name)}");
        
        Check.Contract(parameters.RoleIds, $"Invalid {nameof(parameters.RoleIds)}");
        Check.Contract(parameters.RoleIds.Any(), $"Invalid {nameof(parameters.RoleIds)}");
        var roles = await roleService.FindByConditionAsync(r => parameters.RoleIds.Contains(r.Id), cancellationToken);
        Check.Contract(roles.Any(), $"Invalid {nameof(parameters.RoleIds)}");
        foreach (var role in roles)
        {
            Check.Contract(role.OwnerId == auth.Owner.Id,
                $"Invalid Id and {nameof(parameters.RoleIds)}");
        }
        
        await keyService.IssueNewKey(key, roles, cancellationToken);
        await keyService.SaveChangesAsync(cancellationToken);

        var issueNewKeyResponse = new IssueNewKeyResult
        {
            KeyId = key.Id
        };

        return Ok(issueNewKeyResponse);
    }
}