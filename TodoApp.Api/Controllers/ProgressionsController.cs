using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.DTOs.Requests;
using TodoApp.Api.DTOs.Responses;
using TodoApp.Application.Application.Commands;
using TodoApp.Application.Application.Queries;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/v1/todolists/{todoItemId:int}/[controller]")]
[Produces("application/json")]
public class ProgressionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<ProgressionsController> _logger;

    public ProgressionsController(IMediator mediator, IMapper mapper, ILogger<ProgressionsController> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Register progression for todo item
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<object>), 201)]
    public async Task<IActionResult> RegisterProgression(int todoItemId, [FromBody] RegisterProgressionRequestDto request)
    {
        _logger.LogInformation("Registering progression for item {Id}: {Percent}%", todoItemId, request.Percent);
        
        await _mediator.Send(new RegisterProgressionCommand(todoItemId, request.DateTime ?? DateTime.UtcNow, request.Percent));
        
        return CreatedAtAction(nameof(TodoListsController.GetTodoItem), "TodoLists", new { id = todoItemId },
            ApiResponse<object>.CreateSuccess(null, "Progression registered successfully"));
    }

    /// <summary>
    /// Get progressions for todo item
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<ProgressionResponseDto>>), 200)]
    public async Task<IActionResult> GetProgressions(int todoItemId)
    {
        _logger.LogInformation("Getting progressions for item {Id}", todoItemId);
        
        var items = await _mediator.Send(new GetTodoItemsQuery());
        var todoItem = items.FirstOrDefault(x => x.Id == todoItemId);
        
        if (todoItem == null)
            return NotFound(ApiResponse<object>.CreateError($"Todo item with ID {todoItemId} not found"));
        
        var progressions = _mapper.Map<List<ProgressionResponseDto>>(todoItem.Progressions);
        return Ok(ApiResponse<List<ProgressionResponseDto>>.CreateSuccess(progressions));
    }
}
