using FluentAssertions;
using VotingOnIdeas.Application.Ideas;

namespace VotingOnIdeas.Application.Tests.Validators;

public sealed class RateIdeaCommandValidatorTests
{
    private readonly RateIdeaCommandValidator _sut = new();

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public async Task Validate_ValidValue_ShouldHaveNoErrors(int value)
    {
        // Arrange
        var command = new RateIdeaCommand(Guid.NewGuid(), value, Guid.NewGuid());

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public async Task Validate_ValueOutOfRange_ShouldFail(int value)
    {
        // Arrange
        var command = new RateIdeaCommand(Guid.NewGuid(), value, Guid.NewGuid());

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Value");
    }
}
