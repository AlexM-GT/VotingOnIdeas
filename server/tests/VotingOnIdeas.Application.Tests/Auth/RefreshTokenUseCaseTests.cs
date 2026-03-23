using FluentAssertions;
using NSubstitute;
using VotingOnIdeas.Application.Auth;
using VotingOnIdeas.Application.Exceptions;
using VotingOnIdeas.Application.Interfaces;
using VotingOnIdeas.Domain.Entities;
using VotingOnIdeas.Domain.Interfaces;

namespace VotingOnIdeas.Application.Tests.Auth;

public sealed class RefreshTokenUseCaseTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IJwtTokenService _tokenService = Substitute.For<IJwtTokenService>();
    private readonly RefreshTokenUseCase _sut;

    public RefreshTokenUseCaseTests()
    {
        _tokenService.GenerateAccessToken(Arg.Any<User>()).Returns("new-access-token");
        _tokenService.GenerateRefreshToken().Returns(("new-refresh-token", DateTime.UtcNow.AddDays(7)));

        _sut = new RefreshTokenUseCase(
            _userRepository, _refreshTokenRepository, _unitOfWork, _tokenService);
    }

    [Fact]
    public async Task ExecuteAsync_ValidToken_RotatesTokensAndReturnsNewAuthResponse()
    {
        // Arrange
        var user = User.Create("johndoe", "john@example.com", "hash", "salt");
        var existingToken = RefreshToken.Create(user.Id, "valid-refresh-token", DateTime.UtcNow.AddDays(7));

        _refreshTokenRepository.GetActiveTokenAsync("valid-refresh-token", Arg.Any<CancellationToken>())
            .Returns(existingToken);
        _userRepository.GetByIdAsync(existingToken.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await _sut.ExecuteAsync("valid-refresh-token");

        // Assert
        result.AccessToken.Should().Be("new-access-token");
        result.RefreshToken.Should().Be("new-refresh-token");
        existingToken.IsRevoked.Should().BeTrue();
        await _unitOfWork.Received().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_InvalidToken_ThrowsUnauthorizedException()
    {
        // Arrange
        _refreshTokenRepository.GetActiveTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((RefreshToken?)null);

        // Act
        var act = () => _sut.ExecuteAsync("invalid-token");

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}
