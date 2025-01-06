using Microsoft.EntityFrameworkCore;
using Unify.Domain.Shared;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityClasses.Abstractions;

namespace Unify.Infrastructure.Repositories.UniversityClasses;

public class ClassOfferingRepository : Repository<ClassOffering>, IClassOfferingRepository
{
    public ClassOfferingRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

}