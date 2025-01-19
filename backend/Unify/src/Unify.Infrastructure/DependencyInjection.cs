using Unify.Application.Abstractions.Authentication;
using Unify.Application.Abstractions.Clock;
using Unify.Application.Abstractions.Data;
using Unify.Application.Abstractions.Email;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Unify.Application.Abstractions.Files;
using Unify.Domain.Abstractions;
using Unify.Domain.Messages;
using Unify.Domain.Messages.InformationMessages;
using Unify.Domain.UniversityClasses.Abstractions;
using Unify.Domain.UniversityCore.Abstractions;
using Unify.Domain.Users;
using Unify.Infrastructure.Authentication;
using Unify.Infrastructure.Authorization;
using Unify.Infrastructure.Clock;
using Unify.Infrastructure.Data;
using Unify.Infrastructure.Email;
using Unify.Infrastructure.FileUpload;
using Unify.Infrastructure.Repositories;
using Unify.Infrastructure.Repositories.UniversityClasses;
using Unify.Infrastructure.Repositories.UniversityCore;
using AuthenticationOptions = Unify.Infrastructure.Authentication.AuthenticationOptions;
using AuthenticationService = Unify.Infrastructure.Authentication.AuthenticationService;
using IAuthenticationService = Unify.Application.Abstractions.Authentication.IAuthenticationService;

namespace Unify.Infrastructure;

using AuthenticationOptions = Authentication.AuthenticationOptions;
using AuthenticationService = Authentication.AuthenticationService;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        services.AddTransient<IEmailService, EmailService>();

        services.AddTransient<IFileConversionService, FileConverter>();

        AddPersistence(services, configuration);

        AddAuthentication(services, configuration);

        AddAuthorization(services);

        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("Database") ??
            throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
            options.EnableSensitiveDataLogging();
        }, ServiceLifetime.Scoped);

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IMessageRepository, MessageRepository>();

        services.AddScoped<IInformationMessageRepository, InformationMessageRepository>();

        services.AddScoped<IFieldOfStudyRepository, FieldOfStudyRepository>();

        services.AddScoped<ISpecializationRepository, SpecializationRepository>();

        services.AddScoped<IFacultyRepository, FacultyRepository>();

        services.AddScoped<ILocationRepository, LocationRepository>();

        services.AddScoped<ICourseRepository, CourseRepository>();

        services.AddScoped<IClassOfferingRepository, ClassOfferingRepository>();

        services.AddScoped<IStudentGroupRepository, StudentGroupRepository>();

        services.AddScoped<IClassEnrollmentRepository, ClassEnrollmentRepository>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddSingleton<ISqlConnectionFactory>(_ =>
            new SqlConnectionFactory(connectionString));

        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
    }

    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.Configure<AuthenticationOptions>(configuration.GetSection("Authentication"));

        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));

        services.AddTransient<AdminAuthorizationDelegatingHandler>();

        services.AddHttpClient<IAuthenticationService, AuthenticationService>((serviceProvider, httpClient) =>
            {
                var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

                httpClient.BaseAddress = new Uri(keycloakOptions.AdminUrl);
            })
            .AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

        services.AddHttpClient<IJwtService, JwtService>((serviceProvider, httpClient) =>
        {
            var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

            httpClient.BaseAddress = new Uri(keycloakOptions.TokenUrl);
        });

        services.AddHttpContextAccessor();

        services.AddScoped<IUserContext, UserContext>();
    }

    private static void AddAuthorization(IServiceCollection services)
    {
        services.AddScoped<AuthorizationService>();

        services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
    }
}