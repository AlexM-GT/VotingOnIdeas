using FluentAssertions;
using VotingOnIdeas.Application.Ideas;

namespace VotingOnIdeas.Application.Tests.Validators;

public sealed class CreateIdeaCommandValidatorTests
{
    private readonly CreateIdeaCommandValidator _sut = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new CreateIdeaCommand("My Idea", "A good description", Guid.NewGuid());

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyTitle_ShouldFail()
    {
        // Arrange
        var command = new CreateIdeaCommand("", "Description", Guid.NewGuid());

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task Validate_TitleTooLong_ShouldFail()
    {
        // Arrange
        var command = new CreateIdeaCommand(new string('a', 201), "Description", Guid.NewGuid());

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task Validate_EmptyDescription_ShouldFail()
    {
        // Arrange
        var command = new CreateIdeaCommand("Title", "", Guid.NewGuid());

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }
}
