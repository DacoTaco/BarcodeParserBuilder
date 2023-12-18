using BarcodeParserBuilder.Infrastructure.ProductCodes;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.EAN
{
    public class EanProductSystemTestFixture : BaseBarcodeTestFixture
    {
        [Theory]
        [InlineData(0, GtinProductScheme.ManufacturerAndProduct)]
        [InlineData(1, GtinProductScheme.ManufacturerAndProduct)]
        [InlineData(2, GtinProductScheme.Reserved)]
        [InlineData(3, GtinProductScheme.NationalDrugCode)]
        [InlineData(4, GtinProductScheme.ReservedCoupons)]
        [InlineData(5, GtinProductScheme.Coupons)]
        [InlineData(6, GtinProductScheme.ManufacturerAndProduct)]
        [InlineData(7, GtinProductScheme.ManufacturerAndProduct)]
        [InlineData(8, GtinProductScheme.ManufacturerAndProduct)]
        [InlineData(9, GtinProductScheme.ManufacturerAndProduct)]
        public void CanCreateProductSystemFromNumber(int number, GtinProductScheme expectedSchema)
        {
            //Act
            var result = GtinProductCode.EanProductSystems[number];

            //Assert
            result.Should().Be(expectedSchema);
        }
    }
}
