using BarcodeParserBuilder.Barcodes.GS1;
using BarcodeParserBuilder.Infrastructure;

namespace BarcodeParserBuilder
{
    public class BarcodeParserBuilder : IBarcodeParserBuilder
    {
        private static readonly IAimParser _aimParser = new AimParser();

        public bool TryParse(string? barcodeString, out Barcode? barcode) => TryParse(barcodeString, out barcode, out var _);
        public bool TryParse(string? barcodeString, out Barcode? barcode, out string? feedback)
        {
            try
            {
                barcode = null;
                feedback = null;

                if (string.IsNullOrWhiteSpace(barcodeString))
                    return true;

                foreach (var parserBuilder in _aimParser.GetParsers(barcodeString!))
                {
                    var methodInfo = parserBuilder.GetMethod(nameof(GS1BarcodeParserBuilder.TryParse));
                    if (methodInfo == null)
                        continue;

                    //setup parameters and execute the method
                    var tryParseParameters = new object?[2];
                    tryParseParameters[0] = barcodeString;
                    tryParseParameters[1] = barcode;
                    var returnValue = methodInfo.Invoke(null, tryParseParameters);

                    if (returnValue is not bool canParse || !canParse)
                        continue;

                    //retrieve output parameter and return true
                    barcode = (Barcode?)tryParseParameters[1];
                    return true;
                }

                throw new Exception("Failed to parse barcode : no parser could accept barcode.");
            }
            catch (Exception e)
            {
                feedback = e.Message;
                barcode = null;
                return false;
            }
        }
        public string? Build(Barcode? barcode)
        {
            if (barcode == null)
                return null;

            var parserBuilder = AimParser.ParserBuilders.SingleOrDefault(c => c.BaseType?.GenericTypeArguments?.Any(t => t == barcode.GetType()) ?? false);
            var methodInfo = parserBuilder?.GetMethod(nameof(GS1BarcodeParserBuilder.Build));

            if (methodInfo == null)
                return null;

            var buildParameters = new object[] { barcode };
            var returnValue = methodInfo.Invoke(null, buildParameters);

            return (string?)returnValue;
        }
    }
}
