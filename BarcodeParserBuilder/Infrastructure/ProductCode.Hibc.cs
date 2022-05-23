using System;
using System.Linq;

namespace BarcodeParserBuilder.Infrastructure
{
    public partial class ProductCode
    {
        public static ProductCode? ParseHibc(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (value.Length < 2 || value.Length > 18 || value.Any(c => !char.IsLetterOrDigit(c)) || value.Where(c => char.IsLetter(c)).Any(c => !char.IsUpper(c)))
                throw new ArgumentException($"Invalid HIBC value '{value}'.");

            return new ProductCode(value, ProductCodeType.HIBC);
        }
    }
}
