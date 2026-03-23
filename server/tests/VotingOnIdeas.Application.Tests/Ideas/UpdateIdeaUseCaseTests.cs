using FluentAssertions;
using NSubstitute;
using VotingOnIdeas.Application.Exceptions;
using VotingOnIdeas.Application.Ideas;
using VotingOnIdeas.Domain.Constants;
using VotingOnIdeas.Domain.Entities;
using VotingOnIdeas.Domain.Interfaces;

namespace VotingOnIdeas.Application.Tests.Ideas;

public sealed class UpdateIdeaUseCaseTests
{
    private readonly IIdeaRepository _ideaRepository = Substitute.For<IIdeaRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdateIdeaUseCase _sut;

    public UpdateIdeaUseCaseTests()
    {
        _sut = new UpdateIdeaUseCase(_ideaRepository, _unitOfWork, new UpdateIdeaCommandValidator());
    }

    [Fact]
    public async Task ExecuteAsync_OwnerUpdating_ReturnsUpdatedIdeaDto()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var idea = Idea.Create("Original", "Desc", ownerId);
        _ideaRepository.GetByIdAsync(idea.Id, Arg.Any<CancellationToken>()).Returns(idea);

        var command = new UpdateIdeaCommand(idea.Id, "Updated Title", "Updated Desc", ownerId, UserRole.User);

        // Act
        var result = await _sut.ExecuteAsync(command);

        // Assert
        result.Title.Should().Be("Updated Title");
        result.Description.Should().Be("Updated Desc");
        _ideaRepository.Received().Update(idea);
        await _unitOfWork.Received().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_AdminUpdatingOthersIdea_ReturnsUpdatedIdeaDto()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var idea = Idea.Create("Original", "Desc", ownerId);
        _ideaRepository.GetByIdAsync(idea.Id, Arg.Any<CancellationToken>()).Returns(idea);

        var command = new UpdateIdeaCommand(idea.Id, "Admin Edit", "Admin Desc", adminId, UserRole.Admin);

        // Act
        var result = await _sut.ExecuteAsync(command);

        // Assert
        result.Title.Should().Be("Admin Edit");
    }

    [Fact]
    public async Task ExecuteAsync_NonOwnerUpdating_ThrowsUnauthorizedException()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var idea = Idea.Create("Original", "Desc", ownerId);
        _ideaRepository.GetByIdAsync(idea.Id, Arg.Any<CancellationToken>()).Returns(idea);

        var command = new UpdateIdeaCommand(idea.Id, "Hacked", "Hacked", otherUserId, UserRole.User);

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

        var command = new UpdateIdeaCommand(ideaId, "Title", "Desc", Guid.NewGuid(), UserRole.User);

        // Act
        var act = () => _sut.ExecuteAsync(command);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
