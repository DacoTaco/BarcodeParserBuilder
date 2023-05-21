using BarcodeParserBuilder.Barcodes.MSI;
using BarcodeParserBuilder.Exceptions.MSI;
using BarcodeParserBuilder.Infrastructure;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.MSI
{
    public class MsiProductCodeParserBuilderTestFixture
    {
        [Fact]
        public void FieldParserBuilderAcceptsValidProductCodes()
        {
            //Arrange
            var fieldParserBuilder = new MsiProductCodeParserBuilder();
            var productCode = "79927398713";
            ProductCode result = null;

            //Act
            Action parseAction = () => result = (ProductCode)fieldParserBuilder.Parse(productCode, null, null);

            //Assert
            parseAction.Should().NotThrow();
            result.Should().NotBeNull();
            result.Type.Should().Be(ProductCodeType.MSI);
            result.Code.Should().Be(productCode);
        }

        [Theory]
        [InlineData("91")] //Code Too Short    
        [InlineData("911972534034 #A")] //Invalid Character
        [InlineData("549HAHA418")] //Letters
        public void FieldParserBuilderRejectsInvalidProductCodes(string productCode)
        {
            //Arrange
            var fieldParserBuilder = new MsiProductCodeParserBuilder();

            //Act
            Action parseAction = () => fieldParserBuilder.Parse(productCode, null, null);

            //Assert
            parseAction.Should()
                .Throw<MsiValidateException>()
                .WithMessage($"Invalid MSI value '{productCode}'.");
        }

        [Theory]
        [InlineData("79927398713", "79927398713")] //"MSI code"
        [InlineData("   ", null)] //"spaces"
        [InlineData(null, null)] //TestName = "null"
        public void FieldParserBuilderBuildsCorrectString(string code, string expectedOutput)
        {
            //Arrange
            var fieldParserBuilder = new MsiProductCodeParserBuilder();
            var productCode = ProductCode.ParseMsi(code);

            //Act
            var output = fieldParserBuilder.Build(productCode);

            //Assert
            output.Should().Be(expectedOutput);
        }

        [Fact]
        public void FieldParserBuilderRejectsWrongProductCode()
        {
            //Arrange
            var fieldParserBuilder = new MsiProductCodeParserBuilder();
            var productCode = ProductCode.ParseGtin("91197253403428");

            //Act
            Action buildAction = () => fieldParserBuilder.Build(productCode);

            //Assert
            buildAction.Should()
                .Throw<MsiValidateException>()
                .WithMessage("Invalid ProductCode type 'GTIN'.");
        }
    }
}
