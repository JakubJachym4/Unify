using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.ClassOfferingSessions.CommandsAndQueries;
using Unify.Application.Courses.CourseHandlers;
using Unify.Application.OnlineResources.OfferingResources;
using Unify.Application.OnlineResources.OfferingResources.CommandsAndQueries;
using Unify.Application.UniversityClasses.ClassOfferings.Commands;

namespace Unify.Api.Controllers.UniversityClasses;

[ApiController]
[Authorize]
[Route("api/class-offerings")]
public class ClassOfferingController : ControllerBase
{
    private readonly ISender _sender;

    public ClassOfferingController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize(Roles = "Administrator,Lecturer")]
    public async Task<IActionResult> AddClassOffering([FromBody] AddClassOfferingCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetClassOffering(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetClassOfferingQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}/students")]
    public async Task<IActionResult> GetStudentsByClassOffering(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetStudentsByClassOfferingQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrator,Lecturer")]
    public async Task<IActionResult> UpdateClassOffering(Guid id, [FromBody] UpdateClassOfferingCommand command, CancellationToken cancellationToken)
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
    [Authorize(Roles = "Administrator,Lecturer")]
    public async Task<IActionResult> DeleteClassOffering(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteClassOfferingCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> ListClassOfferings(CancellationToken cancellationToken)
    {
        var query = new ListClassOfferingsQuery();
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/enroll")]
    [Authorize(Roles = "Student,Lecturer")]
    public async Task<IActionResult> Enroll(Guid id, CancellationToken cancellationToken)
    {
        var command = new EnrollStudentCommand(id);

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}/lecturer/{lecturerId:guid}")]
    [Authorize(Roles = "Administrator,Lecturer")]
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
    public async Task<IActionResult> GetClassOfferingsByLecturer(Guid lecturerId, CancellationToken cancellationToken)
    {
        var query = new GetClassOfferingsByLecturerQuery(lecturerId);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("{classOfferingId:guid}/resources")]
    [Authorize(Roles = "Administrator,Lecturer")]
    public async Task<IActionResult> CreateOfferingResource(Guid classOfferingId, [FromForm] CreateOfferingResourceCommand command, CancellationToken cancellationToken)
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

    [HttpPut("resources/{id:guid}")]
    [Authorize(Roles = "Administrator,Lecturer")]
    public async Task<IActionResult> UpdateOfferingResource(Guid id, [FromForm] UpdateOfferingResourceCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("Resource ID mismatch.");
        }

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpDelete("resources/{id:guid}")]
    [Authorize(Roles = "Administrator,Lecturer")]
    public async Task<IActionResult> DeleteOfferingResource(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteOfferingResourceCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }
    
    [HttpGet("resources/{id:guid}")]
    public async Task<IActionResult> GetOfferingResource(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetOfferingResourceQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
    
    [HttpGet("{id:guid}/resources")]
    public async Task<IActionResult> GetOfferingResources(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetOfferingResourcesQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

}






