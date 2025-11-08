using System.Linq;
using WeatherApp.Application.Abstractions;
using WeatherApp.Application.Common;
using WeatherApp.Application.DTOs;
using WeatherApp.Application.Interfaces;
using WeatherApp.Domain.Repositories;
using WeatherApp.Domain.Services;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Enums;

namespace WeatherApp.Application.Services;

public class AlertService : IAlertService
{
    private readonly IWeatherReadingRepository _weatherRepository;
    private readonly IAlertPolicyService _alertPolicy;
    private readonly IAlertRepository _alertRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AlertService(
        IWeatherReadingRepository weatherRepository,
        IAlertPolicyService alertPolicy,
        IAlertRepository alertRepository,
        IUnitOfWork unitOfWork)
    {
        _weatherRepository = weatherRepository;
        _alertPolicy = alertPolicy;
        _alertRepository = alertRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<AlertDto>>> EvaluateAlertsAsync(Guid locationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var reading = await _weatherRepository.GetLatestForLocationAsync(locationId, cancellationToken);
            if (reading is null)
            {
                return Result<IReadOnlyList<AlertDto>>.Fail("Sem leitura para avaliar alertas.");
            }

            var alerts = new List<AlertDto>();

            var rainAlert = _alertPolicy.TryCreateRainAlert(reading, 60);
            if (rainAlert is not null)
            {
                alerts.Add(new AlertDto(rainAlert.Id, rainAlert.Type, rainAlert.Title, rainAlert.Message, rainAlert.CreatedAt, rainAlert.ExpiresAt));
            }

            var extreme = _alertPolicy.TryCreateExtremeTemperatureAlert(reading, 5, 35);
            if (extreme is not null)
            {
                alerts.Add(new AlertDto(extreme.Id, extreme.Type, extreme.Title, extreme.Message, extreme.CreatedAt, extreme.ExpiresAt));
            }

            return Result<IReadOnlyList<AlertDto>>.Ok(alerts);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<AlertDto>>.Fail($"Erro ao avaliar alertas: {ex.Message}");
        }
    }

    public async Task<Result<AlertDto>> CreateAsync(AlertUpsertDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var alert = new Alert(dto.Type, dto.Title, dto.Message, dto.ExpiresAt);
            await _alertRepository.AddAsync(alert, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<AlertDto>.Ok(new AlertDto(alert.Id, alert.Type, alert.Title, alert.Message, alert.CreatedAt, alert.ExpiresAt));
        }
        catch (Exception ex)
        {
            return Result<AlertDto>.Fail($"Erro ao criar alerta: {ex.Message}");
        }
    }

    public async Task<Result<AlertDto>> UpdateAsync(Guid id, AlertUpsertDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var alert = await _alertRepository.GetTrackedAsync(id, cancellationToken);
            if (alert is null)
            {
                return Result<AlertDto>.Fail("Alerta não encontrado.");
            }

            alert.Update(dto.Type, dto.Title, dto.Message, dto.ExpiresAt);
            await _alertRepository.UpdateAsync(alert, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<AlertDto>.Ok(new AlertDto(alert.Id, alert.Type, alert.Title, alert.Message, alert.CreatedAt, alert.ExpiresAt));
        }
        catch (Exception ex)
        {
            return Result<AlertDto>.Fail($"Erro ao atualizar alerta: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _alertRepository.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Erro ao remover alerta: {ex.Message}");
        }
    }

    public async Task<Result<AlertDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var alert = await _alertRepository.GetByIdAsync(id, cancellationToken);
            if (alert is null)
            {
                return Result<AlertDto>.Fail("Alerta não encontrado.");
            }

            return Result<AlertDto>.Ok(new AlertDto(alert.Id, alert.Type, alert.Title, alert.Message, alert.CreatedAt, alert.ExpiresAt));
        }
        catch (Exception ex)
        {
            return Result<AlertDto>.Fail($"Erro ao obter alerta: {ex.Message}");
        }
    }

    public async Task<Result<PagedResult<AlertDto>>> SearchAsync(
        AlertType? type,
        DateTimeOffset? createdFrom,
        DateTimeOffset? createdTo,
        bool? onlyActive,
        int pageNumber = 1,
        int pageSize = 10,
        string? sortBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return Result<PagedResult<AlertDto>>.Fail("Parâmetros de paginação inválidos.");
            }

            var (items, totalCount) = await _alertRepository.SearchAsync(
                type,
                createdFrom,
                createdTo,
                onlyActive,
                pageNumber,
                pageSize,
                sortBy,
                ascending,
                cancellationToken);

            var dtos = items
                .Select(a => new AlertDto(a.Id, a.Type, a.Title, a.Message, a.CreatedAt, a.ExpiresAt))
                .ToList();

            var paged = new PagedResult<AlertDto>(dtos, pageNumber, pageSize, totalCount);
            return Result<PagedResult<AlertDto>>.Ok(paged);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<AlertDto>>.Fail($"Erro na busca de alertas: {ex.Message}");
        }
    }
}
