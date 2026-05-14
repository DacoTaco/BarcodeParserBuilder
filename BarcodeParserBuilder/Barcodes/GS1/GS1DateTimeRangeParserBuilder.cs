using BarcodeParserBuilder.Exceptions.GS1;

namespace BarcodeParserBuilder.Barcodes.GS1;

internal class GS1DateTimeRangeParserBuilder : BaseFieldParserBuilder<BarcodeDateTimeRange?>
{
    protected (string start, string end) GetFormats()
    {
        var parts = FieldFormat?.Split(BarcodeDateTimeRange.DateTimeFormatSeparator) ?? throw new GS1ValidateException("FieldFormat must be set for GS1 date range fields.");
        if (parts.Length != 2)
            throw new GS1ValidateException($"Invalid FieldFormat '{FieldFormat}' for GS1 date range. Must be in format 'startFormat|endFormat'.");

        return (parts[0], parts[1]);
    }

    protected override BarcodeDateTimeRange? Parse(string? value)
    {
        var (startFormat, endFormat) = GetFormats();
        return BarcodeDateTimeRange.Gs1Range(value, startFormat, endFormat);
    }

    protected override string? Build(BarcodeDateTimeRange? obj) => obj is null ? null : $"{obj.Start.StringValue}{obj.End.StringValue}";

    protected override bool Validate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return true;

        var (startFormat, endFormat) = GetFormats();
        var expectedLength = startFormat.Length + endFormat.Length;

        if (value!.Length != expectedLength || !value.All(char.IsDigit))
            throw new GS1ValidateException($"Invalid GS1 Date Range value '{value}'.");

        return true;
    }

    protected override bool ValidateObject(BarcodeDateTimeRange? obj)
    {
        if (obj is null)
            return true;

        var (startFormat, endFormat) = GetFormats();
        if(obj.Start.FormatString != startFormat || obj.End.FormatString != endFormat || !BarcodeDateTime.IsGS1Format(obj.Start.FormatString) || !BarcodeDateTime.IsGS1Format(obj.End.FormatString))
            throw new GS1ValidateException($"Invalid Barcode Value '{obj.Start.StringValue}{obj.End.StringValue}'.");

        return true;
    }
}
