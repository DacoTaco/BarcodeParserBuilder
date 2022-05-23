using BarcodeParserBuilder.Exceptions.EAN;
using BarcodeParserBuilder.Infrastructure;
using System.Linq;

namespace BarcodeParserBuilder.Barcodes.EAN
{
    internal class EanProductSystemParserBuilder : BaseFieldParserBuilder<EanProductSystem?>
    {
        protected override EanProductSystem? Parse(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            if (!value.All(char.IsDigit) || !int.TryParse(value, out var number) || number > 9 || number < 0)
                throw new EanParseException($"Invalid EanProductSystem '{value}'");

            return EanProductSystem.Create(number);
        }
        protected override string? Build(EanProductSystem? obj) => string.IsNullOrWhiteSpace(obj?.Value.ToString()) ? null : obj.Value.ToString();

        protected override bool Validate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            if (!value.All(char.IsDigit) || !int.TryParse(value, out var number) || number > 9 || number < 0)
                throw new EanParseException($"Invalid EanProductSystem '{value}'");

            return true;
        }
    }
}
