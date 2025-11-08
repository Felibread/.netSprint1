using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Api.Contracts.Requests;

public class LocationSearchRequest
{
    [StringLength(100)]
    public string? Name { get; set; }

    [Range(-90, 90)]
    public double? MinLatitude { get; set; }

    [Range(-90, 90)]
    public double? MaxLatitude { get; set; }

    [Range(-180, 180)]
    public double? MinLongitude { get; set; }

    [Range(-180, 180)]
    public double? MaxLongitude { get; set; }

    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    [StringLength(20)]
    public string? SortBy { get; set; }

    public bool Ascending { get; set; } = true;
}

