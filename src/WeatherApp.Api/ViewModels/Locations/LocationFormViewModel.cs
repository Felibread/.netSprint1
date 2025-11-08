using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Api.ViewModels.Locations;

public class LocationFormViewModel
{
    public Guid? Id { get; set; }

    [Display(Name = "Nome")]
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve conter entre 3 e 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Latitude")]
    [Range(-90, 90, ErrorMessage = "A latitude deve estar entre -90 e 90.")]
    public double Latitude { get; set; }

    [Display(Name = "Longitude")]
    [Range(-180, 180, ErrorMessage = "A longitude deve estar entre -180 e 180.")]
    public double Longitude { get; set; }
}

