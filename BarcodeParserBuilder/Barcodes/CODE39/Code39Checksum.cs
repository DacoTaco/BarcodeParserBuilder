using BarcodeParserBuilder.Exceptions.CODE39;

namespace BarcodeParserBuilder.Barcodes.CODE39
{
    internal static class Code39Checksum
    {
        //calculated/written using the following tables : 
        //https://www.activebarcode.com/codes/checkdigit/modulo43
        //https://en.wikipedia.org/wiki/Code_39#Full_ASCII_Code_39
        private static readonly IDictionary<char, int> _encodedCharacters = new Dictionary<char, int>()
        {
            {'0', 0}, {'1', 1}, {'2', 2}, {'3', 3}, {'4', 4}, {'5', 5}, {'6', 6}, {'7', 7}, {'8', 8}, {'9', 9},
            {'A', 10}, {'B', 11}, {'C', 12}, {'D', 13}, {'E', 14}, {'F', 15}, {'G', 16}, {'H', 17}, {'I', 18}, {'J', 19}, {'K', 20}, {'L', 21}, {'M', 22}, {'N', 23}, {'O', 24}, {'P', 25}, {'Q', 26}, {'R', 27}, {'S', 28}, {'T', 29}, {'U', 30}, {'V', 31}, {'W', 32}, {'X', 33}, {'Y', 34}, {'Z', 35},
            {'-', 36}, {'.', 37}, {' ', 38}, {'$', 39}, {'/', 40}, {'+', 41}, {'%', 42},
        };

        internal static int GetEncodedWeight(char character)
        {
            if (character < 0 || character > 0x7F)
                throw new Code39ParseException($"Invalid Code39 character '{character}'");

            if (character == 0)
                return _encodedCharacters['%'] + _encodedCharacters['U']; //%U

            //all regular characters get the index (aka weight) of the character
            if (_encodedCharacters.TryGetValue(character, out var value))
                return value;

            //lower case letters are '+' + char.ToUpper
            var modifier = _encodedCharacters['+'];
            if (char.IsLetter(character))
                return modifier + _encodedCharacters[char.ToUpper(character)];

            if (character == 64)
                return _encodedCharacters['%'] + _encodedCharacters['V']; //%V

            if (character == 96)
                return _encodedCharacters['%'] + _encodedCharacters['W']; //%W

            if (character >= 1 && character <= 26)
                return _encodedCharacters['$'] + _encodedCharacters[(char)('A' + (character - 1))]; //$A - $Z

            if (character >= 27 && character <= 31)
                return _encodedCharacters['%'] + _encodedCharacters[(char)('A' + (character - 1))]; //%A - $E

            if (character >= 33 && character <= 58)
                return _encodedCharacters['/'] + _encodedCharacters[(char)('A' + (character - 1))]; // /A - /Z, with digits in between but those have been caught before this

            if (character >= 59 && character <= 63)
                return _encodedCharacters['%'] + _encodedCharacters[(char)('F' + (character - 1))]; //%F - $J

            if (character >= 91 && character <= 95)
                return _encodedCharacters['%'] + _encodedCharacters[(char)('K' + (character - 1))]; //%K - $O

            return _encodedCharacters['%'] + character switch
            {
                '{' => _encodedCharacters['P'],
                '|' => _encodedCharacters['Q'],
                '}' => _encodedCharacters['R'],
                '~' => _encodedCharacters['S'],
                '\x127' => (_encodedCharacters['%'] * 3) + _encodedCharacters['T'] + _encodedCharacters['X'] + _encodedCharacters['Y'] + _encodedCharacters['Z'],
                _ => throw new Code39ParseException($"Invalid Code39 character '{character}'")
            };
        }

        public static string ValidateAndStripChecksum(string input, Code39SymbologyIdentifier symbologyIdentifier)
        {
            if (string.IsNullOrEmpty(input) || input!.Length < 2)
                return input;

            var barcode = input;
            if (symbologyIdentifier.SymbologyIdentifier == Code39SymbologyIdentifier.NoFullASCIIMod43ChecksumTransmittedValue ||
               symbologyIdentifier.SymbologyIdentifier == Code39SymbologyIdentifier.FullASCIIMod43ChecksumTransmittedValue)
            {
                var checksum = input.Last();
                barcode = input[0..^1].ToString();
                var expectedCharacter = CalculateCheckCharacter(barcode);

                if (checksum != expectedCharacter)
                    throw new Code39ParseException($"Invalid Barcode Checksum '{expectedCharacter}', but read '{checksum}' instead");
            }

            return barcode;
        }

        public static char? GetBarcodeCheckCharacter(string input, AimSymbologyIdentifier symbologyIdentifier)
        {
            return symbologyIdentifier.SymbologyIdentifier != Code39SymbologyIdentifier.NoFullASCIIMod43ChecksumTransmittedValue &&
                   symbologyIdentifier.SymbologyIdentifier != Code39SymbologyIdentifier.FullASCIIMod43ChecksumTransmittedValue
                ? null
                : CalculateCheckCharacter(input);
        }

        //how to calculate the checksum, per character : 
        //take the character and look it up in the lookup table ( 00 - 42 )
        //if it is an extended character, add the value of + (41) to it
        //after this do a mod(10) or mod(43). the value you get is the character to add(use lookup table)

        //example : 'CoDE39' = 12 + 41 + 24 + 13 + 14 + 3 + 9 = 116
        //116 % 43 = 30 = U
        private static char CalculateCheckCharacter(string input)
        {
            const int mod = 43;
            var calculatedWeight = input.Sum(GetEncodedWeight) % mod;
            if (calculatedWeight > _encodedCharacters.Count)
                throw new Code39ParseException($"Unexpected calculated checksum weight: '{calculatedWeight}'");

            return _encodedCharacters.Single(kvp => kvp.Value == calculatedWeight).Key;
        }
    }
}
