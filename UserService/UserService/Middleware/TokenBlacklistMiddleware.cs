using System.IdentityModel.Tokens.Jwt;
using UserService.Domain.Interfaces;

namespace UserService.Middleware;

public class TokenBlacklistMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITokenBlacklistService blacklist)
    {
        string? jti = context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

        if (jti is not null && await blacklist.IsBlacklistedAsync(jti, context.RequestAborted))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await next(context);
    }
}
