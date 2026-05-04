using MediatR;

namespace UserService.Application.Commands.Register;

public record RegisterUserCommand(string Name, string Password) : IRequest<int>;
