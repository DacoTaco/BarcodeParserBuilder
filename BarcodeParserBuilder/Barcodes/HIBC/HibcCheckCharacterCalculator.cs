using BarcodeParserBuilder.Exceptions.HIBC;
using System.Linq;
using System.Text.RegularExpressions;

namespace BarcodeParserBuilder.Barcodes.HIBC
{
    internal static class HibcCheckCharacterCalculator
    {
        public static string AllowedCharacterRegex = @"^[A-Z0-9-. $/+%]*$";

        public static bool ValidateSegment(string value, char? linkCharacter = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            if ( linkCharacter.HasValue && !Regex.IsMatch(linkCharacter.ToString(), AllowedCharacterRegex))
                throw new HIBCValidateException("Invalid input to validate check character on.");

            var input = value.Substring(0, value.Length - 1);
            var checkCharacter = CalculateSegmentCheckCharacter(input);

            if (checkCharacter != value.Last())
                throw new HIBCValidateException($"Check Character did not match: expected '{checkCharacter}' but got '{value.Last()}'.");

            if(linkCharacter.HasValue && input.Last() != linkCharacter)
                throw new HIBCValidateException($"Link Character did not match: expected '{linkCharacter}' but got '{input.Last()}'.");

            return true;
        }

        public static char CalculateSegmentCheckCharacter(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new HIBCValidateException("Can not calculate check character for an empty string.");

            if ( value.Length < 5 || !Regex.IsMatch(value, AllowedCharacterRegex))
                throw new HIBCValidateException("Invalid input to validate check character on.");

            var sum = value.Select(c => GetWeight(c)).Sum();
            return GetCharacter(sum % 43);
        }

        private static int GetWeight(char character)
        {
            if (char.IsDigit(character))
                return int.Parse(character.ToString());

            if (char.IsLetter(character))
                return character - 55;

            switch (character)
            {
                case '-':
                    return 36;
                case '.':
                    return 37;
                case ' ':
                    return 38;
                case '$':
                    return 39;
                case '/':
                    return 40;
                case '+':
                    return 41;
                case '%':
                    return 42;
            }

            throw new HIBCValidateException($"Invalid character '{character}'.");
        }

        private static char GetCharacter(int weight)
        {
            if (weight < 10)
                return weight.ToString().First();

            if (weight < 36)
                return (char)(55 + weight);

            switch (weight)
            {
                case 36:
                    return '-';
                case 37:
                    return '.';
                case 38:
                    return ' ';
                case 39:
                    return '$';
                case 40:
                    return '/';
                case 41:
                    return '+';
                case 42:
                    return '%';
            }

            throw new HIBCValidateException($"Invalid character index '{weight}'.");
        }
    }
}
