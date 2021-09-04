using System.Collections.Generic;

namespace BarcodeParserBuilder.Infrastructure
{
    public interface IBarcodeParserBuilder { }
    public abstract class BaseBarcodeParserBuilder<T> : IBarcodeParserBuilder
        where T : Barcode
    {
        internal static int ParsingOrderNumber => 0;

        protected abstract T ParseString(string barcodeString);
        protected abstract string BuildString(T barcode);
        protected virtual IList<string> BuildBarcodes(T barcode) => new List<string>() { BuildString(barcode) };
    }
}
