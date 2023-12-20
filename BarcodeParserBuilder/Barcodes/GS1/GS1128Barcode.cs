namespace BarcodeParserBuilder.Barcodes.GS1;

public class GS1128Barcode(Code128SymbologyIdentifier? symbologyIdentifier) : GS1Barcode(symbologyIdentifier)
{
    public GS1128Barcode() : this(null) { }

    public override BarcodeType BarcodeType => BarcodeType.GS1128;
}
