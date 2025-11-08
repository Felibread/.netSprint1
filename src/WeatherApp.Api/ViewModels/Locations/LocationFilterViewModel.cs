using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Api.ViewModels.Locations;

public class LocationFilterViewModel
{
    [Display(Name = "Nome")]
    public string? Name { get; set; }

    [Display(Name = "Latitude mínima")]
    [Range(-90, 90, ErrorMessage = "A latitude deve estar entre -90 e 90.")]
    public double? MinLatitude { get; set; }

    [Display(Name = "Latitude máxima")]
    [Range(-90, 90, ErrorMessage = "A latitude deve estar entre -90 e 90.")]
    public double? MaxLatitude { get; set; }

    [Display(Name = "Longitude mínima")]
    [Range(-180, 180, ErrorMessage = "A longitude deve estar entre -180 e 180.")]
    public double? MinLongitude { get; set; }

    [Display(Name = "Longitude máxima")]
    [Range(-180, 180, ErrorMessage = "A longitude deve estar entre -180 e 180.")]
    public double? MaxLongitude { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public bool Ascending { get; set; } = true;
}

