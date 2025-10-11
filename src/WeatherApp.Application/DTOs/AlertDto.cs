using WeatherApp.Domain.Enums;

namespace WeatherApp.Application.DTOs;

public record AlertDto(
    Guid Id,
    AlertType Type,
    string Title,
    string Message,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ExpiresAt
);
