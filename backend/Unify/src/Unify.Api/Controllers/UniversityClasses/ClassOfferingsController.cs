using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unify.Application.UniversityClasses.ClassOfferings.Commands;

namespace Unify.Api.Controllers.UniversityClasses;

[ApiController]
[Authorize]
[Route("api/classes/classofferings")]
public class ClassOfferingController : ControllerBase
{
    private readonly ISender _sender;

    public ClassOfferingController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("add")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> AddClassOffering([FromBody] AddClassOfferingCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPut("update/{id:guid}")]
    [Authorize(Roles = "Administrator")]
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

    [HttpDelete("delete/{id:guid}")]
    [Authorize(Roles = "Administrator")]
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

    [HttpPost("enroll")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Enroll([FromBody] EnrollStudentCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}