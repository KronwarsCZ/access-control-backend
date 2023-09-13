using ApiShared.Dto.Keys;
using Core.Entities.Keys;
using Framework.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices.Common;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("keys/issueNewKey")]
    public async Task<ActionResult<IssueNewKeyResult>> IssueNewKey([FromBody] IssueNewKeyParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters.OwnerId != Guid.Empty, $"Invalid {nameof(parameters.OwnerId)}");
        var owner = await ownerService.GetByIdAsync(parameters.OwnerId, new CancellationToken());
        Check.Contract(owner, $"Invalid {nameof(parameters.OwnerId)}");
        var key = Key.Create(owner, parameters.Name);
        
        Check.Contract(!string.IsNullOrWhiteSpace(parameters.Name), $"Invalid {nameof(parameters.Name)}");
        
        Check.Contract(parameters.RoleIds, $"Invalid {nameof(parameters.RoleIds)}");
        Check.Contract(parameters.RoleIds.Any(), $"Invalid {nameof(parameters.RoleIds)}");
        var roles = await roleService.FindByConditionAsync(r => parameters.RoleIds.Contains(r.Id), cancellationToken);
        Check.Contract(roles.Any(), $"Invalid {nameof(parameters.RoleIds)}");
        foreach (var role in roles)
        {
            Check.Contract(role.OwnerId == owner.Id,
                $"Invalid {nameof(parameters.OwnerId)} and {nameof(parameters.RoleIds)}");
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