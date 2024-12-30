using Microsoft.AspNetCore.Http.Features;
using Serilog;
using Unify.Api.Extensions;
using Unify.Application;
using Unify.Domain.About;
using Unify.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSwaggerGen();
builder.Services.AddRequestValidators();

builder.Services.Configure<FormOptions>(options =>
{
    //TODO: change limit of file size, needs checking
    options.MultipartBodyLengthLimit = 5000000;
});

builder.Services.Configure<UniversityInformation>(
    builder.Configuration.GetSection("UniversityInformation"));

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();

    // REMARK: Uncomment if you want to seed initial data.
    app.SeedData();
}

app.UseHttpsRedirection();

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseCustomExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
