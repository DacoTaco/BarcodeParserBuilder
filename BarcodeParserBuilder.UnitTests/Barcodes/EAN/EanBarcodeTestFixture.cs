using BarcodeParserBuilder.Barcodes.EAN;
using BarcodeParserBuilder.Infrastructure;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.EAN
{
    public class EanBarcodeTestFixture
    {
        public static TheoryData<ProductCode, EanProductSystem> EanBarcodeTestData =>
            new TheoryData<ProductCode, EanProductSystem>
            {
                { ProductCode.ParseEan("5413505300127"), EanProductSystem.Create(2) },
                { ProductCode.ParseNdc("4135053001"), EanProductSystem.Create(3) }
            };

        [Theory]
        [MemberData(nameof(EanBarcodeTestData))]
        public void SetsNumberSystemWhenSettingProductCode(ProductCode productCode, EanProductSystem expectedSchema)
        {
            //Arrange & Act
            var barcode = new EanBarcode
            {
                ProductCode = productCode
            };

            //Assert
            barcode.ProductSystem.Should().NotBeNull();
            barcode.ProductSystem.Scheme.Should().Be(expectedSchema.Scheme);
            barcode.ProductSystem.Value.Should().Be(expectedSchema.Value);
        }
    }
}
