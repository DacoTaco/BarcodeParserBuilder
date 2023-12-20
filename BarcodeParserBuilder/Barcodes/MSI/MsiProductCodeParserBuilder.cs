using BarcodeParserBuilder.Exceptions.MSI;

namespace BarcodeParserBuilder.Barcodes.MSI;

internal class MsiProductCodeParserBuilder : BaseFieldParserBuilder<ProductCode?>
{
    protected override string? Build(ProductCode? obj) => string.IsNullOrWhiteSpace(obj?.Code) ? null : obj!.Code;
    protected override ProductCode? Parse(string? value) => ProductCode.ParseMsi(value);

    protected override bool Validate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return true;

        if (value!.Length < 3 || value.Any(c => !char.IsDigit(c)))
            throw new MsiValidateException($"Invalid MSI value '{value}'.");

        return true;
    }

    protected override bool ValidateObject(ProductCode? obj)
    {
        if (obj == null)
            return true;

        if (obj.Type != ProductCodeType.MSI)
            throw new MsiValidateException($"Invalid ProductCode type '{obj.Type}'.");

        return true;
    }
}
