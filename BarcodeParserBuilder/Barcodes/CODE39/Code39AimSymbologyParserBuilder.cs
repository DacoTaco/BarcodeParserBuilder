using BarcodeParserBuilder.Exceptions.CODE39;

namespace BarcodeParserBuilder.Barcodes.CODE39
{
    internal class Code39AimSymbologyParserBuilder : BaseFieldParserBuilder<AimSymbologyIdentifier?>
    {
        protected override AimSymbologyIdentifier? Parse(string? value) => value == null ? null : AimSymbologyIdentifier.ParseString<Code39SymbologyIdentifier>($"{AimSymbologyIdentifier.AimSymbologyIndicator}{value}");
        protected override string? Build(AimSymbologyIdentifier? obj) => obj == null ? null : $"]{obj!.SymbologyIdentifier}";

        protected override bool Validate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            if (value.ElementAtOrDefault(0) != 'A' || !int.TryParse(value.ElementAtOrDefault(1).ToString(), out var modifier) || modifier < 0 || modifier == 6 || modifier > 7)
                throw new Code39ValidateException($"Invalid Code39 symbology : '{value}'.");

            return true;
        }

        protected override bool ValidateObject(AimSymbologyIdentifier? obj) => obj == null || obj is Code39SymbologyIdentifier;
    }
}
