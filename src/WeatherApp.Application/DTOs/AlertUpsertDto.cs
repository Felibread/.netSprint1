using WeatherApp.Domain.Enums;

namespace WeatherApp.Application.DTOs;

public record AlertUpsertDto(
    AlertType Type,
    string Title,
    string Message,
    DateTimeOffset? ExpiresAt);

