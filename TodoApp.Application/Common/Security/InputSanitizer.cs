using System.Text;
using System.Text.RegularExpressions;

namespace TodoApp.Application.Common.Security;

public static class InputSanitizer
{
    private static readonly Regex HtmlTagsRegex = new(@"<[^>]*>", RegexOptions.Compiled);
    private static readonly Regex ScriptTagsRegex = new(@"<script[^>]*>.*?</script>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
    private static readonly Regex SqlInjectionRegex = new(@"('|(--)|;|\*|%|<|>)", RegexOptions.Compiled);
    
    private static readonly char[] DangerousChars = { '<', '>', '"', '\'', '&' };

    /// <summary>
    /// Sanitiza una cadena de texto eliminando caracteres peligrosos y etiquetas HTML
    /// </summary>
    public static string SanitizeString(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        // Eliminar scripts
        var sanitized = ScriptTagsRegex.Replace(input, string.Empty);
        
        // Eliminar etiquetas HTML
        sanitized = HtmlTagsRegex.Replace(sanitized, string.Empty);
        
        // Codificar caracteres peligrosos
        sanitized = EncodeHtmlChars(sanitized);
        
        // Normalizar espacios en blanco
        sanitized = NormalizeWhitespace(sanitized);

        return sanitized.Trim();
    }

    /// <summary>
    /// Valida que una cadena no contenga patrones de inyección SQL
    /// </summary>
    public static bool IsSafeFromSqlInjection(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return true;
        }

        return !SqlInjectionRegex.IsMatch(input);
    }

    /// <summary>
    /// Codifica caracteres HTML peligrosos
    /// </summary>
    private static string EncodeHtmlChars(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var sb = new StringBuilder(input);
        sb.Replace("&", "&amp;");
        sb.Replace("<", "&lt;");
        sb.Replace(">", "&gt;");
        sb.Replace("\"", "&quot;");
        sb.Replace("'", "&#x27;");
        
        return sb.ToString();
    }

    /// <summary>
    /// Normaliza espacios en blanco múltiples
    /// </summary>
    private static string NormalizeWhitespace(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        return Regex.Replace(input, @"\s+", " ");
    }

    /// <summary>
    /// Valida que una cadena solo contenga caracteres alfanuméricos y espacios
    /// </summary>
    public static bool IsAlphanumericWithSpaces(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return true;
        }

        return Regex.IsMatch(input, @"^[a-zA-Z0-9\s]+$");
    }

    /// <summary>
    /// Sanitiza una descripción de todo item
    /// </summary>
    public static string SanitizeTodoDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return string.Empty;
        }

        var sanitized = SanitizeString(description);
        
        // Limitar longitud
        if (sanitized.Length > 500)
        {
            sanitized = sanitized[..500];
        }

        return sanitized;
    }
}
