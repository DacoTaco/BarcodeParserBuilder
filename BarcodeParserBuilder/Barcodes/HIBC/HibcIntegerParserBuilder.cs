using BarcodeParserBuilder.Exceptions.HIBC;
using BarcodeParserBuilder.Infrastructure;
using System.Linq;

namespace BarcodeParserBuilder.Barcodes.HIBC
{
    internal class HibcIntegerParserBuilder : BaseFieldParserBuilder<int?>
    {
        protected override string Build(int? obj) => obj?.ToString();

        protected override int? Parse(string value) => string.IsNullOrWhiteSpace(value) ? (int?)null : int.Parse(value);

        protected override bool Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            if (value.Any(c => !char.IsDigit(c)))
                throw new HIBCValidateException($"Invalid HIBC value '{value}'.");

            return true;
        }
    }
}
