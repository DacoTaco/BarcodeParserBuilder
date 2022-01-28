using BarcodeParserBuilder.Exceptions.HIBC;
using BarcodeParserBuilder.Infrastructure;
using System.Linq;

namespace BarcodeParserBuilder.Barcodes.HIBC
{
    internal class HibcProductCodeParserBuilder : BaseFieldParserBuilder<ProductCode>
    {
        protected override string Build(ProductCode obj) => string.IsNullOrWhiteSpace(obj?.Code) ? null : obj.Code;

        protected override ProductCode Parse(string value) => ProductCode.ParseHibc(value);

        protected override bool Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            if (value.Length < 2 || 
                value.Length > 18 || 
                value.Any(c => !char.IsLetterOrDigit(c)) || 
                value.Where(c => char.IsLetter(c)).Any(c => !char.IsUpper(c)))
                throw new HIBCValidateException($"Invalid HIBC value '{value}'.");

            return true;
        }

        protected override bool ValidateObject(ProductCode obj)
        {
            if (obj != null && obj.Type != ProductCodeType.HIBC)
                throw new HIBCValidateException($"Invalid ProductCode type '{obj.Type}'.");

            return true;
        }
    }
}
