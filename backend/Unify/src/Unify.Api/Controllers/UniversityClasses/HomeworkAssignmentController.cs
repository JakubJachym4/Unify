using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Unify.Application.Homework.HomeworkAssignments.CommandsAndQueries;

namespace Unify.Api.Controllers.UniversityClasses;


[ApiController]
[Authorize]
[Route("api/class-offerings/assignments")]
public class HomeworkAssignmentController : ControllerBase
{
    private readonly ISender _sender;

    public HomeworkAssignmentController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("/class-offering/{classOfferingId:guid}")]
    [Authorize(Roles = "Administrator,Lecturer")]
    public async Task<IActionResult> CreateHomeworkAssignment(Guid classOfferingId, [FromForm] CreateHomeworkAssignmentCommand command, CancellationToken cancellationToken)
    {
        if (classOfferingId != command.ClassOfferingId)
        {
            return BadRequest("Class offering ID mismatch.");
        }

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPut("/{id:guid}")]
    [Authorize(Roles = "Administrator,Lecturer")]
    public async Task<IActionResult> UpdateHomeworkAssignment(Guid id, [FromForm] UpdateHomeworkAssignmentCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("Homework assignment ID mismatch.");
        }

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpDelete("/{id:guid}")]
    [Authorize(Roles = "Administrator,Lecturer")]
    public async Task<IActionResult> DeleteHomeworkAssignment(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteHomeworkAssignmentCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }
}