using FluentAssertions;
using NSubstitute;
using VotingOnIdeas.Application.Exceptions;
using VotingOnIdeas.Application.Ideas;
using VotingOnIdeas.Domain.Entities;
using VotingOnIdeas.Domain.Interfaces;

namespace VotingOnIdeas.Application.Tests.Ideas;

public sealed class RateIdeaUseCaseTests
{
    private readonly IIdeaRepository _ideaRepository = Substitute.For<IIdeaRepository>();
    private readonly IVoteRepository _voteRepository = Substitute.For<IVoteRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly RateIdeaUseCase _sut;

    public RateIdeaUseCaseTests()
    {
        _sut = new RateIdeaUseCase(_ideaRepository, _voteRepository, _unitOfWork, new RateIdeaCommandValidator());
    }

    [Fact]
    public async Task ExecuteAsync_NoExistingVote_CreatesNewVote()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var idea = Idea.Create("Title", "Desc", userId);
        _ideaRepository.GetByIdAsync(idea.Id, Arg.Any<CancellationToken>()).Returns(idea);
        _voteRepository.GetByIdeaAndUserAsync(idea.Id, userId, Arg.Any<CancellationToken>())
            .Returns((Vote?)null);

        var command = new RateIdeaCommand(idea.Id, 4, userId);

        // Act
        await _sut.ExecuteAsync(command);

        // Assert
        await _voteRepository.Received().AddAsync(Arg.Any<Vote>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_ExistingVote_UpdatesVote()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var idea = Idea.Create("Title", "Desc", userId);
        var existingVote = Vote.Create(idea.Id, userId, 2);
        _ideaRepository.GetByIdAsync(idea.Id, Arg.Any<CancellationToken>()).Returns(idea);
        _voteRepository.GetByIdeaAndUserAsync(idea.Id, userId, Arg.Any<CancellationToken>())
            .Returns(existingVote);

        var command = new RateIdeaCommand(idea.Id, 5, userId);

        // Act
        await _sut.ExecuteAsync(command);

        // Assert
        existingVote.Value.Should().Be(5);
        _voteRepository.Received().Update(existingVote);
        await _unitOfWork.Received().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_IdeaNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var ideaId = Guid.NewGuid();
        _ideaRepository.GetByIdAsync(ideaId, Arg.Any<CancellationToken>()).Returns((Idea?)null);

        var command = new RateIdeaCommand(ideaId, 3, Guid.NewGuid());

        // Act
        var act = () => _sut.ExecuteAsync(command);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
