namespace BarcodeParserBuilder.Infrastructure.ProductCodes
{
    public class Code39ProductCode : ProductCode
    {
        public Code39ProductCode(string productCode) : base(productCode)
        {
            if (productCode.Any(c => c > 0xFF))
                throw new ArgumentException($"Invalid Code39 value in '{productCode}'.");

            // There is no strict rules, how long the Code128 reading can be. But most readers are not able to read more than 55 symbols
            if (productCode.Length == 0 || productCode.Length >= 55)
                throw new ArgumentException("Invalid Code39 length.");
        }

        public override ProductCodeType Type { get => ProductCodeType.CODE39; internal set { } }
    }
}
