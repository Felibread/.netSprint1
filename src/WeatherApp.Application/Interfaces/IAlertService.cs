using WeatherApp.Application.Common;
using WeatherApp.Application.DTOs;

namespace WeatherApp.Application.Interfaces;

public interface IAlertService
{
    Task<Result<IReadOnlyList<AlertDto>>> EvaluateAlertsAsync(Guid locationId, CancellationToken cancellationToken = default);
}
