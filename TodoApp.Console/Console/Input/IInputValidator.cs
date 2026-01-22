namespace TodoApp.Console.Console.Input;

public interface IInputValidator
{
    ValidationResult ValidateAddCommand(string[] args);
    ValidationResult ValidateUpdateCommand(string[] args);
    ValidationResult ValidateRemoveCommand(string[] args);
    ValidationResult ValidateProgressCommand(string[] args);
    ValidationResult ValidateId(string idString, out int id);
    ValidationResult ValidatePercent(string percentString, out decimal percent);
}
