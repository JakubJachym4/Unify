using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unify.Application.Abstractions.Messaging;
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
    public async Task<IActionResult> CreateStudentGroupsForSpecialization([FromBody] CreateStudentGroupsForSpecializationCommand command, CancellationToken cancellationToken)
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

    [Authorize(Roles = Roles.Student)]
    [HttpPost("join/{id:guid}")]
    public async Task<IActionResult> JoinGroup(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new JoinGroupCommand(id), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [Authorize(Roles = Roles.Administrator)]
    [HttpPut("user/{id:guid}")]
    public async Task<IActionResult> MoveUserToGroup(Guid id, MoveUserToGroupRequest request, CancellationToken cancellationToken)
    {
        if(id != request.UserId)
        {
            return BadRequest("Id in the path and in the body do not match");
        }

        var result = await _sender.Send(new MoveUserToGroupCommand(request.UserId, request.GroupId), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [Authorize(Roles = Roles.Administrator)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateGroup(Guid id, UpdateStudentGroupRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateStudentGroupCommand(id, request.Name, request.SpecializationId, request.StudyYear, request.Semester, request.Term, request.MaxGroupSize);

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [Authorize(Roles = Roles.Administrator)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteGroup(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteStudentGroupCommand(id), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [Authorize(Roles = Roles.Administrator)]
    [HttpPost("specialization/{id:guid}/auto-assign")]
    public async Task<IActionResult> AutoAssignStudentsToGroups(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AutoAssignStudentsToGroupsCommand(id), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

}



public record MoveUserToGroupRequest(Guid UserId, Guid? GroupId);
public record UpdateStudentGroupRequest(string Name, Guid? SpecializationId, int StudyYear, int Semester, string Term, int MaxGroupSize);
