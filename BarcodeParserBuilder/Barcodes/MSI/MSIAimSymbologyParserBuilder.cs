using BarcodeParserBuilder.Exceptions.MSI;

namespace BarcodeParserBuilder.Barcodes.MSI;

internal class MSIAimSymbologyParserBuilder : BaseFieldParserBuilder<AimSymbologyIdentifier?>
{
    protected override AimSymbologyIdentifier? Parse(string? value) => value == null ? null : AimSymbologyIdentifier.ParseString($"{AimSymbologyIdentifier.AimSymbologyIndicator}{value}");
    protected override string? Build(AimSymbologyIdentifier? obj) => obj == null ? null : $"]{obj!.SymbologyIdentifier}";

    protected override bool Validate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return true;

        if (value.ElementAtOrDefault(0) != 'M' || !int.TryParse(value.ElementAtOrDefault(1).ToString(), out var modifier) || modifier < 0 || modifier > 3)
            throw new MsiValidateException($"Invalid MSI symbology : '{value}'.");

        return true;
    }

    protected override bool ValidateObject(AimSymbologyIdentifier? obj) => Validate(obj?.SymbologyIdentifier);
}
