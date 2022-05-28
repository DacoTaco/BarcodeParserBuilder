using System;
using System.Linq;

namespace BarcodeParserBuilder.Infrastructure
{
    public partial class ProductCode
    {
        public static ProductCode? ParseGtin(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (value.Length < 13 || value.Length > 14 || !value.All(char.IsDigit))
                throw new ArgumentException($"Invalid GTIN value '{value}'.");

            var schema = value.Length == 14 ? ProductCodeType.GTIN : ProductCodeType.EAN;
            var checkDigit = int.Parse(value.Last().ToString());
            //pad untill we have at least 13 characters.
            //the beauty of GTIN/EAN is thats its backwards & fowards compatible with EAN8/EAN13/UPC etc etc.
            var productCode = value.Remove(value.Length - 1).PadLeft(13, '0');

            //The checkDigit is calculated as following : 
            //every digit is added together. numbers in even positions are multiplied by 3 before being added.
            //the last digit is the remainder of the next 10 fold number minus the added numbers.
            //this is why if you were to add all numbers (including the check digit) together you should always end up with a 10 fold.
            //for more info, see : https://www.gs1.org/services/how-calculate-check-digit-manually
            //we do it slightly different though by cutting off the check digit from the string and calculating it.
            //this way we can debug and throw the exception showing the difference in check digits.

            var calculatedCheckDigit = productCode.Select((character, index) => index % 2 == 0 ? char.GetNumericValue(character) * 3 : char.GetNumericValue(character)).Sum();
            calculatedCheckDigit %= 10;
            if (calculatedCheckDigit > 0)
                calculatedCheckDigit = 10 - calculatedCheckDigit;

            if (calculatedCheckDigit != checkDigit)
                throw new ArgumentException($"Invalid GTIN CheckDigit '{checkDigit}', Expected '{calculatedCheckDigit}'.");

            return new ProductCode(value, schema);
        }
    }
}
