namespace BarcodeParserBuilder.Infrastructure
{
    public enum ProductCodeType
    {
        Unknown = 0,
        GTIN,
        EAN,
        NDC,
        PPN,
        MSI,
        HIBC
    }

    public partial class ProductCode
    {
        internal ProductCode(string value, ProductCodeType schema)
        {
            Code = value;
            Type = schema;
        }

        public string Code { get; protected set; }
        public ProductCodeType Type { get; protected set; }
    }
}
