using WeatherApp.Application.Common;
using WeatherApp.Application.DTOs;

namespace WeatherApp.Application.Interfaces;

public interface IWeatherService
{
    Task<Result<WeatherReadingDto>> GetCurrentAsync(Guid locationId, CancellationToken cancellationToken = default);
}
