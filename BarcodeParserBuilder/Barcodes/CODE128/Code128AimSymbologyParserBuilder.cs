using BarcodeParserBuilder.Exceptions.CODE128;

namespace BarcodeParserBuilder.Barcodes.CODE128
{
    internal class Code128AimSymbologyParserBuilder : BaseFieldParserBuilder<AimSymbologyIdentifier?>
    {
        protected override AimSymbologyIdentifier? Parse(string? value) => value == null ? null : AimSymbologyIdentifier.ParseString<Code128SymbologyIdentifier>($"{AimSymbologyIdentifier.AimSymbologyIndicator}{value}");
        protected override string? Build(AimSymbologyIdentifier? obj) => obj == null ? null : $"]{obj!.SymbologyIdentifier}";

        protected override bool Validate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            if (value.ElementAtOrDefault(0) != 'C' || !int.TryParse(value.ElementAtOrDefault(1).ToString(), out var modifier) || modifier < 0 || modifier > 4)
                throw new Code128ValidateException($"Invalid Code128 symbology : '{value}'.");

            return true;
        }

        protected override bool ValidateObject(AimSymbologyIdentifier? obj) => obj == null || obj is Code128SymbologyIdentifier;
    }
}
