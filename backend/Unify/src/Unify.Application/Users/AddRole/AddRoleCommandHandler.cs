using Unify.Application.Abstractions.Data;
using Unify.Application.Abstractions.Messaging;
using Dapper;
using Unify.Application.Users.AddRole;
using Unify.Domain.Abstractions;
using Unify.Domain.Users;

public class AddRoleCommandHandler : ICommandHandler<AddRoleCommand>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddRoleCommandHandler(ISqlConnectionFactory sqlConnectionFactory, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddRoleCommand request, CancellationToken cancellationToken)
    {
        if(Role.All.Any(role => role.Name == request.Role) == false){
        {
            return Result.Failure(UserErrors.RoleNotFound);
        }}

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        if(user.Roles.Any(role => role.Name == request.Role)){
        {
            return Result.Success();
        }}

        user.AddRole(Role.GetByName(request.Role));

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}