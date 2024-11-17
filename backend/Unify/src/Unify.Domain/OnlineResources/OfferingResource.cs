using Unify.Domain.Messages;
using Unify.Domain.Shared;
using Unify.Domain.UniversityClasses;
using Unify.Domain.UniversityCore;

namespace Unify.Domain.OnlineResources;

public sealed class OfferingResource : OnlineResource
{
    public ClassOffering ClassOffering { get; private set; }

    public OfferingResource(ClassOffering classOffering, Title title, string description) : base(Guid.NewGuid(), title, description)
    {
        ClassOffering = classOffering;
    }
}