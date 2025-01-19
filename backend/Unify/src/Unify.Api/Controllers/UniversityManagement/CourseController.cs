using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.Courses.Commands;
using Unify.Application.Courses.CourseHandlers;

namespace Unify.Api.Controllers.UniversityManagement;

[ApiController]
[Authorize]
[Route("api/courses")]
public class CourseController : ControllerBase
{
    private readonly ISender _sender;

    public CourseController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCourse(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCourseQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> AddCourse([FromBody] AddCourseCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrator,Lecturer")]
    public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch.");
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
    public async Task<IActionResult> DeleteCourse(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCourseCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> ListCourses(CancellationToken cancellationToken)
    {
        var query = new ListCoursesQuery();
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("specialization/{id:guid}")]
    public async Task<IActionResult> ListCoursesBySpecialization(Guid id, CancellationToken cancellationToken)
    {
        var query = new ListCoursesBySpecializationQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}/lecturer/{lecturerId:guid}")]
    [Authorize(Roles = Roles.Administrator)]
    public async Task<IActionResult> AssignLecturer(Guid id, Guid lecturerId, CancellationToken cancellationToken)
    {
        var command = new AssignLecturerCommand(id, lecturerId);
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpGet("lecturer/{lecturerId:guid}")]
    [Authorize(Roles = Roles.Lecturer)]
    public async Task<IActionResult> GetCoursesByLecturer(Guid lecturerId, CancellationToken cancellationToken)
    {
        var query = new GetCoursesByLecturerQuery(lecturerId);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}




