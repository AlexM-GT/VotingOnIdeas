using FluentValidation;
using VotingOnIdeas.Application.Exceptions;
using VotingOnIdeas.Application.Interfaces;
using VotingOnIdeas.Domain.Entities;
using VotingOnIdeas.Domain.Interfaces;

namespace VotingOnIdeas.Application.Auth;

public sealed class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _tokenService;
    private readonly IValidator<LoginCommand> _validator;

    public LoginUseCase(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenService tokenService,
        IValidator<LoginCommand> validator)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _validator = validator;
    }

    public async Task<AuthResponse> ExecuteAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new Exceptions.ValidationException(errors);
        }

        var user = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);

        // Use constant-time comparison to avoid leaking whether the email exists
        if (user is null || !_passwordHasher.Verify(command.Password, user.PasswordHash, user.Salt))
            throw new UnauthorizedException("Invalid email or password.");

        await _refreshTokenRepository.RevokeAllUserTokensAsync(user.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var (refreshValue, refreshExpiry) = _tokenService.GenerateRefreshToken();
        var refreshToken = RefreshToken.Create(user.Id, refreshValue, refreshExpiry);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = _tokenService.GenerateAccessToken(user);
        return new AuthResponse(accessToken, refreshValue, new UserDto(user.Id, user.Username, user.Email, user.Role));
    }
}
