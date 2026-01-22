namespace TodoApp.Console.Console.Input;

public class CommandInputValidator : IInputValidator
{
    public ValidationResult ValidateAddCommand(string[] args)
    {
        if (args.Length < 4)
        {
            return ValidationResult.Failure("Uso: add <título> <descripción> <categoría>");
        }
        return ValidationResult.Success();
    }

    public ValidationResult ValidateUpdateCommand(string[] args)
    {
        if (args.Length < 3)
        {
            return ValidationResult.Failure("Uso: update <id> <descripción>");
        }

        var idValidation = ValidateId(args[1], out _);
        if (!idValidation.IsValid)
        {
            return idValidation;
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidateRemoveCommand(string[] args)
    {
        if (args.Length < 2)
        {
            return ValidationResult.Failure("Uso: remove <id>");
        }

        var idValidation = ValidateId(args[1], out _);
        if (!idValidation.IsValid)
        {
            return idValidation;
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidateProgressCommand(string[] args)
    {
        if (args.Length < 3)
        {
            return ValidationResult.Failure("Uso: progress <id> <porcentaje>");
        }

        var idValidation = ValidateId(args[1], out _);
        if (!idValidation.IsValid)
        {
            return idValidation;
        }

        var percentValidation = ValidatePercent(args[2], out _);
        if (!percentValidation.IsValid)
        {
            return percentValidation;
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidateId(string idString, out int id)
    {
        if (!int.TryParse(idString, out id))
        {
            return ValidationResult.Failure("ID inválido");
        }
        return ValidationResult.Success();
    }

    public ValidationResult ValidatePercent(string percentString, out decimal percent)
    {
        if (!decimal.TryParse(percentString, out percent))
        {
            return ValidationResult.Failure("Porcentaje inválido");
        }
        return ValidationResult.Success();
    }
}
