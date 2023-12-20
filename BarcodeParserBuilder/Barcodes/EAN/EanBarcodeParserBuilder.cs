using BarcodeParserBuilder.Exceptions.EAN;

namespace BarcodeParserBuilder.Barcodes.EAN;

public class EanBarcodeParserBuilder : BaseBarcodeParserBuilder<EanBarcode>
{
    protected EanBarcodeParserBuilder() { }

    public static string? Build(EanBarcode? barcode)
    {
        var parserBuider = new EanBarcodeParserBuilder();
        return parserBuider.BuildString(barcode);
    }

    public static bool TryParse(string? barcode, AimSymbologyIdentifier? symbologyIdentifier, out EanBarcode? eanBarcode)
    {
        try
        {
            eanBarcode = Parse(barcode, symbologyIdentifier);
            return true;
        }
        catch
        {
            eanBarcode = null;
        }
        return false;
    }

    public static EanBarcode? Parse(string? barcode, AimSymbologyIdentifier? symbologyIdentifier)
    {
        var parserBuider = new EanBarcodeParserBuilder();
        return parserBuider.ParseString(barcode, symbologyIdentifier);
    }

    protected override string? BuildString(EanBarcode? barcode) => barcode?.Fields[nameof(barcode.ProductCode)].Build();

    protected override EanBarcode? ParseString(string? inputBarcode, AimSymbologyIdentifier? symbologyIdentifier)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(inputBarcode))
                return null;

            inputBarcode = symbologyIdentifier?.StripSymbologyIdentifier(inputBarcode!) ?? inputBarcode!;
            return new EanBarcode(symbologyIdentifier as EanSymbologyIdentifier)
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
