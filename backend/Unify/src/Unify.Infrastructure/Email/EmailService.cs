using Unify.Application.Abstractions.Email;

namespace Unify.Infrastructure.Email;

internal sealed class EmailService : IEmailService
{
    public Task SendAsync(Unify.Domain.Users.Email recipient, string subject, string body)
    {
        return Task.CompletedTask;
    }
}