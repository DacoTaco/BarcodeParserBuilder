using System.Linq;

namespace BarcodeParserBuilder.Barcodes.EAN
{
    internal static class EanCheckDigitCalculator
    {
        public static int CalculateCheckDigit(string value)
        {
            var calculatedCheckDigit = value.PadLeft(13, '0').Select((character, index) => index % 2 == 0 ? (int)char.GetNumericValue(character) * 3 : (int)char.GetNumericValue(character)).Sum();
            calculatedCheckDigit %= 10;
            if (calculatedCheckDigit > 0)
                calculatedCheckDigit = 10 - calculatedCheckDigit;

            return calculatedCheckDigit;
        }
    }
}
