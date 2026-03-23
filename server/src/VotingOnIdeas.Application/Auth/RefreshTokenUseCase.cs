using VotingOnIdeas.Application.Exceptions;
using VotingOnIdeas.Application.Interfaces;
using VotingOnIdeas.Domain.Entities;
using VotingOnIdeas.Domain.Interfaces;

namespace VotingOnIdeas.Application.Auth;

public sealed class RefreshTokenUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _tokenService;

    public RefreshTokenUseCase(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IJwtTokenService tokenService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> ExecuteAsync(string refreshTokenValue, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _refreshTokenRepository.GetActiveTokenAsync(refreshTokenValue, cancellationToken)
            ?? throw new UnauthorizedException("Invalid or expired refresh token.");

        var user = await _userRepository.GetByIdAsync(refreshToken.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), refreshToken.UserId);

        refreshToken.Revoke();

        var (newRefreshValue, newRefreshExpiry) = _tokenService.GenerateRefreshToken();
        var newRefreshToken = RefreshToken.Create(user.Id, newRefreshValue, newRefreshExpiry);
        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = _tokenService.GenerateAccessToken(user);
        return new AuthResponse(accessToken, newRefreshValue, new UserDto(user.Id, user.Username, user.Email, user.Role));
    }
}
