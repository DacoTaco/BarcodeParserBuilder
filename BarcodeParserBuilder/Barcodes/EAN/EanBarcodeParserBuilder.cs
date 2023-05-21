using BarcodeParserBuilder.Exceptions.EAN;
using BarcodeParserBuilder.Infrastructure;

namespace BarcodeParserBuilder.Barcodes.EAN
{
    public class EanBarcodeParserBuilder : BaseBarcodeParserBuilder<EanBarcode>
    {
        protected EanBarcodeParserBuilder() { }

        public static string? Build(EanBarcode? barcode)
        {
            var parserBuider = new EanBarcodeParserBuilder();
            return parserBuider.BuildString(barcode);
        }

        public static bool TryParse(string? barcode, out EanBarcode? eanBarcode)
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

        public static EanBarcode? Parse(string? barcode)
        {
            var parserBuider = new EanBarcodeParserBuilder();
            return parserBuider.ParseString(barcode);
        }

        protected override string? BuildString(EanBarcode? barcode) => barcode?.ProductCode?.Code;

        protected override EanBarcode? ParseString(string? inputBarcode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(inputBarcode))
                    return null;

                inputBarcode = AimParser.StripBarcodePrefix(inputBarcode);
                return new EanBarcode()
                {
                    ProductCode = new GtinProductCode(inputBarcode)
                };
            }
            catch (Exception e)
            {
                throw new EanParseException($"Failed to parse Ean Barcode :{Environment.NewLine}{e.Message}", e);
            }
        }
    }
}
