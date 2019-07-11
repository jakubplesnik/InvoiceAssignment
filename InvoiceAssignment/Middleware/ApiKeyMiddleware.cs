using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace InvoiceAssignment.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly IConfiguration _config;
        private readonly string _apiKey;

        private RequestDelegate _next;

        private const string headerName = "Api-Key";

        public ApiKeyMiddleware(IConfiguration config, RequestDelegate next)
        {
            _config = config;
            _apiKey = _config["ApiKey"];

            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // check if this is an API call
            if (context.Request.Path.StartsWithSegments(new PathString("/api")))
            {
                // validate API key
                bool valid = false;

                if (context.Request.Headers.Keys.Contains(headerName, StringComparer.InvariantCultureIgnoreCase))
                {
                    if (context.Request.Headers[headerName].Equals(_apiKey))
                    {
                        valid = true;
                    }
                }

                if (!valid)
                {
                    // return 401 Unauthorized in case of invalid API key
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("Invalid API key.");
                }
                else
                {
                    await _next.Invoke(context);
                }
            }
        }
    }

    public static class HandlerExtensions
    {
        public static IApplicationBuilder UseApiKeyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyMiddleware>();
        }
    }
}
