using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingOnIdeas.Application.Common;
using VotingOnIdeas.Application.Ideas;

namespace VotingOnIdeas.API.Controllers;

[Route("api/[controller]")]
public sealed class IdeasController : ApiControllerBase
{
    private readonly GetIdeasUseCase _getIdeas;
    private readonly GetIdeaByIdUseCase _getIdeaById;
    private readonly CreateIdeaUseCase _createIdea;
    private readonly UpdateIdeaUseCase _updateIdea;
    private readonly DeleteIdeaUseCase _deleteIdea;
    private readonly RateIdeaUseCase _rateIdea;

    public IdeasController(
        GetIdeasUseCase getIdeas,
        GetIdeaByIdUseCase getIdeaById,
        CreateIdeaUseCase createIdea,
        UpdateIdeaUseCase updateIdea,
        DeleteIdeaUseCase deleteIdea,
        RateIdeaUseCase rateIdea)
    {
        _getIdeas = getIdeas;
        _getIdeaById = getIdeaById;
        _createIdea = createIdea;
        _updateIdea = updateIdea;
        _deleteIdea = deleteIdea;
        _rateIdea = rateIdea;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<IdeaDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _getIdeas.ExecuteAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<IdeaDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var idea = await _getIdeaById.ExecuteAsync(id, cancellationToken);
        return Ok(idea);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<IdeaDto>> Create(
        [FromBody] CreateIdeaBody body,
        CancellationToken cancellationToken)
    {
        var command = new CreateIdeaCommand(body.Title, body.Description, GetCurrentUserId());
        var idea = await _createIdea.ExecuteAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = idea.Id }, idea);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<IdeaDto>> Update(
        Guid id,
        [FromBody] UpdateIdeaBody body,
        CancellationToken cancellationToken)
    {
        var command = new UpdateIdeaCommand(id, body.Title, body.Description, GetCurrentUserId(), GetCurrentUserRole());
        var idea = await _updateIdea.ExecuteAsync(command, cancellationToken);
        return Ok(idea);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteIdeaCommand(id, GetCurrentUserId(), GetCurrentUserRole());
        await _deleteIdea.ExecuteAsync(command, cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpPut("{id:guid}/rating")]
    public async Task<ActionResult<IdeaDto>> Rate(
        Guid id,
        [FromBody] RateIdeaBody body,
        CancellationToken cancellationToken)
    {
        var command = new RateIdeaCommand(id, body.Value, GetCurrentUserId());
        var idea = await _rateIdea.ExecuteAsync(command, cancellationToken);
        return Ok(idea);
    }
}

public record CreateIdeaBody(string Title, string Description);
public record UpdateIdeaBody(string Title, string Description);
public record RateIdeaBody(int Value);
