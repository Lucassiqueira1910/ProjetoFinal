using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using USUARIOminimalSolution.Web.Extensions;
using USUARIOminimalSolution.Web.Middleware;
using System.IO;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// 🔥 FORÇA CAMINHO DA RAIZ (NÃO USA MAIS /bin)
var projectRoot = Directory.GetParent(Directory.GetCurrentDirectory())!
    .Parent!.Parent!.FullName;

var logPath = Path.Combine(projectRoot, "logs", "log.txt");

// 🔥 CRIA PASTA logs NA RAIZ
Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

// 🔥 CONFIG SERILOG
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
    .CreateLogger();

// 🔥 ATIVA SERILOG
builder.Host.UseSerilog();


// 🔥 OPEN TELEMETRY
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddConsoleExporter();
    });


// 🔹 Controllers
builder.Services.AddControllers();


// 🔹 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SW_USUARIO API",
        Version = "v1"
    });
});


// 🔹 Services
builder.Services.AddProjectServices(builder.Configuration);


// 🔥 CONNECTION STRING
var connString = File.ReadAllText("connection.txt").Trim();


// 🔥 HEALTH CHECKS
builder.Services.AddHealthChecks()
    .AddOracle(connString,
        name: "oracle-db",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "db" })

    .AddUrlGroup(new Uri("https://www.google.com"),
        name: "api-externa",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "api" })

    .AddCheck("self",
        () => HealthCheckResult.Healthy("API OK"),
        tags: new[] { "self" });


var app = builder.Build();


// 🔹 Middleware
app.UseMiddleware<ErrorHandlingMiddleware>();


// 🔹 Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SW_USUARIO API v1");
    c.RoutePrefix = "swagger";
});


app.UseRouting();
app.UseAuthorization();


// 🔥 CORRELAÇÃO DE REQUEST
app.Use(async (context, next) =>
{
    var requestId = Guid.NewGuid().ToString();

    using (Serilog.Context.LogContext.PushProperty("RequestId", requestId))
    {
        await next();
    }
});


// 🔹 Controllers
app.MapControllers();


// 🔥 HEALTH ENDPOINTS
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/database", new HealthCheckOptions
{
    Predicate = c => c.Tags.Contains("db"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/api", new HealthCheckOptions
{
    Predicate = c => c.Tags.Contains("api"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = c => c.Tags.Contains("self"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();

public partial class Program { }