namespace JobbKalk.Core;

public sealed record CalculatorField(
    string Key,
    string Label,
    string Unit,
    string Placeholder = "",
    bool Required = true,
    double? DefaultValue = null);

public sealed record CalculatorDefinition(
    string Id,
    string Category,
    string Title,
    string Description,
    IReadOnlyList<CalculatorField> Fields,
    string? Note = null);

public sealed record ResultItem(string Label, double Value, string Unit, int Decimals = 2);

public sealed record CalculationResult(IReadOnlyList<ResultItem> Items, string? Explanation = null);
