using BarcodeParserBuilder.Exceptions.HIBC;

namespace BarcodeParserBuilder.Barcodes.HIBC
{
    internal class HibcDateParserBuilder : BaseFieldParserBuilder<BarcodeDateTime?>
    {
        protected override BarcodeDateTime? Parse(string? value) => throw new NotImplementedException("Impossible to Parse HIBC Date from string.");
        protected override string? Build(BarcodeDateTime? obj) => obj?.StringValue;

        protected override bool Validate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            if (value.Length < 4 || value.Length > 8 || !value.All(char.IsDigit))
                throw new HIBCValidateException($"Invalid HIBC Date value '{value}'.");

            return true;
        }

        protected override bool ValidateObject(BarcodeDateTime? obj)
        {
            if (obj == null)
                return true;

            var format = HibcBarcodeSegmentFormat.SegmentFormats.First(x => x.Value?.ToUpper() == obj.FormatString.ToUpper()).Value;

            if (!Validate(obj.StringValue) || obj.StringValue.Length != format.Length)
                throw new HIBCValidateException($"Invalid Barcode Value '{obj.StringValue}'.");

            return true;
        }
    }
}
