using BarcodeParserBuilder.Infrastructure;

namespace BarcodeParserBuilder.UnitTests
{
    public class TestProductCode : ProductCode
    {
        public TestProductCode(string value, ProductCodeType schema) : base(value, schema) { }
    }
}
