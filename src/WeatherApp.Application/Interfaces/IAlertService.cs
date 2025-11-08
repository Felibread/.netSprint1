using WeatherApp.Application.Common;
using WeatherApp.Application.DTOs;
using WeatherApp.Domain.Enums;

namespace WeatherApp.Application.Interfaces;

public interface IAlertService
{
    Task<Result<IReadOnlyList<AlertDto>>> EvaluateAlertsAsync(Guid locationId, CancellationToken cancellationToken = default);
    Task<Result<AlertDto>> CreateAsync(AlertUpsertDto dto, CancellationToken cancellationToken = default);
    Task<Result<AlertDto>> UpdateAsync(Guid id, AlertUpsertDto dto, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<AlertDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<AlertDto>>> SearchAsync(
        AlertType? type,
        DateTimeOffset? createdFrom,
        DateTimeOffset? createdTo,
        bool? onlyActive,
        int pageNumber = 1,
        int pageSize = 10,
        string? sortBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default);
}
