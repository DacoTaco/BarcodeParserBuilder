using BarcodeParserBuilder.EAN;
using BarcodeParserBuilder.Exceptions.EAN;
using BarcodeParserBuilder.Infrastructure;
using FluentAssertions;
using System;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.EAN
{
    public class EanProductCodeParserBuilderTestFixture
    {
        [Theory]
        [InlineData("1118999881193")] //Ean13      
        [InlineData("45861734")] //Ean8
        public void FieldParserBuilderAcceptsValidProductCodes(string productCode)
        {
            //Arrange
            var fieldParserBuilder = new EanProductCodeParserBuilder();
            ProductCode result = null;

            //Act
            Action parseAction = () => result = (ProductCode)fieldParserBuilder.Parse(productCode, null);

            //Assert
            parseAction.Should().NotThrow();
            result.Should().NotBeNull();
            result.Type.Should().Be(ProductCodeType.EAN);
            result.Code.Should().Be(productCode);
        }

        [Theory]
        [InlineData("911972534034286")] //Code Too long
        [InlineData("911972534034 #A")] //Invalid Character
        [InlineData("91197253403428")] //GTIN
        [InlineData("07038319110380")] //GTIN (EAN13)
        public void FieldParserBuilderRejectsInvalidProductCodes(string productCode)
        {
            //Arrange
            var fieldParserBuilder = new EanProductCodeParserBuilder();

            //Act
            Action parseAction = () => fieldParserBuilder.Parse(productCode, null);

            //Assert
            parseAction.Should()
                .Throw<EanValidateException>()
                .WithMessage($"Invalid Ean value '{productCode}'.");
        }

        [Theory]
        [InlineData("1118999881193", "1118999881193")] //Ean code
        [InlineData("   ", null)] //spaces
        [InlineData(null, null)] //Null
        public void FieldParserBuilderBuildsCorrectString(string code, string expectedOutput)
        {
            //Arrange
            var fieldParserBuilder = new EanProductCodeParserBuilder();
            var productCode = ProductCode.ParseGtin(code);

            //Act
            var output = fieldParserBuilder.Build(productCode);

            //Assert
            output.Should().Be(expectedOutput);
        }

        [Fact]
        public void FieldParserBuilderRejectsWrongProductCode()
        {
            //Arrange
            var fieldParserBuilder = new EanProductCodeParserBuilder();
            var productCode = ProductCode.ParseGtin("91197253403428");

            //Act
            Action buildAction = () => fieldParserBuilder.Build(productCode);

            //Assert
            buildAction.Should()
                .Throw<EanValidateException>()
                .WithMessage("Invalid ProductCode type 'GTIN'.");
        }
    }
}
