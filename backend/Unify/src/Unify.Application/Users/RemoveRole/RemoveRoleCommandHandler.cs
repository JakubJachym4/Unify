using Unify.Application.Abstractions.Messaging;
using Unify.Domain.Abstractions;
using Unify.Domain.Users;

namespace Unify.Application.Users.RemoveRole;

public sealed class RemoveRoleCommandHandler : ICommandHandler<RemoveRoleCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveRoleCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task<Result<Result>> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        var role = Role.GetByName(request.Role);
        if (role == null)
        {
            return Result.Failure(UserErrors.RoleNotFound);
        }

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        if (user.Roles.Any(r => r.Id == role.Id) == false)
        {
            return Result.Success();
        }

        user.RemoveRole(role);
        _userRepository.Update(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}