using FluentValidation;
using TodoApp.Application.Application.Commands;

namespace TodoApp.Application.Validators;

public class RegisterProgressionCommandValidator : AbstractValidator<RegisterProgressionCommand>
{
    public RegisterProgressionCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El ID debe ser mayor que 0.");

        RuleFor(x => x.DateTime)
            .NotEmpty()
            .WithMessage("La fecha y hora son requeridas.")
            .LessThanOrEqualTo(DateTime.Now.AddHours(1))
            .WithMessage("La fecha no puede ser mayor a la fecha actual (con tolerancia de 1 hora).")
            .GreaterThanOrEqualTo(DateTime.Now.AddYears(-1))
            .WithMessage("La fecha no puede ser anterior a un año desde hoy.");

        RuleFor(x => x.Percent)
            .InclusiveBetween(0, 100)
            .WithMessage("El porcentaje debe estar entre 0 y 100.")
            .Must(BeValidDecimalPrecision)
            .WithMessage("El porcentaje no puede tener más de 2 decimales.");

        // Validación cross-field: si el porcentaje es 100, debe ser la fecha final
        RuleFor(x => x)
            .Must(BeConsistentCompletionData)
            .WithMessage("Un progreso del 100% debe registrarse correctamente.")
            .OverridePropertyName("Percent");
    }

    private bool BeValidDecimalPrecision(decimal percent)
    {
        // Verificar que no tenga más de 2 decimales
        var rounded = Math.Round(percent, 2);
        return Math.Abs(percent - rounded) < 0.0001m;
    }

    private bool BeConsistentCompletionData(RegisterProgressionCommand command)
    {
        // Si el porcentaje es 100%, validar que sea una fecha razonable
        if (command.Percent == 100m)
        {
            return command.DateTime <= DateTime.Now.AddHours(1);
        }

        return true;
    }
}
