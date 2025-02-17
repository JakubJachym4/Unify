using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.Homework.HomeworkAssignments.CommandsAndQueries;

namespace Unify.Api.Controllers.UniversityClasses;


[ApiController]
[Authorize]
[Route("api/assignments")]
public class HomeworkAssignmentController : ControllerBase
{
    private readonly ISender _sender;

    public HomeworkAssignmentController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetHomeworkAssignment(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetHomeworkAssignmentQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("class-offering/{classOfferingId:guid}")]
    public async Task<IActionResult> GetByClassOffering(Guid classOfferingId, CancellationToken cancellationToken)
    {
        var query = new GetHomeworkAssignmentsByClassOfferingQuery(classOfferingId);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("student/{studentId:guid}")]
    public async Task<IActionResult> GetByStudent(Guid studentId, CancellationToken cancellationToken)
    {
        var query = new GetHomeworkAssignmentsByStudentQuery(studentId);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}/lock={locked:bool}")]
    [Authorize(Roles="Lecturer")]
    public async Task<IActionResult> LockHomeworkAssignment(Guid id, bool locked, CancellationToken cancellationToken)
    {
        var command = new LockHomeworkAssignmentCommand(id, locked);
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpPost("class-offering/{classOfferingId:guid}")]
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

    [HttpPut("{id:guid}")]
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

    [HttpDelete("{id:guid}")]
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

    [HttpPost("{assignmentId:guid}/submissions/{submissionId:guid}/grade")]
    [Authorize(Roles = "Lecturer")]
    public async Task<IActionResult> GradeHomeworkSubmission(Guid assignmentId, Guid submissionId, [FromBody] GradeHomeworkSubmissionCommand command, CancellationToken cancellationToken)
    {
        if (assignmentId != command.AssignmentId || submissionId != command.SubmissionId)
        {
            return BadRequest("Mismatched assignment or submission ID.");
        }

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }
}







