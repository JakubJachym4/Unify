using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unify.Application.StudentGroups.CommandsAndQueries;


namespace Unify.Api.Controllers.UniversityCore;

[ApiController]
[Authorize]
[Route("api/student-groups")]
public class StudentGroupController : ControllerBase
{
    private readonly ISender _sender;

    public StudentGroupController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("create-multiple-for-specialization")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateStudentGroupForSpecialization([FromBody] CreateStudentGroupsForSpecializationCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpPost("create")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateStudentGroup([FromBody] CreateStudentGroupCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGroups(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllGroupsQuery(), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }


    [HttpGet("specialization/{id:guid}")]
    public async Task<IActionResult> GetGroupsForSpecialization(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetGroupForSpecializationQuery(id), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("user/{id:guid}")]
    public async Task<IActionResult> GetGroupForUser(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetGroupForUserQuery(id), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}





