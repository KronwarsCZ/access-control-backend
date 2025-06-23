#nullable enable
using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using Framework.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Serilog;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("iot/notify")]
    public async Task<ActionResult> NotifiedAboutNewDevice([FromBody] EventGridEvent[] events, CancellationToken cancellationToken)
    {
        Check.Contract(events, "Payload cannot be null");
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);
        //Check.Contract(auth.Owner, "Authentication error");

        SubscriptionValidationResponse? validationResponse = null;
        foreach (var ege in events)
        {
            var result = await ioTHubDeviceService.EventRouter(ege, cancellationToken);
            if (result != null)
            {
                validationResponse = result;
            }
        }

        return Ok(validationResponse);
    }

}
