namespace BarcodeParserBuilder.Barcodes.GS1
{
    public class GS1128Barcode : GS1Barcode
    {
        internal static string SymbologyPrefix => "]C1";
        public GS1128Barcode() : base() { }
        public override BarcodeType BarcodeType => BarcodeType.GS1128;
    }
}
