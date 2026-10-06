using System.Text.Json.Serialization;
using Serilog;
using TechnicalMastery.Api.Middleware;
using TechnicalMastery.Application;
using TechnicalMastery.Infrastructure;

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
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Global exception handling runs first so every failure leaves this
// pipeline as a uniform ApiResponse envelope (never a stack trace).
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Concise per-request line: method, path, status code, elapsed milliseconds.
app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("TechnicalMastery API started in {Environment}.", app.Environment.EnvironmentName);
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
