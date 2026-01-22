namespace TodoApp.Console.Console.Input;

public interface IInteractiveInput
{
    Task<string> ReadTextAsync(string prompt, bool required = true);
    Task<int> ReadIntAsync(string prompt, int? min = null, int? max = null);
    Task<decimal> ReadDecimalAsync(string prompt, decimal? min = null, decimal? max = null);
    Task<int> ReadOptionAsync(string prompt, List<string> options);
    Task<bool> ReadConfirmationAsync(string prompt);
    void ShowHeader(string title);
    void ShowSeparator();
}
