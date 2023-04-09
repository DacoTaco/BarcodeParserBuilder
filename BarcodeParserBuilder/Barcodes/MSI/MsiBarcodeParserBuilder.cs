using BarcodeParserBuilder.Exceptions.MSI;
using BarcodeParserBuilder.Infrastructure;
using System;

namespace BarcodeParserBuilder.Barcodes.MSI
{
    public class MsiBarcodeParserBuilder : BaseBarcodeParserBuilder<MsiBarcode>
    {
        protected MsiBarcodeParserBuilder() { }

        public static string? Build(MsiBarcode? barcode)
        {
            if (barcode == null)
                return null;

            var parserBuider = new MsiBarcodeParserBuilder();
            return parserBuider.BuildString(barcode);       
        }

        public static bool TryParse(string? barcode, out MsiBarcode? msiBarcode)
        {
            try
            {
                msiBarcode = Parse(barcode);
                return true;
            }
            catch
            {
                msiBarcode = null;
            }
            return false;
        }

        public static MsiBarcode? Parse(string? barcode)
        {
            var parserBuider = new MsiBarcodeParserBuilder();
            return parserBuider.ParseString(barcode);
        }

        protected override string? BuildString(MsiBarcode? barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode?.ProductCode?.Code))
                return null;

            return barcode.Fields[nameof(barcode.ProductCode)].Build();
        }

        protected override MsiBarcode? ParseString(string? barcodeString)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(barcodeString))
                    return null;

                barcodeString = AimParser.StripBarcodePrefix(barcodeString);

                //technically, a MSI has no size limitation.
                //however, it tends to clash with GS1 since GS1 has so many fields and MSI's product code can clash with GS1's
                //so we limit the size to reduce the chances of them being in each other's way
                if (barcodeString.Length > 10)
                    throw new MsiParseException($"Product Code has an unsupported length({barcodeString.Length}).");

                var barcode = new MsiBarcode();
                barcode.Fields[nameof(barcode.ProductCode)].Parse(barcodeString);
                return barcode;
            }
            catch (Exception e)
            {
                throw new MsiParseException($"Failed to parse MSI Barcode :{Environment.NewLine}{e.Message}", e);
            }
        }
    }
}
