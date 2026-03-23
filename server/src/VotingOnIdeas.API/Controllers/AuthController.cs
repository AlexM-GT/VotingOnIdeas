using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingOnIdeas.Application.Auth;

namespace VotingOnIdeas.API.Controllers;

[Route("api/[controller]")]
public sealed class AuthController : ApiControllerBase
{
    private readonly RegisterUseCase _register;
    private readonly LoginUseCase _login;
    private readonly RefreshTokenUseCase _refreshToken;
    private readonly LogoutUseCase _logout;
    private readonly GetCurrentUserUseCase _getCurrentUser;

    public AuthController(
        RegisterUseCase register,
        LoginUseCase login,
        RefreshTokenUseCase refreshToken,
        LogoutUseCase logout,
        GetCurrentUserUseCase getCurrentUser)
    {
        _register = register;
        _login = login;
        _refreshToken = refreshToken;
        _logout = logout;
        _getCurrentUser = getCurrentUser;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(
        [FromBody] RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var response = await _register.ExecuteAsync(command, cancellationToken);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var response = await _login.ExecuteAsync(command, cancellationToken);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _refreshToken.ExecuteAsync(request.RefreshToken, cancellationToken);
        return Ok(response);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        await _logout.ExecuteAsync(request.RefreshToken, cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var user = await _getCurrentUser.ExecuteAsync(GetCurrentUserId(), cancellationToken);
        return Ok(user);
    }
}

public record RefreshTokenRequest(string RefreshToken);
