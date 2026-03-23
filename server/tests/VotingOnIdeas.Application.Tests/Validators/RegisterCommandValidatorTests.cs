using FluentAssertions;
using VotingOnIdeas.Application.Auth;

namespace VotingOnIdeas.Application.Tests.Validators;

public sealed class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _sut = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new RegisterCommand("johndoe", "john@example.com", "Password123");

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyUsername_ShouldFail()
    {
        // Arrange
        var command = new RegisterCommand("", "john@example.com", "Password123");

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Username");
    }

    [Fact]
    public async Task Validate_InvalidEmail_ShouldFail()
    {
        // Arrange
        var command = new RegisterCommand("johndoe", "not-an-email", "Password123");

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task Validate_ShortPassword_ShouldFail()
    {
        // Arrange
        var command = new RegisterCommand("johndoe", "john@example.com", "short");

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }
}
