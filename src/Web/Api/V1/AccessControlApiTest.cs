using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpGet]
    [Route("Persons/getAll")]
    public ActionResult<string> NecoEndpoint()
    {
        var a = context.Person.ToList();
        return Ok(a);
    }
    
    [HttpGet]
    [Route("Persons/add")]
    public async Task<ActionResult<string>> NecoEndpoint2(CancellationToken cancellationToken)
    {
        var a = new Person
        {
            FirstName = "martin",
            LastName = "obecny",
        }; 
        await context.AddAsync(a, cancellationToken);
        await context.SaveChangesAsync(cancellationToken: cancellationToken);
        return Ok(a);
    }
}
