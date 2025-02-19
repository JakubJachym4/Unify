namespace Unify.Application.About;


public record StudentInformationResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string StudentGroup,
    string FiledOfStudy,
    string Specialization,
    int Semester,
    string Term,
    string StudyYear,
    string Faculty);