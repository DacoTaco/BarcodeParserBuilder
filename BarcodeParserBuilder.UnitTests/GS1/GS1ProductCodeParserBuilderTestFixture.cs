using BarcodeParserBuilder.Exceptions.GS1;
using BarcodeParserBuilder.GS1;
using BarcodeParserBuilder.Infrastructure;
using FluentAssertions;
using System;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.GS1
{
    public class GS1ProductCodeParserBuilderTestFixture
    {
        [Theory]
        [InlineData("91197253403428")] //GTIN
        [InlineData("07038319110380")] //GTIN (EAN13)
        public void FieldParserBuilderAcceptsValidProductCodes(string productCode)
        {
            //Arrange
            var fieldParserBuilder = new GS1ProductCodeParserBuilder();
            ProductCode result = null;

            //Act
            Action parseAction = () => result = (ProductCode)fieldParserBuilder.Parse(productCode, null);

            //Assert
            parseAction.Should().NotThrow();
            result.Should().NotBeNull();
            result.Type.Should().Be(ProductCodeType.GTIN);
            result.Code.Should().Be(productCode);
        }

        [Theory]
        [InlineData("911972534034286")] //Code Too long
        [InlineData("911972534034 #A")] //Invalid Character
        [InlineData("1118999881193")] //EAN13
        [InlineData("45861734")] //EAN8
        public void FieldParserBuilderRejectsInvalidProductCodes(string productCode)
        {
            //Arrange
            var fieldParserBuilder = new GS1ProductCodeParserBuilder();

            //Act
            Action parseAction = () => fieldParserBuilder.Parse(productCode, null);

            //Assert
            parseAction.Should()
                .Throw<GS1ValidateException>()
                .WithMessage($"Invalid GTIN value '{productCode}'.");
        }

        [Theory]
        [InlineData("91197253403428", "91197253403428")] //GTIN code
        [InlineData("   ", null)] //spaces
        [InlineData(null, null)] //null
        public void FieldParserBuilderBuildsCorrectString(string code, string expectedOutput)
        {
            //Arrange
            var fieldParserBuilder = new GS1ProductCodeParserBuilder();
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
            var fieldParserBuilder = new GS1ProductCodeParserBuilder();
            var productCode = ProductCode.ParseGtin("1118999881193");

            //Act
            Action buildAction = () => fieldParserBuilder.Build(productCode);

            //Assert
            buildAction.Should()
                .Throw<GS1ValidateException>()
                .WithMessage("Invalid ProductCode type 'EAN'.");
        }
    }
}
