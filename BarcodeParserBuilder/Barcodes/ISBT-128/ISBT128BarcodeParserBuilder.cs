namespace BarcodeParserBuilder.Barcodes.ISBT128
{
    public class ISBT128BarcodeParserBuilder : BaseBarcodeParserBuilder<ISBT128Barcode>
    {
        protected ISBT128BarcodeParserBuilder() { }

        public static bool TryParse(string? barcode, out ISBT128Barcode? isbt128Barcode)
        {
            try
            {
                isbt128Barcode = Parse(barcode);
                return true;
            }
            catch
            {
                isbt128Barcode = null;
            }
            return false;
        }

        public static ISBT128Barcode? Parse(string? barcode)
        {
            var parserBuider = new ISBT128BarcodeParserBuilder();
            return parserBuider.ParseString(barcode);
        }

        public static string? Build(ISBT128Barcode? barcode)
        {
            if (barcode == null)
                return null;

            var parserBuider = new ISBT128BarcodeParserBuilder();
            return parserBuider.BuildString(barcode);
        }

        protected override string? BuildString(ISBT128Barcode? barcode)
        {
            throw new NotImplementedException();
        }

        protected override ISBT128Barcode ParseString(string? barcodeString)
        {
            throw new NotImplementedException();
        }
    }
}
