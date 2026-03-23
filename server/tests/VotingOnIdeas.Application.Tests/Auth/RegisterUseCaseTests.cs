using FluentAssertions;
using NSubstitute;
using VotingOnIdeas.Application.Auth;
using VotingOnIdeas.Application.Exceptions;
using VotingOnIdeas.Application.Interfaces;
using VotingOnIdeas.Application.Tests.Helpers;
using VotingOnIdeas.Domain.Entities;
using VotingOnIdeas.Domain.Interfaces;

namespace VotingOnIdeas.Application.Tests.Auth;

public sealed class RegisterUseCaseTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IPasswordHasher _passwordHasher = new TestPasswordHasher();
    private readonly IJwtTokenService _tokenService = Substitute.For<IJwtTokenService>();
    private readonly RegisterUseCase _sut;

    public RegisterUseCaseTests()
    {
        _tokenService.GenerateAccessToken(Arg.Any<User>()).Returns("access-token");
        _tokenService.GenerateRefreshToken().Returns(("refresh-token", DateTime.UtcNow.AddDays(7)));

        _sut = new RegisterUseCase(
            _userRepository, _refreshTokenRepository, _unitOfWork,
            _passwordHasher, _tokenService, new RegisterCommandValidator());
    }

    [Fact]
    public async Task ExecuteAsync_ValidCommand_ReturnsAuthResponse()
    {
        // Arrange
        var command = new RegisterCommand("johndoe", "john@example.com", "Password123");
        _userRepository.ExistsByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
        _userRepository.ExistsByUsernameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await _sut.ExecuteAsync(command);

        // Assert
        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
        result.User.Username.Should().Be("johndoe");
        result.User.Email.Should().Be("john@example.com");
        await _unitOfWork.Received().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_DuplicateEmail_ThrowsConflictException()
    {
        // Arrange
        var command = new RegisterCommand("johndoe", "existing@example.com", "Password123");
        _userRepository.ExistsByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var act = () => _sut.ExecuteAsync(command);

        // Assert
        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task ExecuteAsync_DuplicateUsername_ThrowsConflictException()
    {
        // Arrange
        var command = new RegisterCommand("existinguser", "new@example.com", "Password123");
        _userRepository.ExistsByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
        _userRepository.ExistsByUsernameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var act = () => _sut.ExecuteAsync(command);

        // Assert
        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task ExecuteAsync_InvalidCommand_ThrowsValidationException()
    {
        // Arrange
        var command = new RegisterCommand("", "invalid-email", "short");

        // Act
        var act = () => _sut.ExecuteAsync(command);

        // Assert
        await act.Should().ThrowAsync<VotingOnIdeas.Application.Exceptions.ValidationException>();
    }
}
