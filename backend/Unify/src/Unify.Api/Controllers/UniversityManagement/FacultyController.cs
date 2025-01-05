using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unify.Application.Abstractions.Messaging;
using Unify.Application.Faculty.Commands;
using Unify.Application.FieldsOfStudy;
using Unify.Domain.Shared;

namespace Unify.Api.Controllers.UniversityManagement;

[ApiController]
[Authorize]
[Route("api/faculties")]
public class FacultyController : ControllerBase
{
    private readonly ISender _sender;

    public FacultyController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> AddFaculty([FromBody] AddFacultyCommand command, CancellationToken cancellationToken)
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
    public async Task<IActionResult> UpdateFaculty(Guid id, [FromBody] UpdateFacultyCommand command, CancellationToken cancellationToken)
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
    public async Task<IActionResult> DeleteFaculty(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteFieldOfStudyCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> ListFaculties(CancellationToken cancellationToken)
    {
        var query = new ListFacultyQuery();
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}

