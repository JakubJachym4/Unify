using System.ComponentModel.DataAnnotations.Schema;

namespace Unify.Domain.Shared;

[NotMapped]
public sealed record Description(string Value);