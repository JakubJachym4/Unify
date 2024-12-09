using FluentValidation;
using Unify.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Unify.Api.Middleware;
using Unify.Application.Users.RegisterUser;

namespace Unify.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();
    }

    public static void UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    public static void AddRequestValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterUserCommandValidator>();
    }

    public static IApplicationBuilder UseRequestContextLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestContextLoggingMiddleware>();
    }
}