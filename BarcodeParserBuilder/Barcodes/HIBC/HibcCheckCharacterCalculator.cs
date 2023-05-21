using System.Text.RegularExpressions;
using BarcodeParserBuilder.Exceptions.HIBC;

namespace BarcodeParserBuilder.Barcodes.HIBC
{
    internal static class HibcCheckCharacterCalculator
    {
        public static string AllowedCharacterRegex = @"^[A-Z0-9-. $/+%]*$";

        public static bool ValidateSegment(string? value, char? linkCharacter = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            if (linkCharacter.HasValue && !Regex.IsMatch(linkCharacter?.ToString() ?? "", AllowedCharacterRegex))
                throw new HIBCValidateException("Invalid input to validate check character on.");

            var input = value.Substring(0, value.Length - 1);
            var checkCharacter = CalculateSegmentCheckCharacter(input);

            if (checkCharacter != value.Last())
                throw new HIBCValidateException($"Check Character did not match: expected '{checkCharacter}' but got '{value.Last()}'.");

            if (linkCharacter.HasValue && input.Last() != linkCharacter)
                throw new HIBCValidateException($"Link Character did not match: expected '{linkCharacter}' but got '{input.Last()}'.");

            return true;
        }

        public static char CalculateSegmentCheckCharacter(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new HIBCValidateException("Can not calculate check character for an empty string.");

            if (value.Length < 5 || !Regex.IsMatch(value, AllowedCharacterRegex))
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

            return character switch
            {
                '-' => 36,
                '.' => 37,
                ' ' => 38,
                '$' => 39,
                '/' => 40,
                '+' => 41,
                '%' => 42,
                _ => throw new HIBCValidateException($"Invalid character '{character}'."),
            };
        }

        private static char GetCharacter(int weight)
        {
            if (weight < 10)
                return weight.ToString().First();

            if (weight < 36)
                return (char)(55 + weight);

            return weight switch
            {
                36 => '-',
                37 => '.',
                38 => ' ',
                39 => '$',
                40 => '/',
                41 => '+',
                42 => '%',
                _ => throw new HIBCValidateException($"Invalid character index '{weight}'."),
            };
        }
    }
}
