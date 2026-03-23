using VotingOnIdeas.Application.Exceptions;
using VotingOnIdeas.Domain.Entities;
using VotingOnIdeas.Domain.Interfaces;

namespace VotingOnIdeas.Application.Auth;

public sealed class GetCurrentUserUseCase
{
    private readonly IUserRepository _userRepository;

    public GetCurrentUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), userId);

        return new UserDto(user.Id, user.Username, user.Email, user.Role);
    }
}
