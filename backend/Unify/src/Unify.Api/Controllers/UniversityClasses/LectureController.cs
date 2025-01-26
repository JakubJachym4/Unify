using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.Lectures;
using Unify.Application.Lectures.CommandsAndQueries;

namespace Unify.Api.Controllers.UniversityClasses;

[ApiController]
[Authorize]
[Route("api/lectures")]
public class LectureController : ControllerBase
{
    private readonly ISender _sender;

    public LectureController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateLecture([FromBody] CreateLectureCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> UpdateLecture(Guid id, [FromBody] UpdateLectureCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("Mismatched lecture ID.");
        }

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteLecture(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteLectureCommand(id), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetLecture(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetLectureQuery(id), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> ListLectures(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ListLecturesQuery(), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("course/{courseId:guid}")]
    public async Task<IActionResult> ListLecturesByCourse(Guid courseId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ListLecturesByCourseQuery(courseId), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("create-interval")]
    [Authorize(Roles = Roles.Lecturer)]
    public async Task<IActionResult> CreateIntervalLectures([FromBody] CreateIntervalLecturesCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }
}

