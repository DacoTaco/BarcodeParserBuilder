using BarcodeParserBuilder.Barcodes.CODE39;
using BarcodeParserBuilder.Exceptions.CODE39;
using BarcodeParserBuilder.Exceptions.EAN;
using BarcodeParserBuilder.Infrastructure;

namespace BarcodeParserBuilder.Barcodes.CODE39
{
    internal class Code39ProductCodeParserBuilder : BaseFieldParserBuilder<ProductCode?>
    {
        protected override ProductCode? Parse(string? value) => ProductCode.ParseGtin(value);
        protected override string? Build(ProductCode? obj) => string.IsNullOrWhiteSpace(obj?.Code) ? null : obj!.Code;

        protected override bool Validate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            if (! Code39StringParserBuilder.ValidateFullASCII(value))
                throw new Code39ValidateException($"Invalid Code39 value '{value}'.");

            return true;
        }

        protected override bool ValidateObject(ProductCode? obj)
        {
            if (obj == null)
                return true;

            if (obj.Type != ProductCodeType.CODE39)
                throw new Code39ValidateException($"Invalid ProductCode type '{obj.Type}'.");

            return true;
        }
    }
}
