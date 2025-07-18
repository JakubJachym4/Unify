﻿using Unify.Domain.Abstractions;
using Unify.Domain.Users.Events;

namespace Unify.Domain.Users;

public sealed class User : Entity
{
    private readonly List<Role> _roles = new();

    private User(Guid id, FirstName firstName, LastName lastName, Email email)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    private User()
    {
    }

    public FirstName FirstName { get; private set; }

    public LastName LastName { get; private set; }

    public Email Email { get; private set; }

    public string IdentityId { get; private set; } = string.Empty;

    public Guid? StudentGroupId { get; private set; }
    public Guid? SpecializationId { get; private set; }

    public IReadOnlyCollection<Role> Roles => _roles.ToList();

    public static User Create(FirstName firstName, LastName lastName, Email email)
    {
        var user = new User(Guid.NewGuid(), firstName, lastName, email);

        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));

        user._roles.Add(Role.Registered);

        return user;
    }

    public void AddRole(Role role)
    {
        if (_roles.Any(x => x.Id == role.Id))
        {
            return;
        }

        _roles.Add(role);
    }

    public void RemoveRole(Role role)
    {
        if (_roles.Any(x => x.Id == role.Id) == false)
        {
            return;
        }

        _roles.Remove(_roles.Single(r => r.Id == role.Id));
    }

    public void SetIdentityId(string identityId)
    {
        IdentityId = identityId;
    }
}