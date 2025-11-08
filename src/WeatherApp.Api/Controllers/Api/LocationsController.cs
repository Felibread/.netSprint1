using Microsoft.AspNetCore.Mvc;
using WeatherApp.Api.Contracts.Requests;
using WeatherApp.Api.Models.Hateoas;
using WeatherApp.Api.Routing;
using WeatherApp.Application.Common;
using WeatherApp.Application.DTOs;
using WeatherApp.Application.Interfaces;

namespace WeatherApp.Api.Controllers.Api;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;
    private readonly ILogger<LocationsController> _logger;

    public LocationsController(ILocationService locationService, ILogger<LocationsController> logger)
    {
        _locationService = locationService;
        _logger = logger;
    }

    [HttpGet("{id:guid}", Name = ApiRouteNames.GetLocationById)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _locationService.GetByIdAsync(id, cancellationToken);
        if (!result.Success)
        {
            return NotFound(new { error = result.Error });
        }

        var dto = result.Value!;
        return Ok(dto);
    }

    [HttpGet("search", Name = ApiRouteNames.LocationSearch)]
    public async Task<IActionResult> Search([FromQuery] LocationSearchRequest request, CancellationToken cancellationToken)
    {
        var result = await _locationService.SearchAsync(
            request.Name,
            request.MinLatitude,
            request.MaxLatitude,
            request.MinLongitude,
            request.MaxLongitude,
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
        var response = PagedResponse<LocationDto>.FromPagedResult(paged, links);

        return Ok(response);
    }

    [HttpPost(Name = ApiRouteNames.CreateLocation)]
    public async Task<IActionResult> Create([FromBody] LocationUpsertRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = await _locationService.CreateAsync(request.Name, request.Latitude, request.Longitude, cancellationToken);
        if (!result.Success)
        {
            return BadRequest(new { error = result.Error });
        }

        var dto = result.Value!;
        var locationUri = Url.Link(ApiRouteNames.GetLocationById, new { id = dto.Id })!;

        return Created(locationUri, dto);
    }

    [HttpPut("{id:guid}", Name = ApiRouteNames.UpdateLocation)]
    public async Task<IActionResult> Update(Guid id, [FromBody] LocationUpsertRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = await _locationService.UpdateAsync(id, request.Name, request.Latitude, request.Longitude, cancellationToken);
        if (!result.Success)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}", Name = ApiRouteNames.DeleteLocation)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var existing = await _locationService.GetByIdAsync(id, cancellationToken);
        if (!existing.Success)
        {
            return NotFound(new { error = existing.Error });
        }

        var result = await _locationService.DeleteAsync(id, cancellationToken);
        if (!result.Success)
        {
            _logger.LogWarning("Falha ao remover localização {LocationId}: {Error}", id, result.Error);
            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }

    private IReadOnlyList<LinkDto> BuildPaginationLinks(PagedResult<LocationDto> paged, LocationSearchRequest request)
    {
        var links = new List<LinkDto>();

        object BuildRouteValues(int pageNumber, int pageSize) => new
        {
            request.Name,
            request.MinLatitude,
            request.MaxLatitude,
            request.MinLongitude,
            request.MaxLongitude,
            request.SortBy,
            request.Ascending,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        links.Add(new LinkDto(
            "self",
            Url.Link(ApiRouteNames.LocationSearch, BuildRouteValues(paged.PageNumber, paged.PageSize))!,
            "GET"));

        if (paged.PageNumber > 1)
        {
            links.Add(new LinkDto(
                "previous",
                Url.Link(ApiRouteNames.LocationSearch, BuildRouteValues(paged.PageNumber - 1, paged.PageSize))!,
                "GET"));
        }

        if (paged.PageNumber < paged.TotalPages)
        {
            links.Add(new LinkDto(
                "next",
                Url.Link(ApiRouteNames.LocationSearch, BuildRouteValues(paged.PageNumber + 1, paged.PageSize))!,
                "GET"));
        }

        return links;
    }
}

