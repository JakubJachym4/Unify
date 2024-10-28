using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unify.Application.Messages;
using Unify.Application.Messages.GetSentMessages;
using Unify.Application.Messages.SendMessage;

namespace Unify.Api.Controllers.Messages;

[ApiController]
[Authorize]
[Route("api/messages")]
public class MessagesController : ControllerBase
{
    private readonly ISender _sender;

    public MessagesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("send")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> SendMessage(
        [FromForm] SendMessageRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SendMessageCommand(
            request.SenderId,
            request.Title,
            request.Content,
            request.RecipientsIds,
            request.Attachments);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("get_sent")]
    public async Task<IActionResult> GetSendMessages(GetSentMessagesRequest request, CancellationToken cancellationToken)
    {
        var query = new GetSentMessagesQuery(request.UserId);

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}