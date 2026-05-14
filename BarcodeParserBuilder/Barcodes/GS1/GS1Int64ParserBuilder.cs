using System.Globalization;
using BarcodeParserBuilder.Exceptions.GS1;

namespace BarcodeParserBuilder.Barcodes.GS1;

internal class GS1Int64ParserBuilder : BaseFieldParserBuilder<Int64?>
{
    protected override string? Build(Int64? obj)
    {
        if (!obj.HasValue)
            return null;

        return obj.Value.ToString();
    }

    protected override Int64? Parse(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        return Int64.Parse(value);
    }

    protected override bool ValidateObjectLength(Int64? obj, int? minimumLength, int? maximumLength)
    {
        if (!obj.HasValue)
            return true;

        var valueString = obj.Value.ToString(CultureInfo.InvariantCulture);
        return valueString.Length <= (maximumLength ?? Int64.MaxValue) && valueString.Length >= (minimumLength ?? 0);
    }

    protected override bool Validate(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return true;

        if (!value.All(char.IsDigit))
            throw new GS1ValidateException($"Invalid GS1 int64 value '{value}'.");

        return true;
    }
}
