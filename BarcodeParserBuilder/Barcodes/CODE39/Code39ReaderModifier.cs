using System;
using System.Collections.Generic;
using System.Text;
using BarcodeParserBuilder.Infrastructure;

namespace BarcodeParserBuilder.Barcodes.CODE39
{
    public class Code39ReaderModifier : AimReaderModifier
    {

        public const string NoFullASCIINoChecksumValue = "A0";
        public const string NoFullASCIIMod43ChecksumTransmittedValue = "A1";
        public const string NoFullASCIIMod43ChecksumStrippedValue = "A2";
        public const string FullASCIIOlnyModChecksumValue = "A3";
        public const string FullASCIINoChecksumValue = "A4";
        public const string FullASCIIMod43ChecksumTransmittedValue = "A5";
        public const string FullASCIIMod43ChecksumStrippedValue = "A7";
        

        public Code39ReaderModifier(string readerModifierType) : base(readerModifierType) { }

        public static Code39ReaderModifier Invalid => new("");

        public static Code39ReaderModifier NoFullASCIINoChecksum => new(NoFullASCIINoChecksumValue);

        // A1 - Reader has performed mod 43 check and transmitted
        public static Code39ReaderModifier NoFullASCIIMod43ChecksumTransmitted => new(NoFullASCIIMod43ChecksumTransmittedValue);

        // A2 - Reader has performed mod 43 check and stripped the check character
        public static Code39ReaderModifier NoFullASCIIMod43ChecksumStripped => new(NoFullASCIIMod43ChecksumStrippedValue);

        // A3 - Reader passes only barcodes which have checksum, no full ASCII
        public static Code39ReaderModifier FullASCIIOlnyModChecksum => new(FullASCIIOlnyModChecksumValue);

        // A4 - Reader has performed Full ASCII conversion. No check character validation
        public static Code39ReaderModifier FullASCIINoChecksum => new(FullASCIINoChecksumValue);

        // A5 - Reader has performed Full ASCII character conversion, verfied check character and transmitted it
        public static Code39ReaderModifier FullASCIIMod43ChecksumTransmitted => new(FullASCIIMod43ChecksumTransmittedValue);

        // A7 - Reader has performed Full ASCII character conversion, verified check character and stripped it
        public static Code39ReaderModifier FullASCIIMod43ChecksumStripped => new(FullASCIIMod43ChecksumStrippedValue);
    }
}
