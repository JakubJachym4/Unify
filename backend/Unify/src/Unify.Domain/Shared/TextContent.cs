using System.ComponentModel.DataAnnotations.Schema;

namespace Unify.Domain.Shared;

[NotMapped]
public record TextContent(string Value);