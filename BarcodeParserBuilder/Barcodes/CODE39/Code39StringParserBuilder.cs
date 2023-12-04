using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using BarcodeParserBuilder.Infrastructure;

namespace BarcodeParserBuilder.Barcodes.CODE39
{
    internal class Code39StringParserBuilder : BaseFieldParserBuilder<string?>
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
        /// The validation depends from the reader symbology identifier and describes how the reading should be interpreted
        /// </summary>
        /// <param name="value">reading</param>
        /// <param name="information">AimSymbologyIdentifier for the Code39</param>
        /// <returns></returns>
        protected internal static bool Validate(string? value, Code39SymbologyIdentifier information)
        {
            switch (information.SymbologyIdentifier)
            {
                case Code39SymbologyIdentifier.FullASCIIOlnyModChecksumValue:
                case Code39SymbologyIdentifier.FullASCIINoChecksumValue:
                case Code39SymbologyIdentifier.FullASCIIMod43ChecksumTransmittedValue:
                case Code39SymbologyIdentifier.FullASCIIMod43ChecksumStrippedValue:
                    return ValidateFullASCII(value); 
                default:
                    return ValidateCode39String(value);
            }
        }

        /// <summary>
        /// Code39 can contain full ASCII set of symbols
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

            return System.Text.RegularExpressions.Regex.IsMatch(value!, "^[\x0-\x7F]+$");
        }

        /// <summary>
        /// Regular Code39 can contain limited number of ASCII symbols A-Z, 0-9 and characters -,.,$,+,%,/
        /// </summary>
        /// <param name="value">reading</param>
        /// <returns>whether reading is valid set of Code39 default charset</returns>
        /// <exception cref="ArgumentNullException"></exception>
        protected internal static bool ValidateCode39String(string? value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return System.Text.RegularExpressions.Regex.IsMatch(value!, @"^[A-Z0-9\s\-\.\$\+\%\/]+$");
        }

        /// <summary>
        /// There is no strict rules, how long the Code39 reading can be. But most readers are not able to read more than 55 symbols
        /// </summary>
        /// <param name="value">read string</param>
        /// <returns>whether the reading is within the limit</returns>
        /// <exception cref="ArgumentNullException"></exception>
        protected internal static bool ValidateCode39ContentLength(string? value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return value.Length > 1 && value.Length < 56;
        }
    }
    
}
