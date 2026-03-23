using FluentValidation;
using VotingOnIdeas.Application.Exceptions;
using VotingOnIdeas.Application.Interfaces;
using VotingOnIdeas.Domain.Entities;
using VotingOnIdeas.Domain.Interfaces;

namespace VotingOnIdeas.Application.Auth;

public sealed class RegisterUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _tokenService;
    private readonly IValidator<RegisterCommand> _validator;

    public RegisterUseCase(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenService tokenService,
        IValidator<RegisterCommand> validator)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _validator = validator;
    }

    public async Task<AuthResponse> ExecuteAsync(RegisterCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new Exceptions.ValidationException(errors);
        }

        if (await _userRepository.ExistsByEmailAsync(command.Email, cancellationToken))
            throw new ConflictException("A user with this email already exists.");

        if (await _userRepository.ExistsByUsernameAsync(command.Username, cancellationToken))
            throw new ConflictException("A user with this username already exists.");

        var hash = _passwordHasher.Hash(command.Password, out var salt);
        var user = User.Create(command.Username, command.Email, hash, salt);
        await _userRepository.AddAsync(user, cancellationToken);

        var (refreshValue, refreshExpiry) = _tokenService.GenerateRefreshToken();
        var refreshToken = RefreshToken.Create(user.Id, refreshValue, refreshExpiry);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = _tokenService.GenerateAccessToken(user);
        return new AuthResponse(accessToken, refreshValue, new UserDto(user.Id, user.Username, user.Email, user.Role));
    }
}
