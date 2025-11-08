using WeatherApp.Domain.Entities;

namespace WeatherApp.Domain.Repositories;

public interface ILocationRepository
{
    Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Location?> GetTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Location> Items, int TotalCount)> SearchAsync(
        string? nameQuery,
        double? minLatitude,
        double? maxLatitude,
        double? minLongitude,
        double? maxLongitude,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool ascending,
        CancellationToken cancellationToken = default);
    Task AddAsync(Location location, CancellationToken cancellationToken = default);
    Task UpdateAsync(Location location, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
