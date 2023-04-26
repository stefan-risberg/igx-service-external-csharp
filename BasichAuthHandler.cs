using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

#pragma warning disable 1591
/// <summary>
/// Handeling basic authentication.
/// </summary>
public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions> {
    IUsers usersDb;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IUsers usersDb
    ) : base(options, logger, encoder, clock) {
        this.usersDb = usersDb;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync() {
        var logger = Logger;
        string? authHeader = Request.Headers["Authorization"];

        if (authHeader == null) {
            logger.LogInformation("No auth header");

            Response.StatusCode = 401;
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
        } else if (!authHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase)) {
            logger.LogInformation("Header does not contain a basic auth part");

            Response.StatusCode = 401;
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
        }

        var token = authHeader.Substring("basic ".Length).Trim();
        var credentialsString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
        var credentials = credentialsString.Split(":");

        if (credentials != null && credentials.Length == 2) {
            var authorized = usersDb.ValidateUser(credentials[0], credentials[1]);

            if (authorized) {
                var claims = new[] { new Claim(ClaimTypes.Name, credentials[0]), new Claim(ClaimTypes.Role, "User") };
                var identity = new ClaimsIdentity(claims, "Basic");
                var claimsPrincipal = new ClaimsPrincipal(identity);
                return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
            } else {
                logger.LogDebug("Not authorized");
            }
        } else {
            logger.LogWarning("Failed to parse credentials");
            Response.StatusCode = 500;
            return Task.FromResult(AuthenticateResult.Fail("Failed to parse credentials"));
        }

        Response.StatusCode = 401;
        return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
    }
}
