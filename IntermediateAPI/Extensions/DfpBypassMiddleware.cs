using IntermediateAPI.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace IntermediateAPI.Extensions
{
    public class DfpBypassMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly FraudProtectionSettings _fraudProtectionSettings;
        public DfpBypassMiddleware(RequestDelegate next, IOptions<FraudProtectionSettings> fraudProtectionSettings)
        {
            _next = next;
            _fraudProtectionSettings = fraudProtectionSettings.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var url = context.Request.Path.Value;

            if (!string.IsNullOrEmpty(url) && url.ToLower().Contains("/dfp/") && _fraudProtectionSettings.BypassDfp)

            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.OK;

                await context.Response.WriteAsync(JsonConvert.SerializeObject(new DfpAccountGenericOutputClaims() { Decision = "Approve", CorrelationId = "DFPBypassed" }));
            }
            else
            {
                // Call the next delegate/middleware in the pipeline.
                await _next(context);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class DfpBypassMiddlewareExtensions
    {
        public static IApplicationBuilder UseDfpBypassing(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DfpBypassMiddleware>();
        }
    }
}
