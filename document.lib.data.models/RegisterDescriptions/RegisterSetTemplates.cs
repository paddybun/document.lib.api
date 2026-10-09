namespace document.lib.data.models.RegisterDescriptions;

public static class RegisterSetTemplates
{
    public static List<string> FromText(string? text) =>
        (text ?? string.Empty)
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
}
