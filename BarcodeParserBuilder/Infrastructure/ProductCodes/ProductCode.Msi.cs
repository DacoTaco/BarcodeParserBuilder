namespace BarcodeParserBuilder.Infrastructure.ProductCodes
{
    public class MsiProductCode : ProductCode
    {
        internal MsiProductCode() : base("") { }
        public MsiProductCode(string code) : base(code)
        {
            ///MSI can have multiple ways to calculate check digits ... *sigh*
            ///Most common is luhn/Mod 10 - from right to left add all numbers together. numbers in even positions need to be multiplied by 2 and added together to make 1 digit. then modulo by 10
            ///See : https://en.wikipedia.org/wiki/Luhn_algorithm#Example_for_validating_check_digit
            ///Least common : no check digit. we don't support those, because wtf?
            ///Modulo 11 : https://en.wikipedia.org/wiki/MSI_Barcode#Mod_11_Check_Digit
            ///Modulo 1010 & 1110 : Combination of Mod 10/11 + mod 10
            ///Thankfully, we can apparently validate them all by doing a luhn/mod10 validation?

            if (code == null || code.Any(c => !char.IsDigit(c)) || code.Length < 3)
                throw new ArgumentException($"Invalid MSI value '{code}'.");

            var productCode = code
                .Reverse()
                .ToList();

            var sum = 0;
            for (var index = 0; index < productCode.Count; index++)
            {
                var character = productCode[index];
                var digit = index % 2 == 0 ? int.Parse(character.ToString()) : int.Parse(character.ToString()) * 2;

                //only 1 digit allowed. add numbers together, or subtract it with 9. 18 -> 9, 16 -> 7 etc etc.
                if (digit > 9)
                    digit -= 9;

                sum += digit;
            }

            if (sum % 10 != 0)
                throw new ArgumentException($"Invalid MSI CheckDigit '{code.Last()}'.");
        }

        public override ProductCodeType Type { get => ProductCodeType.MSI; internal set { } }
    }
}
