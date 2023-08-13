using System.ComponentModel.DataAnnotations;
using System.Net;
using Framework.Utilities;
using Logging.Utils;
using Serilog;

namespace AccessControl.Web.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var correlationId = LoggingHttpContextHelper.GetCorrelationId(context);
        LoggingHttpContextHelper.SetCorrelationId(correlationId, context);

        try
        {
            await _next.Invoke(context);
        }
        catch (ContractException e)
        {
            Log.Warning(e, e.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync($"{{ \"Error message\": \"{e.Message}\" }}");
        }

        catch (ValidationException e)
        {
            Log.Warning(e, e.Message);
            throw LoggingHelper.CreateBaseExceptionModel(e.Message, context.Response.StatusCode, correlationId);
        }
        
        catch (HttpRequestException e)
        {
            Log.Fatal(e, e.Message);
            throw LoggingHelper.CreateBaseExceptionModel(e.Message, context.Response.StatusCode, correlationId);
        }
        
        catch (Exception e)
        {
            Log.Fatal(e, e.Message);
            throw LoggingHelper.CreateBaseExceptionModel($"Something went wrong... Correlation ID: {correlationId}", context.Response.StatusCode, correlationId);
        }
    }
}