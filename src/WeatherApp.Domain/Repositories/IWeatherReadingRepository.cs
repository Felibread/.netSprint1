using WeatherApp.Domain.Entities;

namespace WeatherApp.Domain.Repositories;

public interface IWeatherReadingRepository
{
    Task<WeatherReading?> GetLatestForLocationAsync(Guid locationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WeatherReading>> GetHourlyForLocationAsync(Guid locationId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
    Task AddAsync(WeatherReading reading, CancellationToken cancellationToken = default);
}
