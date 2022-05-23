using System;
using System.Linq;

namespace BarcodeParserBuilder.Infrastructure
{
    public partial class ProductCode
    {
        public static ProductCode? ParseEan(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (value.Length < 3 || value.Length > 13 || !value.All(char.IsDigit))
                throw new ArgumentException($"Invalid Ean Product Code '{value}'.");

            return new ProductCode(value, ProductCodeType.EAN);
        }

        public static ProductCode? ParseNdc(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (value.Length != 10 || !value.All(char.IsDigit))
                throw new ArgumentException($"Invalid NDC value '{value}'.");

            return new ProductCode(value, ProductCodeType.NDC);
        }
    }
}
