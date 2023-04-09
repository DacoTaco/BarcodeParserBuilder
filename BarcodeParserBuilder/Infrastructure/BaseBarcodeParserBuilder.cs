using System.Collections.Generic;
using BarcodeParserBuilder.Abstraction;
using BarcodeParserBuilder.Barcodes;

namespace BarcodeParserBuilder.Infrastructure
{
    public abstract class BaseBarcodeParserBuilder<T> : IBaseBarcodeParserBuilder where T : Barcode
    {
        internal static int ParsingOrderNumber => 0;

        protected abstract T? ParseString(string? barcodeString);
        protected abstract string? BuildString(T? barcode);
        protected virtual IList<string> BuildBarcodes(T barcode)
        {
            var list = new List<string>();
            var barcodeString = BuildString(barcode);
            if (!string.IsNullOrWhiteSpace(barcodeString))
                list.Add(barcodeString);

            return list;
        }
    }
}
