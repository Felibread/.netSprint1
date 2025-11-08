using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Api.Contracts.Requests;

public class LocationUpsertRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }
}

