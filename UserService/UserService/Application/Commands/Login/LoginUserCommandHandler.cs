using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Exceptions;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.Commands.Login;

public class LoginUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher<User> passwordHasher,
    IJwtTokenService jwtTokenService) : IRequestHandler<LoginUserCommand, string>
{
    public async Task<string> Handle(LoginUserCommand command, CancellationToken ct)
    {
        User? user = await userRepository.GetByNameAsync(command.Name, ct);
        if (user is null)
            throw new ValidationException("Invalid credentials.");

        PasswordVerificationResult result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, command.Password);
        if (result == PasswordVerificationResult.Failed)
            throw new ValidationException("Invalid credentials.");

        return jwtTokenService.GenerateToken(user);
    }
}
