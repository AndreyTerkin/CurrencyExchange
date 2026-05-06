using FluentAssertions;
using Moq;
using UserService.Application.Commands.Logout;
using UserService.Domain.Interfaces;

namespace UserService.Tests.Commands;

public class LogoutUserCommandHandlerTests
{
    private readonly Mock<ITokenBlacklistService> _blacklistMock = new();
    private readonly LogoutUserCommandHandler _handler;

    public LogoutUserCommandHandlerTests()
    {
        _handler = new LogoutUserCommandHandler(_blacklistMock.Object);
    }

    [Fact]
    public async Task Handle_WithFutureExpiry_BlacklistsJti()
    {
        // Arrange
        string jti = "test-jti";
        DateTimeOffset expiresAt = DateTimeOffset.UtcNow.AddMinutes(30);
        var command = new LogoutUserCommand(jti, expiresAt);

        _blacklistMock.Setup(b => b.BlacklistAsync(jti, It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _blacklistMock.Verify(b => b.BlacklistAsync(
            jti,
            It.Is<TimeSpan>(ttl => ttl > TimeSpan.Zero),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithPastExpiry_DoesNotCallBlacklist()
    {
        // Arrange
        var command = new LogoutUserCommand("expired-jti", DateTimeOffset.UtcNow.AddMinutes(-5));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _blacklistMock.Verify(b => b.BlacklistAsync(
            It.IsAny<string>(),
            It.IsAny<TimeSpan>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}
