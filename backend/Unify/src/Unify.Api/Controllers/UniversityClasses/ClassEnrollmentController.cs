using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unify.Application.ClassEnrollment.CommandsAndQueries;
using Unify.Domain.UniversityClasses;


namespace Unify.Api.Controllers.UniversityCore;

[ApiController]
[Authorize]
[Route("api/class-enrollments")]
public class ClassEnrollmentController : ControllerBase
{
    private readonly ISender _sender;

    public ClassEnrollmentController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("enroll")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> EnrollStudent([FromBody] EnrollStudentCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpPost("cancelEnrollment")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CancelEnrollment([FromBody] CancelEnrollmentStudentCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpGet("class-offering/{classOfferingId:guid}")]
    public async Task<IActionResult> GetEnrollmentsForClassOffering(Guid classOfferingId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetEnrollmentsForClassOfferingQuery(classOfferingId), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("student/{studentId:guid}")]
    public async Task<IActionResult> GetEnrollmentsForStudent(Guid studentId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetEnrollmentsForStudentQuery(studentId), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}



