using System.ComponentModel.DataAnnotations;

namespace TodoApp.Api.DTOs.Requests;

public class UpdateTodoItemRequestDto
{
    [Required(ErrorMessage = "Description is required")]
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string Description { get; set; } = string.Empty;
}
