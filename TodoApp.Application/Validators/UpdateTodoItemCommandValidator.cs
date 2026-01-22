using FluentValidation;
using TodoApp.Application.Application.Commands;

namespace TodoApp.Application.Validators;

public class UpdateTodoItemCommandValidator : AbstractValidator<UpdateTodoItemCommand>
{
    public UpdateTodoItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El ID debe ser mayor que 0.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("La descripción es requerida.")
            .MinimumLength(3)
            .WithMessage("La descripción debe tener al menos 3 caracteres.")
            .MaximumLength(500)
            .WithMessage("La descripción no puede exceder 500 caracteres.")
            .Must(BeValidDescription)
            .WithMessage("La descripción contiene caracteres o patrones peligrosos.")
            .Must(NotContainSqlInjectionPatterns)
            .WithMessage("La descripción contiene patrones sospechosos.");
    }

    private bool BeValidDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return false;

        // Validar que no contenga scripts o HTML peligroso
        var lowerDesc = description.ToLowerInvariant();
        return !lowerDesc.Contains("<script") && 
               !lowerDesc.Contains("javascript:") &&
               !lowerDesc.Contains("onerror=");
    }

    private bool NotContainSqlInjectionPatterns(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return true;

        var lowerDesc = description.ToLowerInvariant();
        
        // Patrones comunes de inyección SQL
        var dangerousPatterns = new[] 
        { 
            "drop table", 
            "delete from", 
            "insert into",
            "update set",
            "exec(",
            "execute("
        };

        return !dangerousPatterns.Any(pattern => lowerDesc.Contains(pattern));
    }
}
