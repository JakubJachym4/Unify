using Unify.Domain.Abstractions;

namespace Unify.Application.Abstractions.Authentication;

public interface IJwtService
{
    Task<Result<string>> GetAccessTokenAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<Result> LogoutAsync(string token, CancellationToken cancellationToken = default);
}