using System.ComponentModel.DataAnnotations;
using WeatherApp.Domain.Enums;

namespace WeatherApp.Api.Contracts.Requests;

public class WeatherReadingSearchRequest
{
    public Guid? LocationId { get; set; }

    public DateTimeOffset? ObservedFrom { get; set; }

    public DateTimeOffset? ObservedTo { get; set; }

    public WeatherCondition? Condition { get; set; }

    public double? MinTemperature { get; set; }

    public double? MaxTemperature { get; set; }

    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    [StringLength(20)]
    public string? SortBy { get; set; }

    public bool Ascending { get; set; } = true;
}

