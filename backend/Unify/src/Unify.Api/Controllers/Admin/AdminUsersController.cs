using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unify.Application.Users.AddRole;
using Unify.Application.Users.RemoveRole;


namespace Unify.Api.Controllers.Admin
{
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class AdminUsersController : ControllerBase
    {
        private readonly ISender _sender;

        public AdminUsersController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole([FromBody] AddRoleRequest request, CancellationToken cancellationToken)
        {
            var command = new AddRoleCommand(request.UserId, request.Role);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok();
        }

        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveRole([FromBody] DeleteRoleRequest request, CancellationToken cancellationToken)
        {
            var command = new RemoveRoleCommand(request.UserId, request.Role);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok();
        }
    }
}