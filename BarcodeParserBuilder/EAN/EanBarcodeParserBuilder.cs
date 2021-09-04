using BarcodeParserBuilder.Exceptions.EAN;
using BarcodeParserBuilder.Infrastructure;
using System;

namespace BarcodeParserBuilder.EAN
{
    public class EanBarcodeParserBuilder : BaseBarcodeParserBuilder<EanBarcode>
    {
        protected EanBarcodeParserBuilder() { }

        public static string Build(EanBarcode barcode)
        {
            var parserBuider = new EanBarcodeParserBuilder();
            return parserBuider.BuildString(barcode);       
        }

        public static bool TryParse(string barcode, out EanBarcode eanBarcode)
        {
            try
            {
                eanBarcode = Parse(barcode);
                return true;
            }
            catch
            {
                eanBarcode = null;
            }
            return false;
        }

        public static EanBarcode Parse(string barcode)
        {
            var parserBuider = new EanBarcodeParserBuilder();
            return parserBuider.ParseString(barcode);
        }

        protected override string BuildString(EanBarcode barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode?.ProductCode?.Code))
                return null;

            return barcode.Fields[nameof(barcode.ProductCode)].Build();
        }

        protected override EanBarcode ParseString(string barcodeString)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(barcodeString))
                    return null;

                var barcode = new EanBarcode();
                barcode.Fields[nameof(ProductCode)].Parse(barcodeString);
                return barcode;
            }
            catch (Exception e)
            {
                throw new EanParseException($"Failed to parse Ean Barcode :{Environment.NewLine}{e.Message}", e);
            }
        }
    }
}
