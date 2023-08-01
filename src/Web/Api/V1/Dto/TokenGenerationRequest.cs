using System.Collections;

namespace AccessControl.Web.Api.V1.Dto;

public class TokenGenerationRequest
{
    public string Email { get; set; }
    public Guid UserId { get; set; }
    public IEnumerable CustomClaims { get; set; }
}
