using BarcodeParserBuilder.Exceptions.GS1;
using BarcodeParserBuilder.Infrastructure;

namespace BarcodeParserBuilder.Barcodes.GS1
{
    internal class GS1ProductCodeParserBuilder : BaseFieldParserBuilder<ProductCode?>
    {
        protected override ProductCode? Parse(string? value) => ProductCode.ParseGtin(value);
        protected override string? Build(ProductCode? obj) => string.IsNullOrWhiteSpace(obj?.Code) ? null : obj.Code;

        protected override bool Validate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            if (!value.All(char.IsDigit) || value.Length != 14)
                throw new GS1ValidateException($"Invalid GTIN value '{value}'.");

            return true;
        }

        protected override bool ValidateObject(ProductCode? obj)
        {
            if (obj == null)
                return true;

            if (obj.Type != ProductCodeType.GTIN)
                throw new GS1ValidateException($"Invalid ProductCode type '{obj.Type}'.");

            return true;
        }
    }
}
