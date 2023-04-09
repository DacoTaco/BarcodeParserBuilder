using System;
using BarcodeParserBuilder.Barcodes.EAN;
using BarcodeParserBuilder.Exceptions.EAN;
using BarcodeParserBuilder.Infrastructure;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.EAN
{
    public class EanProductCodeParserBuilderTestFixture
    {
        private readonly EanProductCodeParserBuilder _parserBuilder;

        public EanProductCodeParserBuilderTestFixture()
        {
            _parserBuilder = new EanProductCodeParserBuilder();
        }

        [Theory]
        [InlineData("1118999881193")] //Ean13      
        [InlineData("45861734")] //Ean8
        public void FieldParserBuilderAcceptsValidProductCodes(string productCode)
        {
            //Arrange
            ProductCode result = null;

            //Act
            Action parseAction = () => result = (ProductCode)_parserBuilder.Parse(productCode, null, null);

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
            //Arrange & Act
            Action parseAction = () => _parserBuilder.Parse(productCode, null, null);

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
            var productCode = ProductCode.ParseGtin(code);

            //Act
            var output = _parserBuilder.Build(productCode);

            //Assert
            output.Should().Be(expectedOutput);
        }

        [Fact]
        public void FieldParserBuilderRejectsWrongProductCode()
        {
            //Arrange
            var productCode = ProductCode.ParseGtin("91197253403428");

            //Act
            Action buildAction = () => _parserBuilder.Build(productCode);

            //Assert
            buildAction.Should()
                .Throw<EanValidateException>()
                .WithMessage("Invalid ProductCode type 'GTIN'.");
        }
    }
}
