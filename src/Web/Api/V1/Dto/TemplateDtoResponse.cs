namespace AccessControl.Web.Api.V1.Dto;

public class TemplateDtoResponse
{
    public IEnumerable<Guid> Ids { get; set; }

    public IEnumerable<string> Words { get; set; }
}