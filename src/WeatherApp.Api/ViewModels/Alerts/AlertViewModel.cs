using WeatherApp.Domain.Enums;

namespace WeatherApp.Api.ViewModels.Alerts;

public class AlertViewModel
{
    public Guid Id { get; init; }
    public AlertType Type { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? ExpiresAt { get; init; }

    public bool IsActive => !ExpiresAt.HasValue || ExpiresAt >= DateTimeOffset.UtcNow;
}

