using FluentValidation;

namespace VotingOnIdeas.Application.Auth;

public record LoginCommand(string Email, string Password);

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
