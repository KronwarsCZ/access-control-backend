namespace AccessControl.Web.Api.Dto;

public class TemplateDtoResponse
{
    public IEnumerable<Guid> Ids { get; set; }

    public IEnumerable<string> Words { get; set; }
}