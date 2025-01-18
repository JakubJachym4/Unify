using Unify.Application.Abstractions.Messaging;

namespace Unify.Application.StudentGroups.CommandsAndQueries;

public record GetGroupForUserQuery(Guid Id) : IQuery<StudentGroupResponse>;

public record GetGroupForSpecializationQuery(Guid Id) : IQuery<List<StudentGroupResponse>>;

public record GetAllGroupsQuery : IQuery<List<StudentGroupResponse>>;

public sealed record CreateStudentGroupsForSpecializationCommand(
    string Name,
    Guid SpecializationId,
    int StudyYear,
    int Semester,
    string Term,
    int CombinedSize,
    int MaxGroupSize) : ICommand;

public sealed record CreateStudentGroupCommand(
    string Name,
    Guid SpecializationId,
    int StudyYear,
    int Semester,
    string Term,
    int MaxGroupSize) : ICommand;

public record JoinGroupCommand(Guid Id) : ICommand;

public record MoveUserToGroupCommand(Guid UserId, Guid? GroupId) : ICommand;

public record UpdateStudentGroupCommand(
    Guid Id,
    string Name,
    Guid? SpecializationId,
    int StudyYear,
    int Semester,
    string Term,
    int MaxGroupSize) : ICommand;


public record DeleteStudentGroupCommand(Guid Id) : ICommand;

public record AutoAssignStudentsToGroupsCommand(Guid Id) : ICommand;