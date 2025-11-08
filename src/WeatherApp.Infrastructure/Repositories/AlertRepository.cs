using System;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Enums;
using WeatherApp.Domain.Repositories;
using WeatherApp.Infrastructure.Persistence;

namespace WeatherApp.Infrastructure.Repositories;

public class AlertRepository : IAlertRepository
{
    private readonly AppDbContext _dbContext;

    public AlertRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Alert alert, CancellationToken cancellationToken = default)
    {
        await _dbContext.Alerts.AddAsync(alert, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Alerts.FindAsync([id], cancellationToken);
        if (entity is not null)
        {
            _dbContext.Alerts.Remove(entity);
        }
    }

    public async Task<Alert?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Alerts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Alert?> GetTrackedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Alerts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Alert> Items, int TotalCount)> SearchAsync(
        AlertType? type,
        DateTimeOffset? createdFrom,
        DateTimeOffset? createdTo,
        bool? onlyActive,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool ascending,
        CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.Alerts.AsQueryable();

        if (type.HasValue)
        {
            queryable = queryable.Where(a => a.Type == type.Value);
        }

        if (createdFrom.HasValue)
        {
            queryable = queryable.Where(a => a.CreatedAt >= createdFrom.Value);
        }

        if (createdTo.HasValue)
        {
            queryable = queryable.Where(a => a.CreatedAt <= createdTo.Value);
        }

        if (onlyActive.HasValue)
        {
            var now = DateTimeOffset.UtcNow;
            if (onlyActive.Value)
            {
                queryable = queryable.Where(a => !a.ExpiresAt.HasValue || a.ExpiresAt >= now);
            }
            else
            {
                queryable = queryable.Where(a => a.ExpiresAt.HasValue && a.ExpiresAt < now);
            }
        }

        var totalCount = await queryable.CountAsync(cancellationToken);

        queryable = (sortBy?.ToLowerInvariant()) switch
        {
            "createdat" => ascending ? queryable.OrderBy(a => a.CreatedAt.ToUnixTimeMilliseconds())
                                     : queryable.OrderByDescending(a => a.CreatedAt.ToUnixTimeMilliseconds()),
            "expiresat" => ascending ? queryable.OrderBy(a => a.ExpiresAt.HasValue ? a.ExpiresAt.Value.ToUnixTimeMilliseconds() : long.MaxValue)
                                     : queryable.OrderByDescending(a => a.ExpiresAt.HasValue ? a.ExpiresAt.Value.ToUnixTimeMilliseconds() : long.MaxValue),
            "type" => ascending ? queryable.OrderBy(a => a.Type) : queryable.OrderByDescending(a => a.Type),
            _ => ascending ? queryable.OrderBy(a => a.CreatedAt.ToUnixTimeMilliseconds())
                           : queryable.OrderByDescending(a => a.CreatedAt.ToUnixTimeMilliseconds())
        };

        var items = await queryable
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task UpdateAsync(Alert alert, CancellationToken cancellationToken = default)
    {
        _dbContext.Alerts.Update(alert);
        return Task.CompletedTask;
    }
}

