using MediatR;
using Unify.Application.Abstractions.Authentication;
using Unify.Domain.Abstractions;

namespace Unify.Application.Users.LogOutUser;

public sealed class LogOutUserCommandHandler : IRequestHandler<LogOutUserCommand, Result>
{
    private readonly IJwtService _jwtService;

    public LogOutUserCommandHandler(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    public async Task<Result> Handle(LogOutUserCommand request, CancellationToken cancellationToken)
    {
       return await _jwtService.LogoutAsync(request.Token, cancellationToken);
    }
}