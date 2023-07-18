using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Serilog;

namespace AccessControl.Web.BlazorUI.ErrorHandling;

public class CustomErrorBoundary : ErrorBoundary
{
    protected override Task OnErrorAsync(Exception exception)
    {
        Log.Fatal(exception.Message);
        return Task.CompletedTask;
    }
}
