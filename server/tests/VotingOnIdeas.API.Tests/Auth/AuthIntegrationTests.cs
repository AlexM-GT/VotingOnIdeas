using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using VotingOnIdeas.API.Tests.Infrastructure;

namespace VotingOnIdeas.API.Tests.Auth;

[Collection("Integration")]
public sealed class AuthIntegrationTests
{
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public AuthIntegrationTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsOkWithTokens()
    {
        // Arrange
        var id = Guid.NewGuid().ToString("N")[..8];
        var request = new { Username = $"user_{id}", Email = $"{id}@example.com", Password = "Password123" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var body = JsonSerializer.Deserialize<AuthResponseDto>(json, JsonOptions);
        body!.AccessToken.Should().NotBeNullOrEmpty();
        body.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsConflict()
    {
        // Arrange
        var id = Guid.NewGuid().ToString("N")[..8];
        var request = new { Username = $"u1_{id}", Email = $"{id}@example.com", Password = "Password123" };
        await _client.PostAsJsonAsync("/api/auth/register", request);
        var duplicate = new { Username = $"u2_{id}", Email = $"{id}@example.com", Password = "Password123" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", duplicate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithTokens()
    {
        // Arrange
        var id = Guid.NewGuid().ToString("N")[..8];
        var email = $"{id}@example.com";
        await _client.PostAsJsonAsync("/api/auth/register",
            new { Username = $"user_{id}", Email = email, Password = "Password123" });

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new { Email = email, Password = "Password123" });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var body = JsonSerializer.Deserialize<AuthResponseDto>(json, JsonOptions);
        body!.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsForbidden()
    {
        // Arrange
        var id = Guid.NewGuid().ToString("N")[..8];
        var email = $"{id}@example.com";
        await _client.PostAsJsonAsync("/api/auth/register",
            new { Username = $"user_{id}", Email = email, Password = "Password123" });

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new { Email = email, Password = "WrongPassword" });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetMe_WithValidToken_ReturnsCurrentUser()
    {
        // Arrange
        var id = Guid.NewGuid().ToString("N")[..8];
        var email = $"{id}@example.com";
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register",
            new { Username = $"user_{id}", Email = email, Password = "Password123" });
        var registerJson = await registerResponse.Content.ReadAsStringAsync();
        var auth = JsonSerializer.Deserialize<AuthResponseDto>(registerJson, JsonOptions);

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth!.AccessToken);

        // Act
        var response = await _client.GetAsync("/api/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<UserDtoResponse>(json, JsonOptions);
        user!.Email.Should().Be(email);
    }

    private sealed record AuthResponseDto(string AccessToken, string RefreshToken);
    private sealed record UserDtoResponse(Guid Id, string Username, string Email, string Role);
}
