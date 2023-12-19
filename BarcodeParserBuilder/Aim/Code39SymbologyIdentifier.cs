namespace BarcodeParserBuilder.Aim
{
    public class Code39SymbologyIdentifier : AimSymbologyIdentifier
    {
        // A0 - Reader had no checksum value to give
        public const string NoFullASCIINoChecksumValue = "A0";
        // A1 - Reader has performed mod 43 check and transmitted
        public const string NoFullASCIIMod43ChecksumTransmittedValue = "A1";
        // A2 - Reader has performed mod 43 check and stripped the check character
        public const string NoFullASCIIMod43ChecksumStrippedValue = "A2";
        // A3 - Reader passes only barcodes which have checksum, no full ASCII
        public const string FullASCIIOlnyModChecksumValue = "A3";
        // A4 - Reader has performed Full ASCII conversion. No check character validation
        public const string FullASCIINoChecksumValue = "A4";
        // A5 - Reader has performed Full ASCII character conversion, verfied check character and transmitted it
        public const string FullASCIIMod43ChecksumTransmittedValue = "A5";
        // A7 - Reader has performed Full ASCII character conversion, verified check character and stripped it
        public const string FullASCIIMod43ChecksumStrippedValue = "A7";

        public Code39SymbologyIdentifier(string symbologyIdentifier) : base(symbologyIdentifier)
        {
            if (symbologyIdentifier.ElementAtOrDefault(0) != 'A' || !int.TryParse(symbologyIdentifier.ElementAtOrDefault(1).ToString(), out var modifier) || modifier < 0 || modifier == 6 || modifier > 7)
                throw new InvalidDataException($"Invalid Code39 symbology : '{symbologyIdentifier}'");
        }
    }
}
