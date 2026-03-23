using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using VotingOnIdeas.API.Tests.Infrastructure;

namespace VotingOnIdeas.API.Tests.Ideas;

[Collection("Integration")]
public sealed class IdeasIntegrationTests
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public IdeasIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<string> RegisterAndLoginAsync()
    {
        var id = Guid.NewGuid().ToString("N")[..8];
        var email = $"{id}@example.com";
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register",
            new { Username = $"user_{id}", Email = email, Password = "Password123" });
        var json = await registerResponse.Content.ReadAsStringAsync();
        var auth = JsonSerializer.Deserialize<AuthResponseDto>(json, JsonOptions);
        return auth!.AccessToken;
    }

    [Fact]
    public async Task GetIdeas_ReturnsOkWithPagedResult()
    {
        // Act
        var response = await _client.GetAsync("/api/ideas");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateIdea_Authenticated_ReturnsCreated()
    {
        // Arrange
        var token = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsJsonAsync("/api/ideas",
            new { Title = "Great Idea", Description = "This is a great idea" });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var json = await response.Content.ReadAsStringAsync();
        var idea = JsonSerializer.Deserialize<IdeaDtoResponse>(json, JsonOptions);
        idea!.Title.Should().Be("Great Idea");
    }

    [Fact]
    public async Task CreateIdea_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange — fresh client with no auth headers
        var anonClient = _factory.CreateClient();

        // Act
        var response = await anonClient.PostAsJsonAsync("/api/ideas",
            new { Title = "Idea", Description = "Desc" });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateIdea_ByOwner_ReturnsOk()
    {
        // Arrange
        var token = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync("/api/ideas",
            new { Title = "Original", Description = "Original desc" });
        var createJson = await createResponse.Content.ReadAsStringAsync();
        var idea = JsonSerializer.Deserialize<IdeaDtoResponse>(createJson, JsonOptions)!;

        // Act
        var response = await _client.PutAsJsonAsync($"/api/ideas/{idea.Id}",
            new { Title = "Updated", Description = "Updated desc" });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var updated = JsonSerializer.Deserialize<IdeaDtoResponse>(json, JsonOptions);
        updated!.Title.Should().Be("Updated");
    }

    [Fact]
    public async Task RateIdea_ValidValue_ReturnsOkWithRating()
    {
        // Arrange
        var token = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync("/api/ideas",
            new { Title = "Rate Me", Description = "Please rate this" });
        var createJson = await createResponse.Content.ReadAsStringAsync();
        var idea = JsonSerializer.Deserialize<IdeaDtoResponse>(createJson, JsonOptions)!;

        // Act
        var response = await _client.PutAsJsonAsync($"/api/ideas/{idea.Id}/rating", new { Value = 5 });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var rated = JsonSerializer.Deserialize<IdeaDtoResponse>(json, JsonOptions);
        rated!.AverageRating.Should().Be(5);
        rated.VoteCount.Should().Be(1);
    }

    [Fact]
    public async Task DeleteIdea_ByOwner_ReturnsNoContent()
    {
        // Arrange
        var token = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync("/api/ideas",
            new { Title = "Delete Me", Description = "This will be deleted" });
        var createJson = await createResponse.Content.ReadAsStringAsync();
        var idea = JsonSerializer.Deserialize<IdeaDtoResponse>(createJson, JsonOptions)!;

        // Act
        var response = await _client.DeleteAsync($"/api/ideas/{idea.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private sealed record AuthResponseDto(string AccessToken, string RefreshToken);
    private sealed record IdeaDtoResponse(Guid Id, string Title, string Description, Guid UserId, double? AverageRating, int VoteCount);
}
