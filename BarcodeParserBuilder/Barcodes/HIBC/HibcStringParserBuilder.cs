using BarcodeParserBuilder.Exceptions.HIBC;
using BarcodeParserBuilder.Infrastructure;

namespace BarcodeParserBuilder.Barcodes.HIBC
{
    internal class HibcStringParserBuilder : BaseFieldParserBuilder<string?>
    {
        protected override string? Build(string? obj) => string.IsNullOrWhiteSpace(obj) ? null : obj;

        protected override string? Parse(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;

        protected override bool Validate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            if (value.Any(c => !char.IsLetterOrDigit(c)) || value.Where(c => char.IsLetter(c)).Any(c => !char.IsUpper(c)))
                throw new HIBCValidateException($"Invalid HIBC value '{value}'.");

            return true;
        }
    }
}
