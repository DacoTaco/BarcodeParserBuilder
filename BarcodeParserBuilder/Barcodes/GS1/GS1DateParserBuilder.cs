using BarcodeParserBuilder.Exceptions.GS1;

namespace BarcodeParserBuilder.Barcodes.GS1;

internal class GS1DateParserBuilder : BaseFieldParserBuilder<BarcodeDateTime?>
{
    protected string GetFormat() => FieldFormat ?? throw new GS1ValidateException("FieldFormat must be set for GS1 date fields.");
    protected override BarcodeDateTime? Parse(string? value) => BarcodeDateTime.Gs1Date(value, GetFormat());
    protected override string? Build(BarcodeDateTime? obj) => string.IsNullOrWhiteSpace(obj?.StringValue) ? null : obj!.StringValue;

    protected override bool Validate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return true;

        if(FieldFormat == null)
            throw new GS1ValidateException("FieldFormat must be set for GS1 date fields.");

        if (!value.All(char.IsDigit))
            throw new GS1ValidateException($"Invalid GS1 Date value '{value}'.");

        var format = GetFormat();
        if (!format.Contains(BarcodeDateTime.OptionalTimeSeparator))
        {
            if (value!.Length != format.Length)
                throw new GS1ValidateException($"Invalid GS1 Date value '{value}' for format '{format}'.");

            return true;
        }

        var splitFormats = format.Split(BarcodeDateTime.OptionalTimeSeparator);
        var formatLength = format.Replace(BarcodeDateTime.OptionalTimeSeparator.ToString(), string.Empty).Length;
        _ = splitFormats.FirstOrDefault(f => f.Length == value!.Length || value.Length == formatLength) ?? throw new GS1ValidateException($"Invalid GS1 Date value '{value}' for format '{format}'.");

        return true;
    }

    protected override bool ValidateObject(BarcodeDateTime? obj)
    {
        if (obj == null)
            return true;

        if (!Validate(obj.StringValue) || !BarcodeDateTime.IsGS1Format(obj.FormatString))
            throw new GS1ValidateException($"Invalid Barcode Value '{obj.StringValue}'.");

        return true;
    }
}
