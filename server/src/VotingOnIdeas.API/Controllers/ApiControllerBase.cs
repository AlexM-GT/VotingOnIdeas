using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VotingOnIdeas.Application.Exceptions;
using VotingOnIdeas.Domain.Constants;

namespace VotingOnIdeas.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected Guid GetCurrentUserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(sub, out var userId))
            throw new UnauthorizedException();

        return userId;
    }

    protected string GetCurrentUserRole()
        => User.FindFirstValue(ClaimTypes.Role) ?? UserRole.User;
}
