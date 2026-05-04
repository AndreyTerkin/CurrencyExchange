using System.IdentityModel.Tokens.Jwt;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Commands.Login;
using UserService.Application.Commands.Logout;
using UserService.Application.Commands.Register;

namespace UserService.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken ct)
    {
        int userId = await mediator.Send(command, ct);
        return Ok(new { id = userId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command, CancellationToken ct)
    {
        string token = await mediator.Send(command, ct);
        return Ok(new { token });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        string? jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        string? expStr = User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;

        if (jti is null || expStr is null)
            return Unauthorized();

        DateTimeOffset expiresAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expStr));
        await mediator.Send(new LogoutUserCommand(jti, expiresAt), ct);

        return NoContent();
    }
}
