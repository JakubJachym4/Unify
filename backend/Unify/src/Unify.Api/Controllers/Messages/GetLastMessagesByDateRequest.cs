namespace Unify.Api.Controllers.Messages;

public sealed record GetLastMessagesByDateRequest(
    DateOnly Date);