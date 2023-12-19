namespace BarcodeParserBuilder.Infrastructure.ProductCodes
{
    public class Code128ProductCode : ProductCode
    {
        public Code128ProductCode(string productCode) : base(productCode) { }

        public override ProductCodeType Type { get => ProductCodeType.CODE128; internal set { } }
    }
}
