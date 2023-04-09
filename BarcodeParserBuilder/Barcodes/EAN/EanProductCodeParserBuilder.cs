using System.Linq;
using BarcodeParserBuilder.Exceptions.EAN;
using BarcodeParserBuilder.Infrastructure;

namespace BarcodeParserBuilder.Barcodes.EAN
{
    internal class EanProductCodeParserBuilder : BaseFieldParserBuilder<ProductCode?>
    {
        protected override ProductCode? Parse(string? value) => ProductCode.ParseGtin(value);
        protected override string? Build(ProductCode? obj) => string.IsNullOrWhiteSpace(obj?.Code) ? null : obj.Code;

        protected override bool Validate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            if (!value.All(char.IsDigit) || (value.Length < 6 || value.Length > 13))
                throw new EanValidateException($"Invalid Ean value '{value}'.");

            return true;
        }

        protected override bool ValidateObject(ProductCode? obj)
        {
            if (obj == null)
                return true;

            if (obj.Type != ProductCodeType.EAN)
                throw new EanValidateException($"Invalid ProductCode type '{obj.Type}'.");

            return true;
        }
    }
}
