using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.DTOs.Requests;
using TodoApp.Api.DTOs.Responses;
using TodoApp.Application.Application.Commands;
using TodoApp.Application.Application.Queries;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class TodoListsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<TodoListsController> _logger;

    public TodoListsController(IMediator mediator, IMapper mapper, ILogger<TodoListsController> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Get all todo items
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<TodoItemResponseDto>>), 200)]
    public async Task<IActionResult> GetAllTodoItems()
    {
        _logger.LogInformation("Getting all todo items");
        
        var items = await _mediator.Send(new GetTodoItemsQuery());
        var response = _mapper.Map<List<TodoItemResponseDto>>(items);
        
        return Ok(ApiResponse<List<TodoItemResponseDto>>.CreateSuccess(response));
    }

    /// <summary>
    /// Get todo item by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<TodoItemResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetTodoItem(int id)
    {
        _logger.LogInformation("Getting todo item with ID: {TodoItemId}", id);
        
        var items = await _mediator.Send(new GetTodoItemsQuery());
        var item = items.FirstOrDefault(x => x.Id == id);
        
        if (item == null)
            return NotFound(ApiResponse<object>.CreateError($"Todo item with ID {id} not found"));
        
        var response = _mapper.Map<TodoItemResponseDto>(item);
        return Ok(ApiResponse<TodoItemResponseDto>.CreateSuccess(response));
    }

    /// <summary>
    /// Create a new todo item
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TodoItemResponseDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> CreateTodoItem([FromBody] CreateTodoItemRequestDto request)
    {
        _logger.LogInformation("Creating todo item: {Title}", request.Title);
        
        var nextId = await _mediator.Send(new GetNextIdQuery());
        await _mediator.Send(new AddTodoItemCommand(nextId, request.Title, request.Description, request.Category));
        
        var items = await _mediator.Send(new GetTodoItemsQuery());
        var createdItem = items.FirstOrDefault(x => x.Id == nextId);
        var response = _mapper.Map<TodoItemResponseDto>(createdItem);
        
        return CreatedAtAction(nameof(GetTodoItem), new { id = nextId }, 
            ApiResponse<TodoItemResponseDto>.CreateSuccess(response));
    }

    /// <summary>
    /// Update todo item
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> UpdateTodoItem(int id, [FromBody] UpdateTodoItemRequestDto request)
    {
        _logger.LogInformation("Updating todo item: {Id}", id);
        
        await _mediator.Send(new UpdateTodoItemCommand(id, request.Description));
        
        return Ok(ApiResponse<object>.CreateSuccess(null, "Todo item updated successfully"));
    }

    /// <summary>
    /// Delete todo item
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> DeleteTodoItem(int id)
    {
        _logger.LogInformation("Deleting todo item: {Id}", id);
        
        await _mediator.Send(new RemoveTodoItemCommand(id));
        
        return Ok(ApiResponse<object>.CreateSuccess(null, "Todo item deleted successfully"));
    }
}
