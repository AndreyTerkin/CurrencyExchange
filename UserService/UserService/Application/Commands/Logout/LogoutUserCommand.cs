using MediatR;

namespace UserService.Application.Commands.Logout;

public record LogoutUserCommand(string Jti, DateTimeOffset ExpiresAt) : IRequest;
