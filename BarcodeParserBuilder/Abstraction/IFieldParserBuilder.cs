namespace BarcodeParserBuilder.Abstraction;

public interface IFieldParserBuilder
{
    public string? FieldFormat { get; init; }
    object? Parse(object? obj, int? minimumLength, int? maximumLength);
    object? Parse(string? value, int? minimumLength, int? maximumLength);
    string? Build(object? obj);
}
