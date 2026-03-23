using FluentAssertions;
using NSubstitute;
using VotingOnIdeas.Application.Exceptions;
using VotingOnIdeas.Application.Ideas;
using VotingOnIdeas.Domain.Constants;
using VotingOnIdeas.Domain.Entities;
using VotingOnIdeas.Domain.Interfaces;

namespace VotingOnIdeas.Application.Tests.Ideas;

public sealed class DeleteIdeaUseCaseTests
{
    private readonly IIdeaRepository _ideaRepository = Substitute.For<IIdeaRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly DeleteIdeaUseCase _sut;

    public DeleteIdeaUseCaseTests()
    {
        _sut = new DeleteIdeaUseCase(_ideaRepository, _unitOfWork);
    }

    [Fact]
    public async Task ExecuteAsync_OwnerDeleting_RemovesIdea()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var idea = Idea.Create("Title", "Desc", ownerId);
        _ideaRepository.GetByIdAsync(idea.Id, Arg.Any<CancellationToken>()).Returns(idea);

        var command = new DeleteIdeaCommand(idea.Id, ownerId, UserRole.User);

        // Act
        await _sut.ExecuteAsync(command);

        // Assert
        _ideaRepository.Received().Remove(idea);
        await _unitOfWork.Received().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_AdminDeletingOthersIdea_RemovesIdea()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var idea = Idea.Create("Title", "Desc", ownerId);
        _ideaRepository.GetByIdAsync(idea.Id, Arg.Any<CancellationToken>()).Returns(idea);

        var command = new DeleteIdeaCommand(idea.Id, adminId, UserRole.Admin);

        // Act
        await _sut.ExecuteAsync(command);

        // Assert
        _ideaRepository.Received().Remove(idea);
    }

    [Fact]
    public async Task ExecuteAsync_NonOwnerDeleting_ThrowsUnauthorizedException()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var idea = Idea.Create("Title", "Desc", ownerId);
        _ideaRepository.GetByIdAsync(idea.Id, Arg.Any<CancellationToken>()).Returns(idea);

        var command = new DeleteIdeaCommand(idea.Id, otherUserId, UserRole.User);

        // Act
        var act = () => _sut.ExecuteAsync(command);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task ExecuteAsync_IdeaNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var ideaId = Guid.NewGuid();
        _ideaRepository.GetByIdAsync(ideaId, Arg.Any<CancellationToken>()).Returns((Idea?)null);

        var command = new DeleteIdeaCommand(ideaId, Guid.NewGuid(), UserRole.User);

        // Act
        var act = () => _sut.ExecuteAsync(command);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
