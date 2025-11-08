using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WeatherApp.Api.ViewModels.Home;
using WeatherApp.Api.ViewModels.Locations;
using WeatherApp.Application.Interfaces;

namespace WeatherApp.Api.Controllers;

public class HomeController : Controller
{
    private readonly ILocationService _locationService;
    private readonly IAlertService _alertService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        ILocationService locationService,
        IAlertService alertService,
        ILogger<HomeController> logger)
    {
        _locationService = locationService;
        _alertService = alertService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var locationsResult = await _locationService.SearchAsync(
            name: null,
            minLatitude: null,
            maxLatitude: null,
            minLongitude: null,
            maxLongitude: null,
            pageNumber: 1,
            pageSize: 5,
            sortBy: "name",
            ascending: true,
            cancellationToken);

        var highlight = locationsResult.Success
            ? locationsResult.Value!.Items.Select(l => new LocationViewModel(l.Id, l.Name, l.Latitude, l.Longitude)).ToList()
            : new List<LocationViewModel>();

        var totalLocations = locationsResult.Success ? locationsResult.Value!.TotalItems : 0;

        int evaluatedAlerts = 0;
        if (!locationsResult.Success)
        {
            _logger.LogWarning("Falha ao buscar destaques para a home: {Error}", locationsResult.Error);
        }
        else
        {
            foreach (var loc in locationsResult.Value!.Items)
            {
                var alerts = await _alertService.EvaluateAlertsAsync(loc.Id, cancellationToken);
                if (alerts.Success)
                {
                    evaluatedAlerts += alerts.Value!.Count;
                }
            }
        }

        var viewModel = new HomeViewModel
        {
            HighlightedLocations = highlight,
            TotalLocations = totalLocations,
            TotalAlertsEvaluated = evaluatedAlerts,
            Message = locationsResult.Success ? null : locationsResult.Error
        };

        return View(viewModel);
    }

    [HttpGet("sobre")]
    public IActionResult About()
    {
        return View();
    }
}

