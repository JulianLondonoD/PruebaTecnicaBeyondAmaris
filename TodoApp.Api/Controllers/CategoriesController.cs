using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.DTOs.Responses;
using TodoApp.Application.Application.Queries;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(IMediator mediator, ILogger<CategoriesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all categories
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<string>>), 200)]
    public async Task<IActionResult> GetCategories()
    {
        _logger.LogInformation("Getting categories");
        
        var categories = await _mediator.Send(new GetCategoriesQuery());
        
        return Ok(ApiResponse<List<string>>.CreateSuccess(categories));
    }
}
