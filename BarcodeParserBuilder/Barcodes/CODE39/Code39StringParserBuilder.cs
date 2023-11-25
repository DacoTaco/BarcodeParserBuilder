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

        protected internal static bool Validate(string? value, Code39ReaderModifier modifier)
        {
            switch (modifier.Value)
            {
                case Code39ReaderModifier.FullASCIIOlnyModChecksumValue:
                case Code39ReaderModifier.FullASCIINoChecksumValue:
                case Code39ReaderModifier.FullASCIIMod43ChecksumTransmittedValue:
                case Code39ReaderModifier.FullASCIIMod43ChecksumStrippedValue:
                    return ValidateFullASCII(value); 
                default:
                    return ValidateCode39String(value);
            }
        }

        protected internal static bool ValidateFullASCII(string? value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return System.Text.RegularExpressions.Regex.IsMatch(value!, "^[\x0-\x7F]+$");
        }

        protected internal static bool ValidateCode39String(string? value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return System.Text.RegularExpressions.Regex.IsMatch(value!, @"^[A-Z0-9\s\-\.\$\+\%\/]+$");
        }

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
