using Application.Services;
using Core;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api;

[ApiController]
[Route("[controller]")]
public class NecoEndpointController : ControllerBase
{
    private readonly INecoService _service;

    public NecoEndpointController(INecoService necoService)
    {
        _service = necoService;
    }

    [HttpGet]
    public ActionResult<Dto.TemplateDtoResponse> NecoEndpoint()
    {
        IEnumerable<Stringos> result = _service.ContractedProcessMethod();
        var res = result.Select(r => new Dto.TemplateDtoResponse()
        {
            Words = new List<string>(){r.Text},
            Ids = new List<Guid>(){Guid.NewGuid()}
        });
        return Ok(res);
    }
}
