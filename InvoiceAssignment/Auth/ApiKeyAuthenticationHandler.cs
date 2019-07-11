using InvoiceAssignment.Domain.Communication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace InvoiceAssignment.Auth
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private const string ProblemDetailsContentType = "application/problem+json";
        private const string ApiKeyHeaderName = "X-Api-Key";
        private readonly IConfiguration _config;
        private readonly string _apiKey;
        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration config) : base(options, logger, encoder, clock)
        {
            _config = config;
            _apiKey = _config["ApiKey"] ?? throw new ArgumentNullException(nameof(_apiKey));
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // If no X-Api-Key header is present 
            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            // If the header is present but null or empty
            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();
            if (apiKeyHeaderValues.Count == 0 || string.IsNullOrWhiteSpace(providedApiKey))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            if (_apiKey.Equals(providedApiKey))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "User")
                };

                claims.Add(new Claim(ClaimTypes.Role, "User"));

                var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
                var identities = new List<ClaimsIdentity> { identity };
                var principal = new ClaimsPrincipal(identities);
                var ticket = new AuthenticationTicket(principal, Options.Scheme);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }

            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key provided."));
        }

        // 401 response
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 401;
            Response.ContentType = ProblemDetailsContentType;

            await Response.WriteAsync(JsonConvert.SerializeObject(new BaseResponse(401)));
        }

        // 403 response
        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 403;
            Response.ContentType = ProblemDetailsContentType;

            await Response.WriteAsync(JsonConvert.SerializeObject(new BaseResponse(403)));
        }
    }
}
