using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Enums;

namespace WeatherApp.Domain.Repositories;

public interface IWeatherReadingRepository
{
    Task<WeatherReading?> GetLatestForLocationAsync(Guid locationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WeatherReading>> GetHourlyForLocationAsync(Guid locationId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
    Task<WeatherReading?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WeatherReading?> GetTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<WeatherReading> Items, int TotalCount)> SearchAsync(
        Guid? locationId,
        DateTimeOffset? observedFrom,
        DateTimeOffset? observedTo,
        WeatherCondition? condition,
        double? minTemperature,
        double? maxTemperature,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool ascending,
        CancellationToken cancellationToken = default);
    Task AddAsync(WeatherReading reading, CancellationToken cancellationToken = default);
    Task UpdateAsync(WeatherReading reading, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
