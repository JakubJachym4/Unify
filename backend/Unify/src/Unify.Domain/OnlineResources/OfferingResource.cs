using Unify.Domain.Messages;
using Unify.Domain.Shared;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityCore;
using Guid = System.Guid;

namespace Unify.Domain.OnlineResources;

public sealed class OfferingResource : OnlineResource
{

    private OfferingResource() { }
    public OfferingResource(ClassOffering classOffering, Title title, Description description) : base(Guid.NewGuid(), title, description)
    {
        ClassOfferingId = classOffering.Id;
    }

    public Guid ClassOfferingId { get; private set; }
}