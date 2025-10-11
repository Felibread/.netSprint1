using WeatherApp.Application.Common;
using WeatherApp.Application.DTOs;

namespace WeatherApp.Application.Interfaces;

public interface IWeatherService
{
    Task<Result<WeatherReadingDto>> GetCurrentAsync(Guid locationId, CancellationToken cancellationToken = default);
    Task<Result<WeatherReadingDto>> RefreshCurrentAsync(Guid locationId, CancellationToken cancellationToken = default);
}
