using BarcodeParserBuilder.Exceptions.CODE128;

namespace BarcodeParserBuilder.Barcodes.CODE128
{
    public class Code128BarcodeParserBuilder : BaseBarcodeParserBuilder<Code128Barcode>
    {
        protected Code128BarcodeParserBuilder() { }

        public static string? Build(Code128Barcode? barcode)
        {
            var parserBuilder = new Code128BarcodeParserBuilder();
            return parserBuilder.BuildString(barcode);
        }

        public static bool TryParse(string? barcode, AimSymbologyIdentifier? symbologyIdentifier, out Code128Barcode? code128Barcode)
        {
            try
            {
                code128Barcode = Parse(barcode, symbologyIdentifier);
                return true;
            }
            catch
            {
                code128Barcode = null;
            }
            return false;
        }

        public static Code128Barcode? Parse(string? barcode, AimSymbologyIdentifier? symbologyIdentifier)
        {
            var parserBuider = new Code128BarcodeParserBuilder();
            return parserBuider.ParseString(barcode, symbologyIdentifier);
        }

        protected override string? BuildString(Code128Barcode? barcode)
        {
            if (barcode?.ProductCode?.Code == null)
                return string.Empty;

            var barcodeStr = barcode.Fields[nameof(barcode.ProductCode)].Build();
            if (!string.IsNullOrWhiteSpace(barcode.ReaderInformation?.SymbologyIdentifier))
                barcodeStr = $"]{barcode.ReaderInformation!.SymbologyIdentifier}{barcodeStr}";

            return barcodeStr;
        }

        protected override Code128Barcode? ParseString(string? inputBarcode, AimSymbologyIdentifier? symbologyIdentifier)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(inputBarcode))
                    return null;

                if (symbologyIdentifier is not Code128SymbologyIdentifier code128identifier)
                    throw new Code128ParseException("Invalid Code128 Identifier");

                if (code128identifier.SymbologyIdentifier != Code128SymbologyIdentifier.StandardNoFNC1Value)
                    throw new Code128ParseException("Not a standard Code128 barcode by the symbology identifier");

                // Although Code128 does not specify any structure whether the reading is ProductCode or SerialNumber
                // or something else, we initialize the ProductCode, because it is most aligned with the current implementation
                inputBarcode = code128identifier.StripSymbologyIdentifier(inputBarcode!);
                return new Code128Barcode(code128identifier)
                {
                    ProductCode = new Code128ProductCode(inputBarcode)
                };
            }
            catch (Exception e)
            {
                throw new Code128ParseException($"Failed to parse Code128 Barcode :{Environment.NewLine}{e.Message}", e);
            }
        }
    }
}
