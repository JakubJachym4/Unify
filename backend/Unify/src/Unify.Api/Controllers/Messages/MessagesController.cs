using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unify.Application.Messages;
using Unify.Application.Messages.ForwardMessage;
using Unify.Application.Messages.GetLastMessagesByDate;
using Unify.Application.Messages.GetLastMessagesByNumber;
using Unify.Application.Messages.GetSentMessages;
using Unify.Application.Messages.ReplyToMessage;
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
    public async Task<IActionResult> GetSendMessages(CancellationToken cancellationToken)
    {
        var query = new GetSentMessagesQuery();

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet($"get_last_by_date")]
    public async Task<IActionResult> GetMessages([FromQuery] GetLastMessagesByDateRequest request, CancellationToken cancellationToken)
    {
        var query = new GetLastMessagesByDateQuery(request.Date);

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("get_last_by_number/{number:int}")]
    public async Task<IActionResult> GetMessages(int number, CancellationToken cancellationToken)
    {
        var query = new GetLastMessagesByNumberQuery(number);

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("reply")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ReplyToMessage([FromForm] ReplyToMessageRequest request, CancellationToken cancellationToken)
    {
        var command = new ReplyToMessageCommand(
            request.RespondingToId,
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

    [HttpPost("forward")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ForwardMessage([FromForm] ForwardMessageRequest request, CancellationToken cancellationToken)
    {
        var command = new ForwardMessageCommand(
            request.OriginalMessageId,
            request.NewRecipientsIds);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

}