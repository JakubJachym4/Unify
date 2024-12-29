using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unify.Application.Messages.GetNonExpiredInformationMessages;
using Unify.Application.Messages.SendInformationMessage;

namespace Unify.Api.Controllers.Messages.InformationMessages;


[ApiController]
[Authorize]
[Route("api/notifications")]
public class InformationMessagesController : ControllerBase
{
    private readonly ISender _sender;

    public InformationMessagesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("send")]
    [Authorize(Roles = "Administrator,Lecturer")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> SendMessage([FromForm] SendInformationMessageRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SendInformationMessageCommand(
            request.Title,
            request.Content,
            request.Severity,
            request.ExpirationDate,
            request.RecipientsIds,
            request.Attachments);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetMessages(CancellationToken cancellationToken)
    {
        var query = new GetNonExpiredInformationMessagesQuery();

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}