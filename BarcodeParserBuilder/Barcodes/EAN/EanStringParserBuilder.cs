namespace BarcodeParserBuilder.Barcodes.EAN;

internal class EanStringParserBuilder : BaseFieldParserBuilder<string?>
{
    protected override string? Build(string? obj) => string.IsNullOrWhiteSpace(obj) ? null : obj;
    protected override string? Parse(string? value) => value;

    protected override bool Validate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return true;

        //in ean we only allow digits
        return value.All(char.IsDigit);
    }
}
