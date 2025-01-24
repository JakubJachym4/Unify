using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unify.Application.ClassOfferingSessions.CommandsAndQueries;


namespace Unify.Api.Controllers.UniversityCore;

[ApiController]
[Authorize]
[Route("api/class-offering-sessions")]
public class ClassOfferingSessionController : ControllerBase
{
    private readonly ISender _sender;

    public ClassOfferingSessionController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateClassOfferingSession([FromBody] CreateClassOfferingSessionCommand command, CancellationToken cancellationToken)
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
    public async Task<IActionResult> UpdateClassOfferingSession(Guid id, [FromBody] UpdateClassOfferingSessionCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("Mismatched session ID.");
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
    public async Task<IActionResult> DeleteClassOfferingSession(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteClassOfferingSessionCommand(id), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetClassOfferingSession(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetClassOfferingSessionQuery(id), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> ListClassOfferingSessions(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ListClassOfferingSessionsQuery(), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
    [HttpGet("class-offering/{classOfferingId:guid}")]
    public async Task<IActionResult> GetSessionByClassOffering(Guid classOfferingId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetSessionByClassOfferingQuery(classOfferingId), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("lecturer/{lecturerId:guid}")]
    public async Task<IActionResult> GetSessionByLecturer(Guid lecturerId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetSessionByLecturerQuery(lecturerId), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("student/{studentId:guid}")]
    public async Task<IActionResult> GetSessionByStudent(Guid studentId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetSessionByStudentQuery(studentId), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }


}

