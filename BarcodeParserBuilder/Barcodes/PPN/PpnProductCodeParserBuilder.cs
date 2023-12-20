using System.Text.RegularExpressions;
using BarcodeParserBuilder.Exceptions.PPN;

namespace BarcodeParserBuilder.Barcodes.PPN;

internal class PpnProductCodeParserBuilder : BaseFieldParserBuilder<ProductCode?>
{
    protected override string? Build(ProductCode? obj) => string.IsNullOrWhiteSpace(obj?.Code) ? null : obj!.Code;
    protected override ProductCode? Parse(string? value) => ProductCode.ParsePpn(value);

    protected override bool Validate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return true;

        var regex = new Regex(@"^[A-Z0-9]*\d{2,2}$");
        if (!regex.IsMatch(value) || value!.Length < 4 || value.Length > 22)
            throw new PPNValidateException($"Invalid PPN value '{value}'.");

        return true;
    }

    protected override bool ValidateObject(ProductCode? obj)
    {
        if (obj == null)
            return true;

        if (obj.Type != ProductCodeType.PPN)
            throw new PPNValidateException($"Invalid ProductCode type '{obj.Type}'.");

        return true;
    }
}
