namespace TodoApp.Application.Common.Security;

public static class SecurityExtensions
{
    /// <summary>
    /// Sanitiza una cadena de forma segura
    /// </summary>
    public static string Sanitize(this string? input)
    {
        return InputSanitizer.SanitizeString(input);
    }

    /// <summary>
    /// Valida que la cadena sea segura contra inyección SQL
    /// </summary>
    public static bool IsSafeSql(this string? input)
    {
        return InputSanitizer.IsSafeFromSqlInjection(input);
    }

    /// <summary>
    /// Valida que la cadena solo contenga caracteres alfanuméricos
    /// </summary>
    public static bool IsAlphanumeric(this string? input)
    {
        return InputSanitizer.IsAlphanumericWithSpaces(input);
    }
}
