namespace BarcodeParserBuilder.Barcodes.GS1
{
    public class GS1128Barcode : GS1Barcode
    {
        public GS1128Barcode() : this(null) { }
        public GS1128Barcode(Code128SymbologyIdentifier? symbologyIdentifier) : base(symbologyIdentifier) { }
        public override BarcodeType BarcodeType => BarcodeType.GS1128;
    }
}
