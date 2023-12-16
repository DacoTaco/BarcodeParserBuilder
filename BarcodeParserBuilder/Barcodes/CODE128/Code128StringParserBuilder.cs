namespace BarcodeParserBuilder.Barcodes.CODE128
{
    internal class Code128StringParserBuilder : BaseFieldParserBuilder<string?>
    {
        protected override string? Build(string? obj) => string.IsNullOrWhiteSpace(obj) ? null : obj;
        protected override string? Parse(string? value) => value;

        protected override bool Validate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            return true;
        }

        /// <summary>
        /// The validation currently does _not_ depend from the reader symbology identifier since all Code128 barcodes have same encoding
        /// It is very unlikely that such old standard will evolve but we keep the method signature
        /// </summary>
        /// <param name="value">reading</param>
        /// <param name="information">AimSymbologyIdentifier for the Code39</param>
        /// <returns></returns>
        protected internal static bool Validate(string? value, Code128SymbologyIdentifier information)
        {
            return ValidateFullASCII(value);
        }

        /// <summary>
        /// Code128 contains full ASCII set of symbols
        /// </summary>
        /// <param name="value">reading</param>
        /// <returns>whether the reading is subset of full ASCII</returns>
        /// <exception cref="ArgumentNullException"></exception>
        protected internal static bool ValidateFullASCII(string? value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return System.Text.RegularExpressions.Regex.IsMatch(value!, "^[\x0-\xFF]+$");
        }

        /// <summary>
        /// There is no strict rules, how long the Code128 reading can be. But most readers are not able to read more than 55 symbols
        /// </summary>
        /// <param name="value">read string</param>
        /// <returns>whether the reading is within the limit</returns>
        /// <exception cref="ArgumentNullException"></exception>
        protected internal static bool ValidateCode128ContentLength(string? value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return value.Length > 1 && value.Length < 56;
        }
    }
}
