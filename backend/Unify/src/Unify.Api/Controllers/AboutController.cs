using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Unify.Application.About;
using Unify.Application.Abstractions.Messaging;
using Unify.Domain.About;

namespace Unify.Api.Controllers;

[ApiController]
[Route("api/about")]
public class AboutController : ControllerBase
{
    private readonly UniversityInformation _universityInformation;
    private readonly ISender _sender;
    public AboutController(IOptions<UniversityInformation> universityInformation, ISender sender)
    {
        _sender = sender;
        _universityInformation = universityInformation.Value;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult GetUniversityInformation()
    {
        return Ok(new
        {
            Name = _universityInformation.Name,
            Abbreviation = _universityInformation.Abbreviation
        });
    }

    [Authorize(Roles = Roles.Student)]
    [HttpGet("me")]
    public async Task<IActionResult> GetStudentInformation()
    {
        var command = new GetStudentInformationQuery();
        var result = await _sender.Send(command);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}

