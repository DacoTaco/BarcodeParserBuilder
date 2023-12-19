namespace BarcodeParserBuilder.Aim
{
    public class Code128SymbologyIdentifier : AimSymbologyIdentifier
    {
        // From the ISO/IEC 15424:2008(E) standard:
        // 0 Standard data packet.No FNC1 in first or second symbol character position after start character
        // 1 GS1-128 data packet - FNC1 in first symbol character position after start character
        // 2 FNC1 in second symbol character position after start character
        // 4 Concatenation according to International Society for Blood Transfusion specifications has been
        //    performed; concatenated data follows

        public const string StandardNoFNC1Value = "C0";
        public const string FNC1InFirstSymbolValue = "C1";
        public const string FNC1InSecondSymbolValue = "C2";
        public const string ISBTConcatenatedValue = "C4";

        public Code128SymbologyIdentifier(string symbologyIdentifier) : base(symbologyIdentifier)
        {
            if (symbologyIdentifier.ElementAtOrDefault(0) != 'C' || !int.TryParse(symbologyIdentifier.ElementAtOrDefault(1).ToString(), out var modifier) || modifier < 0 || modifier > 4)
                throw new InvalidDataException($"Invalid Code128 symbology : '{symbologyIdentifier}'");
        }
    }
}

