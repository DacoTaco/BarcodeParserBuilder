using BarcodeParserBuilder.Exceptions.PPN;
using BarcodeParserBuilder.Infrastructure;
using System;
using System.IO;
using System.Linq;

namespace BarcodeParserBuilder.Barcodes.PPN
{
    public class PpnBarcodeParserBuilder : BaseBarcodeParserBuilder<PpnBarcode>
    {
        protected PpnBarcodeParserBuilder() { }

        public static bool TryParse(string? barcode, out PpnBarcode? ppnBarcode)
        {
            try
            {
                ppnBarcode = Parse(barcode);
                return true;
            }
            catch
            {
                ppnBarcode = null;
            }
            return false;
        }

        public static PpnBarcode? Parse(string? barcode)
        {
            var parserBuider = new PpnBarcodeParserBuilder();
            return parserBuider.ParseString(barcode);
        }

        public static string? Build(PpnBarcode? barcode)
        {
            if (barcode == null)
                return null;

            var parserBuider = new PpnBarcodeParserBuilder();
            return parserBuider.BuildString(barcode);
        }

        protected override string? BuildString(PpnBarcode? barcode)
        {

            if (barcode == null)
                return null;

            var barcodeString = "";
            foreach (var field in barcode.Fields)
            {
                var value = field.Build();

                if (string.IsNullOrWhiteSpace(value))
                    continue;

                barcodeString += $"{(string.IsNullOrWhiteSpace(barcodeString) ? "" : PpnBarcode.GroupSeparator.ToString())}{field.Identifier}{value}";
            }
            
            return $"{PpnBarcode.Prefix}{barcodeString}{PpnBarcode.Suffix}";
        }

        protected override PpnBarcode? ParseString(string? barcodeString)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(barcodeString))
                    return default;

                barcodeString = AimParser.StripBarcodePrefix(barcodeString);
                if (!barcodeString.StartsWith(PpnBarcode.Prefix, StringComparison.Ordinal) || 
                    !barcodeString.EndsWith(PpnBarcode.Suffix, StringComparison.Ordinal) ||
                    barcodeString.Length < (PpnBarcode.Prefix.Length + PpnBarcode.Suffix.Length) )
                    throw new PPNParseException("Invalid PPN Barcode Prefix/Suffix/Size");

                
                barcodeString = barcodeString[PpnBarcode.Prefix.Length..(barcodeString.Length - PpnBarcode.Suffix.Length)]; //remove prefix & suffix

                var barcode = new PpnBarcode();
                var codeStream = new StringReader(barcodeString);
                var applicationIdentifier = "";
                while (codeStream.Peek() > -1)
                {
                    var character = (char)codeStream.Read();
                    applicationIdentifier += character;

                    if (!applicationIdentifier.All(char.IsLetterOrDigit) || applicationIdentifier.Any(x => x == '\0') || string.IsNullOrWhiteSpace(applicationIdentifier))
                        throw new PPNParseException($"Invalid character detected in AI '{applicationIdentifier}'.");

                    if (applicationIdentifier.Length > 3 || barcode.Fields.Contains(applicationIdentifier))
                    {
                        if (codeStream.Peek() < 0)
                            throw new PPNParseException("Garbage barcode detected.");

                        if (barcode.Fields.Contains(applicationIdentifier))
                            barcode.Fields[applicationIdentifier].Parse(codeStream);

                        applicationIdentifier = "";

                        //skip all group separators
                        while (codeStream.Peek() == PpnBarcode.GroupSeparator ||
                            codeStream.Peek() == PpnBarcode.RecordSeparator ||
                            codeStream.Peek() == PpnBarcode.EndOfTransmission)
                        {
                            codeStream.Read();
                        }
                    }
                }

                if (barcode.Fields["9N"].Value != null && barcode.Fields["8P"].Value != null)
                    throw new PPNParseException("Barcode can not contain both a PPN and GTIN.");

                return barcode;
            }
            catch (Exception e)
            {
                throw new PPNParseException($"Failed to parse PPN Barcode :{Environment.NewLine}{e.Message}", e);
            }
        }
    }
}
