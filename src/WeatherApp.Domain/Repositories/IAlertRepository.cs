using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Enums;

namespace WeatherApp.Domain.Repositories;

public interface IAlertRepository
{
    Task<Alert?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Alert?> GetTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Alert> Items, int TotalCount)> SearchAsync(
        AlertType? type,
        DateTimeOffset? createdFrom,
        DateTimeOffset? createdTo,
        bool? onlyActive,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool ascending,
        CancellationToken cancellationToken = default);
    Task AddAsync(Alert alert, CancellationToken cancellationToken = default);
    Task UpdateAsync(Alert alert, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

