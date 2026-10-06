using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TechnicalMastery.Api.Middleware;
using TechnicalMastery.Application;
using TechnicalMastery.Application.DTOs.Common;
using TechnicalMastery.Infrastructure;
using TechnicalMastery.Infrastructure.Data.Seed;

var builder = WebApplication.CreateBuilder(args);

// Serilog reads its sinks/levels from the "Serilog" appsettings section.
// Only method/path/status/elapsed are ever logged — never bodies, tokens,
// passwords or connection strings (§27).
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

builder.Services.AddInfrastructure(connectionString);
builder.Services.AddApplication();

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Model-binding failures (bad enums, malformed query values) never reach actions,
// so they get the same 400 envelope here instead of the framework default.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        List<string> errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .SelectMany(entry => entry.Value!.Errors.Select(error => entry.Key + ": " + error.ErrorMessage))
            .ToList();

        ApiResponse<object> response = ApiResponse<object>.Fail("One or more validation errors occurred.", errors);

        return new BadRequestObjectResult(response);
    };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "DotNetTechnicalMastery API",
        Version = "v1",
        Description = "Backend for the .NET Technical Q&A Learning Platform. " +
            "WPF today, mobile tomorrow — every endpoint below is the stable " +
            "HTTP + JSON contract any client can consume."
    });

    string xmlFile = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Global exception handling runs first so every failure leaves this
// pipeline as a uniform ApiResponse envelope (never a stack trace).
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Interactive API documentation for developers (including future mobile clients).
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Concise per-request line: method, path, status code, elapsed milliseconds.
app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("TechnicalMastery API started in {Environment}.", app.Environment.EnvironmentName);

    // Applies pending migrations, then loads categories/topics/question batches.
    // Idempotent: safe to run on every startup.
    using (IServiceScope scope = app.Services.CreateScope())
    {
        DatabaseSeeder seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync(CancellationToken.None);
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "TechnicalMastery API terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
