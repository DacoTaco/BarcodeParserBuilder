using BarcodeParserBuilder.Exceptions.GS1;

namespace BarcodeParserBuilder.Barcodes.GS1;

public class GS1128BarcodeParserBuilder : BaseGS1BarcodeParserBuilder<GS1128Barcode>
{
    protected GS1128BarcodeParserBuilder() { }

    public static bool TryParse(string? barcode, AimSymbologyIdentifier? symbologyIdentifier, out GS1128Barcode? gs1128Barcode)
    {
        try
        {
            gs1128Barcode = Parse(barcode, symbologyIdentifier);
            return true;
        }
        catch
        {
            gs1128Barcode = null;
        }
        return false;
    }

    public static GS1128Barcode? Parse(string? barcode, AimSymbologyIdentifier? symbologyIdentifier)
    {
        var parserBuider = new GS1128BarcodeParserBuilder();
        return parserBuider.ParseString(barcode, symbologyIdentifier);
    }

    public static string? Build(GS1128Barcode? barcode)
    {
        if (barcode == null)
            return null;

        var parserBuider = new GS1128BarcodeParserBuilder();
        return parserBuider.BuildString(barcode);
    }

    public static IList<string> BuildList(GS1128Barcode? barcode) => new GS1128BarcodeParserBuilder().BuildBarcodes(barcode);

    protected override GS1128Barcode? ParseString(string? barcodeString, AimSymbologyIdentifier? symbologyIdentifier)
    {
        try
        {
            if (symbologyIdentifier is not Code128SymbologyIdentifier identifier)
                throw new GS1128ParseException($"Invalid GS1-128 identifier.");

            if (string.IsNullOrWhiteSpace(barcodeString))
                return null;

            if (identifier.SymbologyIdentifier != Code128SymbologyIdentifier.FNC1InFirstSymbolValue)
                throw new GS1128ParseException("Barcode does not start with the Symbology Prefix.");

            barcodeString = identifier.StripSymbologyIdentifier(barcodeString!).Replace($"]{Code128SymbologyIdentifier.FNC1InFirstSymbolValue}", GS1Barcode.GroupSeparator.ToString());
            return base.ParseString(barcodeString, symbologyIdentifier);
        }
        catch (Exception e)
        {
            throw new GS1128ParseException($"Failed to parse GS1-128 Barcode :{Environment.NewLine}{e.Message}", e);
        }
    }

    protected override IList<string> BuildBarcodes(GS1128Barcode? barcode)
    {
        var list = new List<string>();
        if (barcode == null)
            return list;

        foreach (var field in barcode.Fields.OrderBy(f => f.Identifier))
        {
            var value = field.Build();

            if (string.IsNullOrWhiteSpace(value))
                continue;

            list.Add($"]{barcode.ReaderInformation!.SymbologyIdentifier}{field.Identifier}{value}");
        }

        return list;
    }

    protected override string? BuildString(GS1128Barcode? barcode)
    {
        var list = BuildBarcodes(barcode);
        var barcodeString = list.Select(s => s).Aggregate((i, s) => i + s);

        return string.IsNullOrWhiteSpace(barcodeString) ? null : barcodeString;
    }
}
