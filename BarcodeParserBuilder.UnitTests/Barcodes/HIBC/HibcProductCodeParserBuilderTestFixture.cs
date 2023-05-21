using BarcodeParserBuilder.Barcodes.HIBC;
using BarcodeParserBuilder.Exceptions.HIBC;
using BarcodeParserBuilder.Infrastructure;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.HIBC
{
    public class HibcProductCodeParserBuilderTestFixture
    {
        [Fact]
        public void FieldParserBuilderAcceptsValidProductCodes()
        {
            //Arrange
            var fieldParserBuilder = new HibcProductCodeParserBuilder();
            var productCode = "AZBCD79927398713";
            ProductCode result = null;

            //Act
            Action parseAction = () => result = (ProductCode)fieldParserBuilder.Parse(productCode, null, null);

            //Assert
            parseAction.Should().NotThrow();
            result.Should().NotBeNull();
            result.Type.Should().Be(ProductCodeType.HIBC);
            result.Code.Should().Be(productCode);
        }

        [Theory]
        [InlineData("9")] //Code Too Short
        [InlineData("911972534034 #A")] //Invalid Character
        [InlineData("1234567890ABCDEFGHI")] //Too Long
        [InlineData("1234567890AbCD")] //lowercase letter
        public void FieldParserBuilderRejectsInvalidProductCodes(string productCode)
        {
            //Arrange
            var fieldParserBuilder = new HibcProductCodeParserBuilder();

            //Act
            Action parseAction = () => fieldParserBuilder.Parse(productCode, null, null);

            //Assert
            parseAction.Should()
                .Throw<HIBCValidateException>()
                .WithMessage($"Invalid HIBC value '{productCode}'.");
        }

        [Theory]
        [InlineData("AQ3451", "AQ3451")] //HIBC code
        [InlineData("   ", null)] //spaces
        [InlineData(null, null)] //null
        public void FieldParserBuilderBuildsCorrectString(string code, string expectedOutput)
        {
            //Arrange
            var fieldParserBuilder = new HibcProductCodeParserBuilder();
            var productCode = ProductCode.ParseHibc(code);

            //Act
            var output = fieldParserBuilder.Build(productCode);

            //Assert
            output.Should().Be(expectedOutput);
        }

        [Fact]
        public void FieldParserBuilderRejectsWrongProductCode()
        {
            //Arrange
            var fieldParserBuilder = new HibcProductCodeParserBuilder();
            var productCode = ProductCode.ParseGtin("91197253403428");

            //Act
            Action buildAction = () => fieldParserBuilder.Build(productCode);

            //Assert
            buildAction.Should()
                .Throw<HIBCValidateException>()
                .WithMessage("Invalid ProductCode type 'GTIN'.");
        }
    }
}
