using VotingOnIdeas.Domain.Interfaces;

namespace VotingOnIdeas.Application.Auth;

public sealed class LogoutUseCase
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutUseCase(IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(string refreshTokenValue, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _refreshTokenRepository.GetActiveTokenAsync(refreshTokenValue, cancellationToken);
        if (refreshToken is null)
            return;

        refreshToken.Revoke();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
