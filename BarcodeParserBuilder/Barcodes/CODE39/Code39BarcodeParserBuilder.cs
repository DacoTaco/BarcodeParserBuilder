using BarcodeParserBuilder.Exceptions.CODE39;

namespace BarcodeParserBuilder.Barcodes.CODE39
{
    internal class Code39BarcodeParserBuilder : BaseBarcodeParserBuilder<Code39Barcode>
    {
        protected Code39BarcodeParserBuilder() { }

        public static string? Build(Code39Barcode? barcode)
        {
            var parserBuider = new Code39BarcodeParserBuilder();
            return parserBuider.BuildString(barcode);
        }

        public static bool TryParse(string? barcode, out Code39Barcode? code39Barcode)
        {
            try
            {
                code39Barcode = Parse(barcode);
                return true;
            }
            catch
            {
                code39Barcode = null;
            }
            return false;
        }


        public static Code39Barcode? Parse(string? barcode)
        {
            var parserBuider = new Code39BarcodeParserBuilder();
            return parserBuider.ParseString(barcode);
        }

        protected override string? BuildString(Code39Barcode? barcode) => barcode?.SerialNumber;

        protected override Code39Barcode? ParseString(string? inputBarcode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(inputBarcode))
                    return null;

                // Try to initialize the symbology identifier. This should succeed if the reading looks like having AIM identifier
                // But it may not be from the correct set of the supported identifiers of particular barcode class
                // AimSymbologyIdentifier is not responsible of validating that although
                Code39SymbologyIdentifier code39identifier = AimSymbologyIdentifier.FromRawReading<Code39SymbologyIdentifier>(inputBarcode!);

                var dataContent = AimSymbologyIdentifier.StripSymbologyIdentifier(inputBarcode);

                // Reading is validated now in the context of obtained identifier information 
                // Same reading may give different validation results depending on the AIM identifier
                if (!Code39StringParserBuilder.Validate(dataContent, code39identifier))
                {
                    throw new Code39ParseException("Code content does not match reader information");
                }

                // When the check character is transmitted, we strip it from the entire reading, 
                // because Code39 does not have any specific structure and check character applies to entire reading
                string strippedInput = Code39Barcode.StripCheckCharacter(dataContent, code39identifier);

                // Although Code39 does not specify any structure whether the reading is ProductCode or SerialNumber
                // or something else, we initialize the ProductCode, because it is most aligned with the current implementation
                return new Code39Barcode(code39identifier)
                {
                    ProductCode = new Code39Productcode(strippedInput)
                };
            }
            catch (Exception e)
            {
                throw new Code39ParseException($"Failed to parse Code39 Barcode :{Environment.NewLine}{e.Message}", e);
            }
        }
    }
}
