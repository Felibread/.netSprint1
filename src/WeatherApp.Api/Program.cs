using Microsoft.EntityFrameworkCore;
using WeatherApp.Application.Abstractions;
using WeatherApp.Application.Interfaces;
using WeatherApp.Application.Services;
using WeatherApp.Application.External;
using WeatherApp.Domain.Repositories;
using WeatherApp.Domain.Services;
using WeatherApp.Infrastructure.Persistence;
using WeatherApp.Infrastructure.Repositories;
using WeatherApp.Infrastructure.External.OpenWeather;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database (SQLite for simplicity)
var connectionString = builder.Configuration.GetConnectionString("Default") ?? "Data Source=weather.db";
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

// DI bindings
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IWeatherReadingRepository, WeatherReadingRepository>();
builder.Services.AddScoped<IAlertPolicyService, AlertPolicyService>();
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddHttpClient<IExternalWeatherClient, OpenWeatherClient>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Minimal endpoints
app.MapGet("/api/locations/search", async (string q, int limit, ILocationService service, CancellationToken ct) =>
{
    var result = await service.SearchAsync(q, limit, ct);
    return result.Success ? Results.Ok(result.Value) : Results.BadRequest(new { error = result.Error });
}).WithOpenApi();

app.MapPost("/api/locations", async (CreateLocationRequest request, ILocationService service, CancellationToken ct) =>
{
    var result = await service.CreateAsync(request.Name, request.Latitude, request.Longitude, ct);
    return result.Success ? Results.Created($"/api/locations/{result.Value!.Id}", result.Value) : Results.BadRequest(new { error = result.Error });
}).WithOpenApi();

app.MapGet("/api/weather/current/{locationId}", async (Guid locationId, IWeatherService service, CancellationToken ct) =>
{
    var result = await service.GetCurrentAsync(locationId, ct);
    return result.Success ? Results.Ok(result.Value) : Results.NotFound(new { error = result.Error });
}).WithOpenApi();

app.MapPost("/api/weather/refresh/{locationId}", async (Guid locationId, IWeatherService service, CancellationToken ct) =>
{
    var result = await service.RefreshCurrentAsync(locationId, ct);
    return result.Success ? Results.Ok(result.Value) : Results.BadRequest(new { error = result.Error });
}).WithOpenApi();

app.MapGet("/api/alerts/{locationId}", async (Guid locationId, IAlertService service, CancellationToken ct) =>
{
    var result = await service.EvaluateAlertsAsync(locationId, ct);
    return result.Success ? Results.Ok(result.Value) : Results.BadRequest(new { error = result.Error });
}).WithOpenApi();

app.Run();

public record CreateLocationRequest(string Name, double Latitude, double Longitude);
