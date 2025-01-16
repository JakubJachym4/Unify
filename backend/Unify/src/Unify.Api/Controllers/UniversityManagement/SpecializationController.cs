

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.Specializations;

namespace Unify.Api.Controllers.UniversityManagement;

[ApiController]
[Route("api/specializations")]
public class SpecializationController : ControllerBase
{
    private readonly ISender _sender;

    public SpecializationController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> AddSpecialization([FromBody] AddSpecializationCommand command, CancellationToken cancellationToken)
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
    public async Task<IActionResult> UpdateSpecialization(Guid id, [FromBody] UpdateSpecializationCommand command, CancellationToken cancellationToken)
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
    public async Task<IActionResult> DeleteSpecialization(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteSpecializationCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> ListSpecializations(CancellationToken cancellationToken)
    {
        var query = new ListSpecializationsQuery();
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("assign-student")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> AssignStudentToSpecialization([FromBody] AssignStudentToSpecializationCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }
    [HttpDelete("unassign-student")]
    [Authorize]
    public async Task<IActionResult> UnassignStudentFromSpecialization([FromBody] UnassignStudentFromSpecializationCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpGet("{id:guid}/students")]
    public async Task<IActionResult> GetSpecializationStudents(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetSpecializationStudents(id);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}
