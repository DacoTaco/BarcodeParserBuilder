using BarcodeParserBuilder.Exceptions.PPN;

namespace BarcodeParserBuilder.Barcodes.PPN;

internal class PpnDateParserBuilder : BaseFieldParserBuilder<BarcodeDateTime?>
{
    protected override BarcodeDateTime? Parse(string? value)
    {
        if (value == null)
            return null;

        return value.Length == 8
            ? BarcodeDateTime.PpnDate(value)
            : BarcodeDateTime.Gs1Date(value);
    }
    protected override string? Build(BarcodeDateTime? obj) => obj?.StringValue;

    protected override bool Validate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return true;

        if ((value!.Length != 6 && value.Length != 8) || !value.All(char.IsDigit))
            throw new PPNValidateException($"Invalid PPN Date value '{value}'.");

        return true;
    }

    protected override bool ValidateObject(BarcodeDateTime? obj)
    {
        if (obj == null)
            return true;

        if ((obj.StringValue.Length == 8 && obj.FormatString != BarcodeDateTime.PPNFormat) ||
            (obj.StringValue.Length == 6 && obj.FormatString != BarcodeDateTime.GS1Format) ||
            !Validate(obj.StringValue))
            throw new PPNValidateException($"Invalid Barcode Value '{obj.StringValue}'.");

        return true;
    }
}
