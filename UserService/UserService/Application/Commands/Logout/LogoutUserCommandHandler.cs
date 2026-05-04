using MediatR;
using UserService.Domain.Interfaces;

namespace UserService.Application.Commands.Logout;

public class LogoutUserCommandHandler(ITokenBlacklistService blacklist) : IRequestHandler<LogoutUserCommand>
{
    public async Task Handle(LogoutUserCommand command, CancellationToken ct)
    {
        TimeSpan ttl = command.ExpiresAt - DateTimeOffset.UtcNow;
        if (ttl > TimeSpan.Zero)
            await blacklist.BlacklistAsync(command.Jti, ttl, ct);
    }
}
