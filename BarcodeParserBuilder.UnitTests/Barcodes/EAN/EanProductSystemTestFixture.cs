using BarcodeParserBuilder.Barcodes.EAN;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.EAN
{
    public class EanProductSystemTestFixture : BaseBarcodeTestFixture
    {
        [Theory]
        [InlineData(0, EanProductSystemScheme.ManufacturerAndProduct)]
        [InlineData(1, EanProductSystemScheme.ManufacturerAndProduct)]
        [InlineData(2, EanProductSystemScheme.Reserved)]
        [InlineData(3, EanProductSystemScheme.NationalDrugCode)]
        [InlineData(4, EanProductSystemScheme.ReservedCoupons)]
        [InlineData(5, EanProductSystemScheme.Coupons)]
        [InlineData(6, EanProductSystemScheme.ManufacturerAndProduct)]
        [InlineData(7, EanProductSystemScheme.ManufacturerAndProduct)]
        [InlineData(8, EanProductSystemScheme.ManufacturerAndProduct)]
        [InlineData(9, EanProductSystemScheme.ManufacturerAndProduct)]
        public void CanCreateProductSystemFromNumber(int number, EanProductSystemScheme expectedSchema)
        {

            //Act
            var result = EanProductSystem.Create(number);

            //Assert
            result.Should().NotBeNull();
            result.Value.Should().Be(number);
            result.Scheme.Should().Be(expectedSchema);
        }

        [Theory]
        [InlineData(0, EanProductSystemScheme.ManufacturerAndProduct)]
        [InlineData(2, EanProductSystemScheme.Reserved)]
        [InlineData(3, EanProductSystemScheme.NationalDrugCode)]
        [InlineData(4, EanProductSystemScheme.ReservedCoupons)]
        [InlineData(5, EanProductSystemScheme.Coupons)]
        public void CanCreateProductSystemBySchema(int expectedNumber, EanProductSystemScheme schema)
        {

            //Act
            var result = EanProductSystem.Create(schema);

            //Assert
            result.Should().NotBeNull();
            result.Value.Should().Be(expectedNumber);
            result.Scheme.Should().Be(schema);
        }
    }
}
