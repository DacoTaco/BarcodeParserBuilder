using System.Linq;
using BarcodeParserBuilder.Exceptions.PPN;
using BarcodeParserBuilder.Infrastructure;

namespace BarcodeParserBuilder.Barcodes.PPN
{
    internal class PpnStringParserBuilder : BaseFieldParserBuilder<string?>
    {
        protected override string? Build(string? obj) => string.IsNullOrWhiteSpace(obj) ? null : obj;
        protected override string? Parse(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;

        protected override bool Validate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            if (!value.All(char.IsLetterOrDigit))
                throw new PPNValidateException($"Invalid PPN string value '{value}'.");

            return true;
        }
    }
}
