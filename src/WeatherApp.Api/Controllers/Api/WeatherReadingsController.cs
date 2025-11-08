using Microsoft.AspNetCore.Mvc;
using WeatherApp.Api.Contracts.Requests;
using WeatherApp.Api.Models.Hateoas;
using WeatherApp.Api.Routing;
using WeatherApp.Application.Common;
using WeatherApp.Application.DTOs;
using WeatherApp.Application.Interfaces;

namespace WeatherApp.Api.Controllers.Api;

[ApiController]
[Route("api/weather-readings")]
public class WeatherReadingsController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger<WeatherReadingsController> _logger;

    public WeatherReadingsController(IWeatherService weatherService, ILogger<WeatherReadingsController> logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    [HttpGet("{id:guid}", Name = ApiRouteNames.GetWeatherById)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _weatherService.GetByIdAsync(id, cancellationToken);
        if (!result.Success)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpGet("current/{locationId:guid}", Name = ApiRouteNames.GetCurrentWeather)]
    public async Task<IActionResult> GetCurrent(Guid locationId, CancellationToken cancellationToken)
    {
        var result = await _weatherService.GetCurrentAsync(locationId, cancellationToken);
        if (!result.Success)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpGet("search", Name = ApiRouteNames.WeatherSearch)]
    public async Task<IActionResult> Search([FromQuery] WeatherReadingSearchRequest request, CancellationToken cancellationToken)
    {
        var result = await _weatherService.SearchAsync(
            request.LocationId,
            request.ObservedFrom,
            request.ObservedTo,
            request.Condition,
            request.MinTemperature,
            request.MaxTemperature,
            request.PageNumber,
            request.PageSize,
            request.SortBy,
            request.Ascending,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new { error = result.Error });
        }

        var paged = result.Value!;
        var links = BuildPaginationLinks(paged, request);
        var response = PagedResponse<WeatherReadingDto>.FromPagedResult(paged, links);
        return Ok(response);
    }

    [HttpPost(Name = ApiRouteNames.CreateWeatherReading)]
    public async Task<IActionResult> Create([FromBody] WeatherReadingUpsertRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var dto = new WeatherReadingUpsertDto(
            request.ObservedAt,
            request.Temperature,
            request.TemperatureUnit,
            request.HumidityPercent,
            request.WindSpeedKmh,
            request.Condition,
            request.PrecipitationProbabilityPercent,
            request.LocationId);

        var result = await _weatherService.CreateAsync(dto, cancellationToken);
        if (!result.Success)
        {
            return BadRequest(new { error = result.Error });
        }

        var created = result.Value!;
        var resourceUri = Url.Link(ApiRouteNames.GetWeatherById, new { id = created.Id })!;
        return Created(resourceUri, created);
    }

    [HttpPut("{id:guid}", Name = ApiRouteNames.UpdateWeatherReading)]
    public async Task<IActionResult> Update(Guid id, [FromBody] WeatherReadingUpsertRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var dto = new WeatherReadingUpsertDto(
            request.ObservedAt,
            request.Temperature,
            request.TemperatureUnit,
            request.HumidityPercent,
            request.WindSpeedKmh,
            request.Condition,
            request.PrecipitationProbabilityPercent,
            request.LocationId);

        var result = await _weatherService.UpdateAsync(id, dto, cancellationToken);
        if (!result.Success)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}", Name = ApiRouteNames.DeleteWeatherReading)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var existing = await _weatherService.GetByIdAsync(id, cancellationToken);
        if (!existing.Success)
        {
            return NotFound(new { error = existing.Error });
        }

        var result = await _weatherService.DeleteAsync(id, cancellationToken);
        if (!result.Success)
        {
            _logger.LogWarning("Falha ao remover leitura {WeatherId}: {Error}", id, result.Error);
            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }

    private IReadOnlyList<LinkDto> BuildPaginationLinks(PagedResult<WeatherReadingDto> paged, WeatherReadingSearchRequest request)
    {
        var links = new List<LinkDto>();

        object BuildRouteValues(int pageNumber, int pageSize) => new
        {
            request.LocationId,
            request.ObservedFrom,
            request.ObservedTo,
            request.Condition,
            request.MinTemperature,
            request.MaxTemperature,
            request.SortBy,
            request.Ascending,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        links.Add(new LinkDto(
            "self",
            Url.Link(ApiRouteNames.WeatherSearch, BuildRouteValues(paged.PageNumber, paged.PageSize))!,
            "GET"));

        if (paged.PageNumber > 1)
        {
            links.Add(new LinkDto(
                "previous",
                Url.Link(ApiRouteNames.WeatherSearch, BuildRouteValues(paged.PageNumber - 1, paged.PageSize))!,
                "GET"));
        }

        if (paged.PageNumber < paged.TotalPages)
        {
            links.Add(new LinkDto(
                "next",
                Url.Link(ApiRouteNames.WeatherSearch, BuildRouteValues(paged.PageNumber + 1, paged.PageSize))!,
                "GET"));
        }

        return links;
    }
}

