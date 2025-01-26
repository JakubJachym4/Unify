using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unify.Application.Homework.HomeworkSubmissions.CommandsAndQueries;

namespace Unify.Api.Controllers.UniversityClasses;


[Authorize]
[ApiController]
[Route("api/class-offerings/submissions")]
public class HomeworkSubmissionsController : ControllerBase
{
    private readonly ISender _sender;

    public HomeworkSubmissionsController(ISender sender)
    {
        _sender = sender;
    }


    [HttpPost("assignment/{homeworkAssignmentId:guid}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> CreateHomeworkSubmission(Guid homeworkAssignmentId, [FromForm] CreateHomeworkSubmissionCommand command, CancellationToken cancellationToken)
    {
        if (homeworkAssignmentId != command.HomeworkAssignmentId)
        {
            return BadRequest("Homework assignment ID mismatch.");
        }

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> UpdateHomeworkSubmission(Guid id, [FromForm] UpdateHomeworkSubmissionCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("Submission ID mismatch.");
        }

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> DeleteHomeworkSubmission(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteHomeworkSubmissionCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }
}