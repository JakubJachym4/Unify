using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Unify.Domain.About;

namespace Unify.Api.Controllers;

[ApiController]
[Route("api/about")]
public class AboutController : ControllerBase
{
    private readonly UniversityInformation _universityInformation;
    public AboutController(IOptions<UniversityInformation> universityInformation)
    {
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
}