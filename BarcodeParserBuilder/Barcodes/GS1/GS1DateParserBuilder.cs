using BarcodeParserBuilder.Exceptions.GS1;

namespace BarcodeParserBuilder.Barcodes.GS1
{
    internal class GS1DateParserBuilder : BaseFieldParserBuilder<BarcodeDateTime?>
    {
        protected override BarcodeDateTime? Parse(string? value) => BarcodeDateTime.Gs1Date(value);
        protected override string? Build(BarcodeDateTime? obj) => string.IsNullOrWhiteSpace(obj?.StringValue) ? null : obj!.StringValue;

        protected override bool Validate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            if (value!.Length != 6 || !value.All(char.IsDigit))
                throw new GS1ValidateException($"Invalid GS1 Date value '{value}'.");

            return true;
        }

        protected override bool ValidateObject(BarcodeDateTime? obj)
        {
            if (obj == null)
                return true;

            if (!Validate(obj.StringValue) || obj.FormatString != BarcodeDateTime.GS1Format)
                throw new GS1ValidateException($"Invalid Barcode Value '{obj.StringValue}'.");

            return true;
        }
    }
}
