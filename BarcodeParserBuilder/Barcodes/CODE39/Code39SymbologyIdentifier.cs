namespace BarcodeParserBuilder.Barcodes.CODE39
{
    public class Code39SymbologyIdentifier : AimSymbologyIdentifier
    {

        public const string NoFullASCIINoChecksumValue = "A0";
        public const string NoFullASCIIMod43ChecksumTransmittedValue = "A1";
        public const string NoFullASCIIMod43ChecksumStrippedValue = "A2";
        public const string FullASCIIOlnyModChecksumValue = "A3";
        public const string FullASCIINoChecksumValue = "A4";
        public const string FullASCIIMod43ChecksumTransmittedValue = "A5";
        public const string FullASCIIMod43ChecksumStrippedValue = "A7";

        public Code39SymbologyIdentifier() { }

        public Code39SymbologyIdentifier(string symbologyIdentifier) : base(symbologyIdentifier) { }

        public static Code39SymbologyIdentifier Invalid => new("");

        public static Code39SymbologyIdentifier NoFullASCIINoChecksum => new(NoFullASCIINoChecksumValue);

        // A1 - Reader has performed mod 43 check and transmitted
        public static Code39SymbologyIdentifier NoFullASCIIMod43ChecksumTransmitted => new(NoFullASCIIMod43ChecksumTransmittedValue);

        // A2 - Reader has performed mod 43 check and stripped the check character
        public static Code39SymbologyIdentifier NoFullASCIIMod43ChecksumStripped => new(NoFullASCIIMod43ChecksumStrippedValue);

        // A3 - Reader passes only barcodes which have checksum, no full ASCII
        public static Code39SymbologyIdentifier FullASCIIOlnyModChecksum => new(FullASCIIOlnyModChecksumValue);

        // A4 - Reader has performed Full ASCII conversion. No check character validation
        public static Code39SymbologyIdentifier FullASCIINoChecksum => new(FullASCIINoChecksumValue);

        // A5 - Reader has performed Full ASCII character conversion, verfied check character and transmitted it
        public static Code39SymbologyIdentifier FullASCIIMod43ChecksumTransmitted => new(FullASCIIMod43ChecksumTransmittedValue);

        // A7 - Reader has performed Full ASCII character conversion, verified check character and stripped it
        public static Code39SymbologyIdentifier FullASCIIMod43ChecksumStripped => new(FullASCIIMod43ChecksumStrippedValue);
    }
}
