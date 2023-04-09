using System;
using System.Collections.Generic;
using System.Linq;
using BarcodeParserBuilder.Exceptions.GS1;

namespace BarcodeParserBuilder.Barcodes.GS1
{
    public class GS1128BarcodeParserBuilder : BaseGS1BarcodeParserBuilder<GS1128Barcode>
    {
        protected GS1128BarcodeParserBuilder() { }

        public static bool TryParse(string? barcode, out GS1128Barcode? gs1128Barcode)
        {
            try
            {
                gs1128Barcode = Parse(barcode);
                return true;
            }
            catch
            {
                gs1128Barcode = null;
            }
            return false;
        }

        public static GS1128Barcode? Parse(string? barcode)
        {
            var parserBuider = new GS1128BarcodeParserBuilder();
            return parserBuider.ParseString(barcode);
        }

        public static string? Build(GS1128Barcode? barcode)
        {
            if (barcode == null)
                return null;

            var parserBuider = new GS1128BarcodeParserBuilder();
            return parserBuider.BuildString(barcode);
        }

        public static IList<string> BuildList(GS1128Barcode? barcode) => new GS1128BarcodeParserBuilder().BuildBarcodes(barcode);

        protected override GS1128Barcode? ParseString(string? barcodeString)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(barcodeString))
                    return null;

                if (!barcodeString.StartsWith(GS1128Barcode.SymbologyPrefix, StringComparison.Ordinal))
                    throw new GS1128ParseException("Barcode does not start with the Symbology Prefix.");

                barcodeString = barcodeString.Replace(GS1128Barcode.SymbologyPrefix, GS1Barcode.GroupSeparator.ToString());
                return base.ParseString(barcodeString);
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

                list.Add($"{GS1128Barcode.SymbologyPrefix}{field.Identifier}{value}");
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
}
