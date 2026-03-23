using FluentAssertions;
using NSubstitute;
using VotingOnIdeas.Application.Exceptions;
using VotingOnIdeas.Application.Ideas;
using VotingOnIdeas.Domain.Entities;
using VotingOnIdeas.Domain.Interfaces;

namespace VotingOnIdeas.Application.Tests.Ideas;

public sealed class CreateIdeaUseCaseTests
{
    private readonly IIdeaRepository _ideaRepository = Substitute.For<IIdeaRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CreateIdeaUseCase _sut;

    public CreateIdeaUseCaseTests()
    {
        _sut = new CreateIdeaUseCase(_ideaRepository, _unitOfWork, new CreateIdeaCommandValidator());
    }

    [Fact]
    public async Task ExecuteAsync_ValidCommand_ReturnsIdeaDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateIdeaCommand("Test Idea", "A description", userId);

        _ideaRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(x => Idea.Create("Test Idea", "A description", userId));

        // Act
        var result = await _sut.ExecuteAsync(command);

        // Assert
        result.Title.Should().Be("Test Idea");
        result.Description.Should().Be("A description");
        result.UserId.Should().Be(userId);
        await _ideaRepository.Received().AddAsync(Arg.Any<Idea>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_InvalidCommand_ThrowsValidationException()
    {
        // Arrange
        var command = new CreateIdeaCommand("", "", Guid.NewGuid());

        // Act
        var act = () => _sut.ExecuteAsync(command);

        // Assert
        await act.Should().ThrowAsync<VotingOnIdeas.Application.Exceptions.ValidationException>();
    }
}
