namespace Unify.Application.Abstractions.Email;

public interface IEmailService
{
    Task SendAsync(Unify.Domain.Users.Email recipient, string subject, string body);
}