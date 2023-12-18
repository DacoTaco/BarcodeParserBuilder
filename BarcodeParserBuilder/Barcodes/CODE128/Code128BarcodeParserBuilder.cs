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

        public static bool TryParse(string? barcode, out Code128Barcode? code128Barcode)
        {
            try
            {
                code128Barcode = Parse(barcode);
                return true;
            }
            catch
            {
                code128Barcode = null;
            }
            return false;
        }

        public static Code128Barcode? Parse(string? barcode)
        {
            var parserBuider = new Code128BarcodeParserBuilder();
            return parserBuider.ParseString(barcode);
        }

        protected override string? BuildString(Code128Barcode? barcode) => barcode?.ProductCode?.Code;

        protected override Code128Barcode? ParseString(string? inputBarcode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(inputBarcode))
                    return null;

                // Try to initialize the symbology identifier. This should succeed if the reading looks like having AIM identifier
                // But it may not be from the correct set of the supported identifiers of particular barcode class
                // AimSymbologyIdentifier is not responsible of validating that although
                Code128SymbologyIdentifier code128identifier = AimSymbologyIdentifier.ParseString<Code128SymbologyIdentifier>(inputBarcode!);

                if (!code128identifier.Equals(Code128SymbologyIdentifier.StandardNoFNC1))
                    throw new Code128ParseException("Not a standard Code128 barcode by the symbology identifier");

                var dataContent = AimSymbologyIdentifier.StripSymbologyIdentifier(inputBarcode!);

                // Reading is validated now in the context of obtained identifier information 
                // Same reading may give different validation results depending on the AIM identifier
                if (!Code128StringParserBuilder.Validate(dataContent, code128identifier))
                    throw new Code128ParseException("Code content does not match reader information");

                // Although Code128 does not specify any structure whether the reading is ProductCode or SerialNumber
                // or something else, we initialize the ProductCode, because it is most aligned with the current implementation
                return new Code128Barcode(code128identifier)
                {
                    ProductCode = new Code128ProductCode(dataContent)
                };
            }
            catch (Exception e)
            {
                throw new Code128ParseException($"Failed to parse Code128 Barcode :{Environment.NewLine}{e.Message}", e);
            }
        }
    }
}
