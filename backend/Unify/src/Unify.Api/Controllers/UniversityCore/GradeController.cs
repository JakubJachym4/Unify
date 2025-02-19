using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unify.Application.Grades.CommandsAndQueries;
using Unify.Application.Grades.Handler;

namespace Unify.Api.Controllers.UniversityCore;


[ApiController]
[Authorize]
[Route("api/grades")]
public class GradeController : ControllerBase
{
    private ISender _sender;

    public GradeController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGrade(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetGradeQuery(id), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/marks")]
    public async Task<IActionResult> CreateMark(Guid id, [FromBody] CreateMarkRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreateMarkCommand(id, request.Title, request.Score, request.MaxScore), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpPut("{id:guid}/award={awarded:bool}")]
    [Authorize(Roles="Lecturer")]
    public async Task<IActionResult> AwardGrade(Guid id, bool awarded, CancellationToken cancellationToken)
    {
        var command = new AwardGradeCommand(id, awarded);
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        return Ok();
    }

}



public record CreateMarkRequest(string Title, decimal Score, decimal MaxScore);
