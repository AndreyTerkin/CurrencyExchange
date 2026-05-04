using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Exceptions;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.Commands.Register;

public class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher<User> passwordHasher) : IRequestHandler<RegisterUserCommand, int>
{
    public async Task<int> Handle(RegisterUserCommand command, CancellationToken ct)
    {
        if (await userRepository.ExistsAsync(command.Name, ct))
            throw new ValidationException($"User '{command.Name}' already exists.");

        // PasswordHasher<T> does not use the user argument during hashing
        string hash = passwordHasher.HashPassword(null!, command.Password);
        User user = User.Create(command.Name, hash);

        await userRepository.AddAsync(user, ct);
        await userRepository.SaveChangesAsync(ct);

        return user.Id;
    }
}
