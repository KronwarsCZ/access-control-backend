using Azure.Messaging.EventGrid;
using Framework.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;

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

        foreach (var e in events)
        {
            if (e.EventType != "Microsoft.Devices.DeviceCreated") continue;
            
            // tady registruj novy device do db pres IoT Hub service
            await ioTHubDeviceService.NotifiedOfNewDevice(events, cancellationToken);
        }
        return Ok();
    }

}
