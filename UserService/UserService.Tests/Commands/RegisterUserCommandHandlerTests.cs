using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using UserService.Application.Commands.Register;
using UserService.Application.Exceptions;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Tests.Commands;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _repoMock = new();
    private readonly Mock<IPasswordHasher<User>> _hasherMock = new();
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _handler = new RegisterUserCommandHandler(_repoMock.Object, _hasherMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidData_CreatesUser_And_ReturnsId()
    {
        // Arrange
        _repoMock.Setup(r => r.ExistsAsync("john", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _hasherMock.Setup(h => h.HashPassword(It.IsAny<User>(), "password123"))
            .Returns("hashed_password");
        _repoMock.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new RegisterUserCommand("john", "password123");

        // Act
        int result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThanOrEqualTo(0);
        _repoMock.Verify(r => r.AddAsync(
            It.Is<User>(u => u.Name == "john" && u.PasswordHash == "hashed_password"),
            It.IsAny<CancellationToken>()), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithDuplicateName_ThrowsValidationException()
    {
        // Arrange
        _repoMock.Setup(r => r.ExistsAsync("john", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new RegisterUserCommand("john", "password123");

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*already exists*");
    }
}
