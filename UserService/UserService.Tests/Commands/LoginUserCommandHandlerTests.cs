using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using UserService.Application.Commands.Login;
using UserService.Application.Exceptions;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Tests.Commands;

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _repoMock = new();
    private readonly Mock<IPasswordHasher<User>> _hasherMock = new();
    private readonly Mock<IJwtTokenService> _jwtMock = new();
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        _handler = new LoginUserCommandHandler(_repoMock.Object, _hasherMock.Object, _jwtMock.Object);
    }

    [Fact]
    public async Task Handle_WithCorrectPassword_ReturnsToken()
    {
        // Arrange
        User user = User.Create("john", "hashed_password");
        _repoMock.Setup(r => r.GetByNameAsync("john", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _hasherMock.Setup(h => h.VerifyHashedPassword(user, "hashed_password", "password123"))
            .Returns(PasswordVerificationResult.Success);
        _jwtMock.Setup(j => j.GenerateToken(user))
            .Returns("jwt_token");

        var command = new LoginUserCommand("john", "password123");

        // Act
        string token = await _handler.Handle(command, CancellationToken.None);

        // Assert
        token.Should().Be("jwt_token");
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ThrowsValidationException()
    {
        // Arrange
        User user = User.Create("john", "hashed_password");
        _repoMock.Setup(r => r.GetByNameAsync("john", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _hasherMock.Setup(h => h.VerifyHashedPassword(user, "hashed_password", "wrong_password"))
            .Returns(PasswordVerificationResult.Failed);

        var command = new LoginUserCommand("john", "wrong_password");

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Invalid credentials.");
    }
}
