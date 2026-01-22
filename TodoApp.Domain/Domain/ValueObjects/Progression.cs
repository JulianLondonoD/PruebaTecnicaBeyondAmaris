namespace TodoApp.Domain.Domain.ValueObjects;

public record Progression
{
    public DateTime DateTime { get; init; }
    public decimal Percent { get; init; }

    public Progression(DateTime dateTime, decimal percent)
    {
        if (percent <= 0 || percent > 100)
            throw new ArgumentException("Percent must be greater than 0 and less than or equal to 100", nameof(percent));
        
        DateTime = dateTime;
        Percent = percent;
    }
}
