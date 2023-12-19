namespace BarcodeParserBuilder.Infrastructure.ProductCodes
{
    internal class Code39Productcode : ProductCode
    {
        public Code39Productcode(string productCode) : base(productCode) { }

        public override ProductCodeType Type { get => ProductCodeType.CODE39; internal set { } }
    }
}
