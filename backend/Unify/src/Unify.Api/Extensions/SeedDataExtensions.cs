using Bogus;
using Unify.Application.Abstractions.Data;
using Dapper;
using Unify.Domain.Abstractions;
using Unify.Domain.Users;

namespace Unify.Api.Extensions;

public static class SeedDataExtensions
{
    public static void SeedData(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
        using var connection = sqlConnectionFactory.CreateConnection();

        var faker = new Faker();

        //TODO: data seeding
    }
}
