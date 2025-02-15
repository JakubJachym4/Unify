using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unify.Application.Homework.HomeworkSubmissions.CommandsAndQueries;

namespace Unify.Api.Controllers.UniversityClasses;


[Authorize]
[ApiController]
[Route("api/assignments")]
public class HomeworkSubmissionsController : ControllerBase
{
    private readonly ISender _sender;

    public HomeworkSubmissionsController(ISender sender)
    {
        _sender = sender;
    }


    [HttpPost("{homeworkAssignmentId:guid}/submit")]
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

    [HttpPut("submissions/{id:guid}")]
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

    [HttpDelete("submissions/{id:guid}")]
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

    [HttpGet("submissions/{id:guid}")]
    public async Task<IActionResult> GetHomeworkSubmission(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetHomeworkSubmissionQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("{homeworkAssignmentId:guid}/submissions")]
    public async Task<IActionResult> GetHomeworkSubmissionsByAssignment(Guid homeworkAssignmentId, CancellationToken cancellationToken)
    {
        var query = new GetHomeworkSubmissionsByAssignmentQuery(homeworkAssignmentId);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("student/{studentId:guid}/submissions")]
    public async Task<IActionResult> GetHomeworkSubmissionsByStudent(Guid studentId, CancellationToken cancellationToken)
    {
        var query = new GetHomeworkSubmissionsByStudentQuery(studentId);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}