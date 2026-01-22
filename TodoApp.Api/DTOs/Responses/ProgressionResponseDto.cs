namespace TodoApp.Api.DTOs.Responses;

public class ProgressionResponseDto
{
    public DateTime DateTime { get; set; }
    public decimal Percent { get; set; }
    public decimal AccumulatedPercent { get; set; }
}
