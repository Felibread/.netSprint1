using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WeatherApp.Api.ViewModels.Alerts;
using WeatherApp.Api.ViewModels.Locations;
using WeatherApp.Api.ViewModels.Shared;
using WeatherApp.Api.ViewModels.Weather;
using WeatherApp.Application.Interfaces;

namespace WeatherApp.Api.Controllers;

[Route("locais")]
public class LocationsController : Controller
{
    private readonly ILocationService _locationService;
    private readonly IWeatherService _weatherService;
    private readonly IAlertService _alertService;
    private readonly ILogger<LocationsController> _logger;

    public LocationsController(
        ILocationService locationService,
        IWeatherService weatherService,
        IAlertService alertService,
        ILogger<LocationsController> logger)
    {
        _locationService = locationService;
        _weatherService = weatherService;
        _alertService = alertService;
        _logger = logger;
    }

    [HttpGet("")]
    [HttpGet("lista")]
    public async Task<IActionResult> Index([FromQuery] LocationFilterViewModel filter, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(new LocationListViewModel
            {
                Filter = filter,
                ErrorMessage = "Parâmetros de filtro inválidos.",
                Pagination = new PaginationViewModel(filter.PageNumber, filter.PageSize, 0, 0)
            });
        }

        var result = await _locationService.SearchAsync(
            filter.Name,
            filter.MinLatitude,
            filter.MaxLatitude,
            filter.MinLongitude,
            filter.MaxLongitude,
            filter.PageNumber,
            filter.PageSize,
            filter.SortBy,
            filter.Ascending,
            cancellationToken);

        if (!result.Success)
        {
            return View(new LocationListViewModel
            {
                Filter = filter,
                ErrorMessage = result.Error,
                Pagination = new PaginationViewModel(filter.PageNumber, filter.PageSize, 0, 0)
            });
        }

        var paged = result.Value!;

        var viewModel = new LocationListViewModel
        {
            Filter = filter,
            Items = paged.Items.Select(l => new LocationViewModel(l.Id, l.Name, l.Latitude, l.Longitude)).ToList(),
            Pagination = new PaginationViewModel(paged.PageNumber, paged.PageSize, paged.TotalItems, paged.TotalPages)
        };

        return View(viewModel);
    }

    [HttpGet("novo")]
    public IActionResult Create()
    {
        return View(new LocationFormViewModel());
    }

    [HttpPost("novo")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LocationFormViewModel viewModel, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var result = await _locationService.CreateAsync(viewModel.Name, viewModel.Latitude, viewModel.Longitude, cancellationToken);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Erro ao criar local.");
            return View(viewModel);
        }

        return RedirectToAction(nameof(Details), new { id = result.Value!.Id });
    }

    [HttpGet("editar/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _locationService.GetByIdAsync(id, cancellationToken);
        if (!result.Success)
        {
            return RedirectToAction(nameof(Index));
        }

        var dto = result.Value!;
        var viewModel = new LocationFormViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };

        return View(viewModel);
    }

    [HttpPost("editar/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, LocationFormViewModel viewModel, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var result = await _locationService.UpdateAsync(id, viewModel.Name, viewModel.Latitude, viewModel.Longitude, cancellationToken);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Erro ao atualizar local.");
            return View(viewModel);
        }

        TempData["SuccessMessage"] = "Local atualizado com sucesso.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost("remover/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _locationService.DeleteAsync(id, cancellationToken);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.Error ?? "Não foi possível remover o local.";
        }
        else
        {
            TempData["SuccessMessage"] = "Local removido com sucesso.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("detalhes/{id:guid}", Name = "detalhes-local")]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var locationResult = await _locationService.GetByIdAsync(id, cancellationToken);
        if (!locationResult.Success)
        {
            TempData["ErrorMessage"] = locationResult.Error ?? "Local não encontrado.";
            return RedirectToAction(nameof(Index));
        }

        var location = locationResult.Value!;

        WeatherReadingViewModel? weatherVm = null;
        var weatherResult = await _weatherService.GetCurrentAsync(id, cancellationToken);
        if (weatherResult.Success && weatherResult.Value is not null)
        {
            var weather = weatherResult.Value;
            weatherVm = new WeatherReadingViewModel
            {
                Id = weather.Id,
                ObservedAt = weather.ObservedAt,
                Temperature = weather.Temperature,
                TemperatureUnit = weather.TemperatureUnit,
                HumidityPercent = weather.HumidityPercent,
                WindSpeedKmh = weather.WindSpeedKmh,
                Condition = weather.Condition,
                PrecipitationProbabilityPercent = weather.PrecipitationProbabilityPercent
            };
        }

        var alertsResult = await _alertService.EvaluateAlertsAsync(id, cancellationToken);
        var alertsVm = alertsResult.Success && alertsResult.Value is not null
            ? alertsResult.Value.Select(a => new AlertViewModel
            {
                Id = a.Id,
                Type = a.Type,
                Title = a.Title,
                Message = a.Message,
                CreatedAt = a.CreatedAt,
                ExpiresAt = a.ExpiresAt
            }).ToList()
            : new List<AlertViewModel>();

        var viewModel = new LocationDetailsViewModel
        {
            Location = new LocationViewModel(location.Id, location.Name, location.Latitude, location.Longitude),
            CurrentWeather = weatherVm,
            Alerts = alertsVm,
            ErrorMessage = alertsResult.Success ? null : alertsResult.Error
        };

        return View(viewModel);
    }
}

