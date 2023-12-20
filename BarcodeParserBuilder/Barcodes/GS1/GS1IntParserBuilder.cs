using System.Globalization;
using BarcodeParserBuilder.Exceptions.GS1;

namespace BarcodeParserBuilder.Barcodes.GS1;

internal class GS1IntParserBuilder : BaseFieldParserBuilder<int?>
{
    protected override string? Build(int? obj)
    {
        if (!obj.HasValue)
            return null;

        return obj.Value.ToString();
    }

    protected override int? Parse(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        return int.Parse(value);
    }

    protected override bool ValidateObjectLength(int? obj, int? minimumLength, int? maximumLength)
    {
        if (!obj.HasValue)
            return true;

        var valueString = obj.Value.ToString(CultureInfo.InvariantCulture);
        return valueString.Length <= (maximumLength ?? int.MaxValue) && valueString.Length >= (minimumLength ?? 0);
    }

    protected override bool Validate(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return true;

        if (!value.All(char.IsDigit))
            throw new GS1ValidateException($"Invalid GS1 int value '{value}'.");

        return true;
    }
}
