using FluentValidation;
using TodoApp.Application.Application.Commands;

namespace TodoApp.Application.Validators;

public class AddTodoItemCommandValidator : AbstractValidator<AddTodoItemCommand>
{
    public AddTodoItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El ID debe ser mayor que 0.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("El título es requerido.")
            .MinimumLength(3)
            .WithMessage("El título debe tener al menos 3 caracteres.")
            .MaximumLength(100)
            .WithMessage("El título no puede exceder 100 caracteres.")
            .Must(BeValidTitle)
            .WithMessage("El título contiene caracteres no permitidos.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("La descripción es requerida.")
            .MinimumLength(3)
            .WithMessage("La descripción debe tener al menos 3 caracteres.")
            .MaximumLength(500)
            .WithMessage("La descripción no puede exceder 500 caracteres.")
            .Must(BeValidDescription)
            .WithMessage("La descripción contiene caracteres o patrones peligrosos.");

        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("La categoría es requerida.")
            .MaximumLength(50)
            .WithMessage("La categoría no puede exceder 50 caracteres.")
            .Must(BeValidCategory)
            .WithMessage("La categoría contiene caracteres no permitidos.");
    }

    private bool BeValidTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return false;

        // Validar que no contenga caracteres peligrosos
        return !title.Contains('<') && !title.Contains('>') && !title.Contains('&');
    }

    private bool BeValidDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return false;

        // Validar que no contenga scripts o HTML peligroso
        var lowerDesc = description.ToLowerInvariant();
        return !lowerDesc.Contains("<script") && !lowerDesc.Contains("javascript:");
    }

    private bool BeValidCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return false;

        // Validar que solo contenga letras, números y espacios
        return System.Text.RegularExpressions.Regex.IsMatch(category, @"^[a-zA-Z0-9\s]+$");
    }
}
