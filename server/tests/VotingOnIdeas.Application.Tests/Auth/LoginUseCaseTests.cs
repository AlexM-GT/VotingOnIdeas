using FluentAssertions;
using NSubstitute;
using VotingOnIdeas.Application.Auth;
using VotingOnIdeas.Application.Exceptions;
using VotingOnIdeas.Application.Interfaces;
using VotingOnIdeas.Application.Tests.Helpers;
using VotingOnIdeas.Domain.Entities;
using VotingOnIdeas.Domain.Interfaces;

namespace VotingOnIdeas.Application.Tests.Auth;

public sealed class LoginUseCaseTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly TestPasswordHasher _passwordHasher = new();
    private readonly IJwtTokenService _tokenService = Substitute.For<IJwtTokenService>();
    private readonly LoginUseCase _sut;

    public LoginUseCaseTests()
    {
        _tokenService.GenerateAccessToken(Arg.Any<User>()).Returns("access-token");
        _tokenService.GenerateRefreshToken().Returns(("refresh-token", DateTime.UtcNow.AddDays(7)));

        _sut = new LoginUseCase(
            _userRepository, _refreshTokenRepository, _unitOfWork,
            _passwordHasher, _tokenService, new LoginCommandValidator());
    }

    [Fact]
    public async Task ExecuteAsync_ValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var hash = _passwordHasher.Hash("Password123", out var salt);
        var user = User.Create("johndoe", "john@example.com", hash, salt);
        _userRepository.GetByEmailAsync("john@example.com", Arg.Any<CancellationToken>()).Returns(user);

        var command = new LoginCommand("john@example.com", "Password123");

        // Act
        var result = await _sut.ExecuteAsync(command);

        // Assert
        result.AccessToken.Should().Be("access-token");
        result.User.Email.Should().Be("john@example.com");
    }

    [Fact]
    public async Task ExecuteAsync_UserNotFound_ThrowsUnauthorizedException()
    {
        // Arrange
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var command = new LoginCommand("missing@example.com", "Password123");

        // Act
        var act = () => _sut.ExecuteAsync(command);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task ExecuteAsync_WrongPassword_ThrowsUnauthorizedException()
    {
        // Arrange
        var hash = _passwordHasher.Hash("CorrectPassword", out var salt);
        var user = User.Create("johndoe", "john@example.com", hash, salt);
        _userRepository.GetByEmailAsync("john@example.com", Arg.Any<CancellationToken>()).Returns(user);

        var command = new LoginCommand("john@example.com", "WrongPassword");

        // Act
        var act = () => _sut.ExecuteAsync(command);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid email or password.");
    }
}
