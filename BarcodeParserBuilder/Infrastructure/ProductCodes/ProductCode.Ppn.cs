using System.Text.RegularExpressions;

namespace BarcodeParserBuilder.Infrastructure.ProductCodes
{
    public class PpnProductCode : ProductCode
    {
        internal PpnProductCode() : base("") { }

        //Specs : https://www.ifaffm.de/mandanten/1/documents/04_ifa_coding_system/IFA-Info_Check_Digit_Calculations_PZN_PPN_UDI_EN.pdf
        public PpnProductCode(string code) : base(code)
        {
            var regex = new Regex(@"^[A-Z0-9]*\d{2,2}$");
            if (code == null || !regex.IsMatch(code) || code.Length < 4 || code.Length > 22)
                throw new ArgumentException($"Invalid PPN value '{code}'.");

            //per character we get the int value, multiply it with 2+character index and then modulo 97.
            //the output should match the last 2 digits in the product code
            var checkDigit = int.Parse(code.Substring(code.Length - 2, 2));
            var productCode = code.Remove(code.Length - 2);

            var calculatedCheckDigit = productCode.Select((character, index) => character * (index + 2)).Sum();
            calculatedCheckDigit %= 97;

            if (calculatedCheckDigit != checkDigit)
                throw new ArgumentException($"Invalid PPN CheckDigit '{checkDigit:00}', Expected '{calculatedCheckDigit:00}'.");
        }

        public override ProductCodeType Type { get => ProductCodeType.PPN; internal set { } }
    }
}
