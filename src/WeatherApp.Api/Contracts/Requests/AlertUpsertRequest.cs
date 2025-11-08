using System.ComponentModel.DataAnnotations;
using WeatherApp.Domain.Enums;

namespace WeatherApp.Api.Contracts.Requests;

public class AlertUpsertRequest
{
    [Required]
    public AlertType Type { get; set; }

    [Required]
    [StringLength(80, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(400, MinimumLength = 10)]
    public string Message { get; set; } = string.Empty;

    public DateTimeOffset? ExpiresAt { get; set; }
}

