using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace BarcodeParserBuilder.Infrastructure
{
    public enum ProductCodeType
    {
        Unknown = 0,
        GTIN,
        EAN,
        PPN,
        MSI,
        HIBC
    }

    public class ProductCode
    {
        protected ProductCode(string value, ProductCodeType schema)
        {
            Code = value;
            Type = schema;
        }

        public string Code { get; protected set; }
        public ProductCodeType Type { get; protected set; }

        public static ProductCode ParseGtin(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (value.Length < 8 || value.Length > 14 || !value.All(char.IsDigit))
                throw new ArgumentException($"Invalid GTIN value '{value}'.");

            var schema = value.Length == 14 ? ProductCodeType.GTIN : ProductCodeType.EAN;
            var checkDigit = int.Parse(value.Last().ToString());
            //pad untill we have at least 13 characters.
            //the beauty of GTIN/EAN is thats its backwards & fowards compatible with EAN8/EAN13/UPC etc etc.
            var productCode = value.Remove(value.Length-1).PadLeft(13, '0');

            //The checkDigit is calculated as following : 
            //every digit is added together. numbers in even positions are multiplied by 3 before being added.
            //the last digit is the remainder of the next 10 fold number minus the added numbers.
            //this is why if you were to add all numbers (including the check digit) together you should always end up with a 10 fold.
            //for more info, see : https://www.gs1.org/services/how-calculate-check-digit-manually
            //we do it slightly different though by cutting off the check digit from the string and calculating it.
            //this way we can debug and throw the exception showing the difference in check digits.

            var calculatedCheckDigit = productCode.Select((character, index) => index % 2 == 0 ? char.GetNumericValue(character) * 3 : char.GetNumericValue(character)).Sum();
            calculatedCheckDigit %= 10;
            if(calculatedCheckDigit > 0)
                calculatedCheckDigit = 10 - calculatedCheckDigit;

            if (calculatedCheckDigit != checkDigit)
                throw new ArgumentException($"Invalid GTIN CheckDigit '{checkDigit}', Expected '{calculatedCheckDigit}'.");

            return new ProductCode(value, schema);
        }

        //Specs : https://www.ifaffm.de/mandanten/1/documents/04_ifa_coding_system/IFA-Info_Check_Digit_Calculations_PZN_PPN_UDI_EN.pdf
        public static ProductCode ParsePpn(string value)
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

            var calculatedCheckDigit = productCode.Select((character,index) => character * (index+2)).Sum();
            calculatedCheckDigit %= 97;

            if (calculatedCheckDigit != checkDigit)
                throw new ArgumentException($"Invalid PPN CheckDigit '{checkDigit:00}', Expected '{calculatedCheckDigit:00}'.");

            return new ProductCode(value, ProductCodeType.PPN);
        }

        ///MSI can have multiple ways to calculate check digits ... *sigh*
        ///Most common is luhn/Mod 10 - from right to left add all numbers together. numbers in even positions need to be multiplied by 2 and added together to make 1 digit. then modulo by 10
        ///See : https://en.wikipedia.org/wiki/Luhn_algorithm#Example_for_validating_check_digit
        ///Least common : no check digit. we don't support those, because wtf?
        ///Modulo 11 : https://en.wikipedia.org/wiki/MSI_Barcode#Mod_11_Check_Digit
        ///Modulo 1010 & 1110 : Combination of Mod 10/11 + mod 10
        ///Thankfully, we can apparently validate them all by doing a luhn/mod10 validation?
        public static ProductCode ParseMsi(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (value.Any(c => !char.IsDigit(c)) || value.Length < 3)
                throw new ArgumentException($"Invalid MSI value '{value}'.");

            var productCode = value
                .Reverse()
                .ToList();

            var sum = 0;
            for(int index = 0; index < productCode.Count; index++)
            {
                var character = productCode[index];
                var digit = index % 2 == 0 ? int.Parse(character.ToString()) : int.Parse(character.ToString()) * 2;

                //only 1 digit allowed. add numbers together, or subtract it with 9. 18 -> 9, 16 -> 7 etc etc.
                if (digit > 9)
                    digit -= 9;

                sum += digit;
            }

            if (sum % 10 != 0)
                throw new ArgumentException($"Invalid MSI CheckDigit '{value.Last()}'.");

            return new ProductCode(value, ProductCodeType.MSI);
        }

        public static ProductCode ParseHibc(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (value.Length < 2 || value.Length > 18 || value.Any(c => !char.IsLetterOrDigit(c)) || value.Where(c => char.IsLetter(c)).Any(c => !char.IsUpper(c)))
                throw new ArgumentException($"Invalid HIBC value '{value}'.");

            return new ProductCode(value, ProductCodeType.HIBC);
        }
    }
}
