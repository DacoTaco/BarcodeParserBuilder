using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace BarcodeParserBuilder.Infrastructure
{
    public partial class ProductCode
    {
        //Specs : https://www.ifaffm.de/mandanten/1/documents/04_ifa_coding_system/IFA-Info_Check_Digit_Calculations_PZN_PPN_UDI_EN.pdf
        public static ProductCode? ParsePpn(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var regex = new Regex(@"^[A-Z0-9]*\d{2,2}$");
            if (!regex.IsMatch(value) || value.Length < 4 || value.Length > 22)
                throw new ArgumentException($"Invalid PPN value '{value}'.");

            //per character we get the int value, multiply it with 2+character index and then modulo 97.
            //the output should match the last 2 digits in the product code
            var checkDigit = int.Parse(value.Substring(value.Length - 2, 2));
            var productCode = value.Remove(value.Length - 2);

            var calculatedCheckDigit = productCode.Select((character, index) => character * (index + 2)).Sum();
            calculatedCheckDigit %= 97;

            if (calculatedCheckDigit != checkDigit)
                throw new ArgumentException($"Invalid PPN CheckDigit '{checkDigit:00}', Expected '{calculatedCheckDigit:00}'.");

            return new ProductCode(value, ProductCodeType.PPN);
        }
    }
}
