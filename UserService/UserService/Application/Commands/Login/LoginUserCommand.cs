using MediatR;

namespace UserService.Application.Commands.Login;

public record LoginUserCommand(string Name, string Password) : IRequest<string>;
