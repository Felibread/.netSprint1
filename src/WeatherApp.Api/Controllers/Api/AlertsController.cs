using Microsoft.AspNetCore.Mvc;
using WeatherApp.Api.Contracts.Requests;
using WeatherApp.Api.Models.Hateoas;
using WeatherApp.Api.Routing;
using WeatherApp.Application.Common;
using WeatherApp.Application.DTOs;
using WeatherApp.Application.Interfaces;

namespace WeatherApp.Api.Controllers.Api;

[ApiController]
[Route("api/alerts")]
public class AlertsController : ControllerBase
{
    private readonly IAlertService _alertService;
    private readonly ILogger<AlertsController> _logger;

    public AlertsController(IAlertService alertService, ILogger<AlertsController> logger)
    {
        _alertService = alertService;
        _logger = logger;
    }

    [HttpGet("{id:guid}", Name = ApiRouteNames.GetAlertById)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _alertService.GetByIdAsync(id, cancellationToken);
        if (!result.Success)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpGet("evaluate/{locationId:guid}", Name = ApiRouteNames.EvaluateAlert)]
    public async Task<IActionResult> Evaluate(Guid locationId, CancellationToken cancellationToken)
    {
        var result = await _alertService.EvaluateAlertsAsync(locationId, cancellationToken);
        if (!result.Success)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpGet("search", Name = ApiRouteNames.AlertSearch)]
    public async Task<IActionResult> Search([FromQuery] AlertSearchRequest request, CancellationToken cancellationToken)
    {
        var result = await _alertService.SearchAsync(
            request.Type,
            request.CreatedFrom,
            request.CreatedTo,
            request.OnlyActive,
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
        var response = PagedResponse<AlertDto>.FromPagedResult(paged, links);
        return Ok(response);
    }

    [HttpPost(Name = ApiRouteNames.CreateAlert)]
    public async Task<IActionResult> Create([FromBody] AlertUpsertRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var dto = new AlertUpsertDto(request.Type, request.Title, request.Message, request.ExpiresAt);
        var result = await _alertService.CreateAsync(dto, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new { error = result.Error });
        }

        var created = result.Value!;
        var resourceUri = Url.Link(ApiRouteNames.GetAlertById, new { id = created.Id })!;
        return Created(resourceUri, created);
    }

    [HttpPut("{id:guid}", Name = ApiRouteNames.UpdateAlert)]
    public async Task<IActionResult> Update(Guid id, [FromBody] AlertUpsertRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var dto = new AlertUpsertDto(request.Type, request.Title, request.Message, request.ExpiresAt);
        var result = await _alertService.UpdateAsync(id, dto, cancellationToken);
        if (!result.Success)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}", Name = ApiRouteNames.DeleteAlert)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var existing = await _alertService.GetByIdAsync(id, cancellationToken);
        if (!existing.Success)
        {
            return NotFound(new { error = existing.Error });
        }

        var result = await _alertService.DeleteAsync(id, cancellationToken);
        if (!result.Success)
        {
            _logger.LogWarning("Falha ao remover alerta {AlertId}: {Error}", id, result.Error);
            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }

    private IReadOnlyList<LinkDto> BuildPaginationLinks(PagedResult<AlertDto> paged, AlertSearchRequest request)
    {
        var links = new List<LinkDto>();

        object BuildRouteValues(int pageNumber, int pageSize) => new
        {
            request.Type,
            request.CreatedFrom,
            request.CreatedTo,
            request.OnlyActive,
            request.SortBy,
            request.Ascending,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        links.Add(new LinkDto(
            "self",
            Url.Link(ApiRouteNames.AlertSearch, BuildRouteValues(paged.PageNumber, paged.PageSize))!,
            "GET"));

        if (paged.PageNumber > 1)
        {
            links.Add(new LinkDto(
                "previous",
                Url.Link(ApiRouteNames.AlertSearch, BuildRouteValues(paged.PageNumber - 1, paged.PageSize))!,
                "GET"));
        }

        if (paged.PageNumber < paged.TotalPages)
        {
            links.Add(new LinkDto(
                "next",
                Url.Link(ApiRouteNames.AlertSearch, BuildRouteValues(paged.PageNumber + 1, paged.PageSize))!,
                "GET"));
        }

        return links;
    }
}

