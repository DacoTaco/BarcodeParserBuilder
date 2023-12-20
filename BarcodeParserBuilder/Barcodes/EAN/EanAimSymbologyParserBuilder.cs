using BarcodeParserBuilder.Exceptions.EAN;

namespace BarcodeParserBuilder.Barcodes.EAN;

internal class EanAimSymbologyParserBuilder : BaseFieldParserBuilder<AimSymbologyIdentifier?>
{
    protected override AimSymbologyIdentifier? Parse(string? value) => value == null ? null : AimSymbologyIdentifier.ParseString<EanSymbologyIdentifier>($"{AimSymbologyIdentifier.AimSymbologyIndicator}{value}");
    protected override string? Build(AimSymbologyIdentifier? obj) => obj == null ? null : $"]{obj!.SymbologyIdentifier}";

    protected override bool Validate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return true;

        if (value.ElementAtOrDefault(0) != 'E' || !int.TryParse(value.ElementAtOrDefault(1).ToString(), out var modifier) || modifier < 0 || modifier > 4)
            throw new EanValidateException($"Invalid EAN symbology : '{value}'.");

        return true;
    }

    protected override bool ValidateObject(AimSymbologyIdentifier? obj) => obj == null || obj is EanSymbologyIdentifier;
}
