using System.Globalization;
using BarcodeParserBuilder.Exceptions.GS1;

namespace BarcodeParserBuilder.Barcodes.GS1
{
    internal class GS1DoubleParserBuilder : BaseFieldParserBuilder<double?>
    {
        protected override string? Build(double? obj)
        {
            if (!obj.HasValue)
                return null;

            var objString = obj.Value.ToString(CultureInfo.InvariantCulture);
            var location = objString.IndexOf('.');
            objString = objString.Replace(".", "");
            if (location < 0 || location > 9)
                location = 0;
            else
                location = objString.Length - location;

            return $"{location}{objString.TrimStart('0').PadLeft(6, '0')}";
        }

        protected override double? Parse(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var location = int.Parse(value.First().ToString());
            value = value[1..];

            return double.Parse(value) * Math.Pow(10, -location);
        }

        protected override bool ValidateObjectLength(double? obj, int? minimumLength, int? maximumLength)
        {
            if (!obj.HasValue)
                return true;

            var valueString = obj.Value
                .ToString(CultureInfo.InvariantCulture)
                .Replace(".", "")
                .TrimStart('0');

            return valueString.Length <= (maximumLength ?? int.MaxValue) && valueString.Length >= (minimumLength ?? 0);
        }

        protected override bool Validate(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return true;

            if (!value.All(char.IsDigit))
                throw new GS1ValidateException($"Invalid GS1 double value '{value}'.");

            return true;
        }
    }
}
