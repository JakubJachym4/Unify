namespace Unify.Api.Controllers.Messages;

public sealed record GetLastMessagesByNumberRequest(
    int NumberOfMessages);