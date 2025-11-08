using System.ComponentModel.DataAnnotations;
using WeatherApp.Domain.Enums;

namespace WeatherApp.Api.Contracts.Requests;

public class AlertSearchRequest
{
    public AlertType? Type { get; set; }

    public DateTimeOffset? CreatedFrom { get; set; }

    public DateTimeOffset? CreatedTo { get; set; }

    public bool? OnlyActive { get; set; }

    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    [StringLength(20)]
    public string? SortBy { get; set; }

    public bool Ascending { get; set; } = true;
}

