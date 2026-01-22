using System.ComponentModel.DataAnnotations;

namespace TodoApp.Api.DTOs.Requests;

public class RegisterProgressionRequestDto
{
    [Required]
    [Range(0.01, 100, ErrorMessage = "Percent must be between 0.01 and 100")]
    public decimal Percent { get; set; }

    public DateTime? DateTime { get; set; }
}
