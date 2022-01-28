using BarcodeParserBuilder.Abstraction;
using BarcodeParserBuilder.Barcodes;
using System.Collections.Generic;

namespace BarcodeParserBuilder.Infrastructure
{
    public abstract class BaseBarcodeParserBuilder<T> : IBaseBarcodeParserBuilder where T : Barcode
    {
        internal static int ParsingOrderNumber => 0;

        protected abstract T ParseString(string barcodeString);
        protected abstract string BuildString(T barcode);
        protected virtual IList<string> BuildBarcodes(T barcode) => new List<string>() { BuildString(barcode) };
    }
}
