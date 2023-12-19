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

        public static bool TryParse(string? barcode, AimSymbologyIdentifier? symbologyIdentifier, out Code39Barcode? code39Barcode)
        {
            try
            {
                code39Barcode = Parse(barcode, symbologyIdentifier);
                return true;
            }
            catch
            {
                code39Barcode = null;
            }
            return false;
        }

        public static Code39Barcode? Parse(string? barcode, AimSymbologyIdentifier? symbologyIdentifier)
        {
            var parserBuider = new Code39BarcodeParserBuilder();
            return parserBuider.ParseString(barcode, symbologyIdentifier);
        }

        protected override string? BuildString(Code39Barcode? barcode)
        {
            if (barcode?.ProductCode?.Code == null)
                return string.Empty;

            var barcodeStr = $"{barcode.Fields[nameof(barcode.ProductCode)].Build()}";
            if (barcode.ReaderInformation is Code39SymbologyIdentifier symbologyIdentifier)
                barcodeStr = $"{barcodeStr}{Code39Checksum.GetBarcodeCheckCharacter(barcodeStr, symbologyIdentifier)}";

            if (!string.IsNullOrWhiteSpace(barcode.ReaderInformation?.SymbologyIdentifier))
                barcodeStr = $"]{barcode.ReaderInformation!.SymbologyIdentifier}{barcodeStr}";

            return barcodeStr;
        }

        protected override Code39Barcode? ParseString(string? inputBarcode, AimSymbologyIdentifier? symbologyIdentifier)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(inputBarcode))
                    return null;

                if (symbologyIdentifier is not Code39SymbologyIdentifier code39identifier)
                    throw new Code39ParseException("Invalid Code39 SymbologyIdentifier");

                inputBarcode = code39identifier.StripSymbologyIdentifier(inputBarcode!);

                // Reading is validated now in the context of obtained identifier information 
                // Same reading may give different validation results depending on the AIM identifier
                if (!Validate(inputBarcode, code39identifier))
                    throw new Code39ParseException("Code content does not match reader information");

                // When the check character is transmitted, we strip it from the entire reading, 
                // because Code39 does not have any specific structure and check character applies to entire reading
                string strippedInput = Code39Checksum.ValidateAndStripChecksum(inputBarcode, code39identifier);

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
