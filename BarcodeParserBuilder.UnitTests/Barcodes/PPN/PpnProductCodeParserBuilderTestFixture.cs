using BarcodeParserBuilder.Barcodes.PPN;
using BarcodeParserBuilder.Exceptions.PPN;
using BarcodeParserBuilder.Infrastructure.ProductCodes;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.PPN;

public class PpnProductCodeParserBuilderTestFixture
{
    [Fact]
    public void FieldParserBuilderAcceptsValidProductCodes()
    {
        //Arrange
        var productCode = "110375286414";
        var fieldParserBuilder = new PpnProductCodeParserBuilder();
        ProductCode? result = null;

        //Act
        Action parseAction = () => result = (ProductCode?)fieldParserBuilder.Parse(productCode, null, null);

        //Assert
        parseAction.Should().NotThrow();
        result.Should().NotBeNull();
        result!.Type.Should().Be(ProductCodeType.PPN);
        result.Code.Should().Be(productCode);
    }

    [Theory]
    [InlineData("111899#881193")] //"Invalid Character"     
    [InlineData("911972534034274895489245")] //PPN too long
    [InlineData("NONUMBERS")] //CheckDigit letters
    [InlineData("14Haha92140")] //small letters
    public void FieldParserBuilderRejectsInvalidProductCodes(string productCode)
    {
        //Arrange
        var fieldParserBuilder = new PpnProductCodeParserBuilder();

        //Act
        Action parseAction = () => fieldParserBuilder.Parse(productCode, null, null);

        //Assert
        parseAction.Should()
            .Throw<PPNValidateException>()
            .WithMessage($"Invalid PPN value '{productCode}'.");
    }

    [Theory]
    [InlineData("110375286414", "110375286414")] //PPN code
    [InlineData("   ", null)] //spaces
    [InlineData(null, null)] //null
    public void FieldParserBuilderBuildsCorrectString(string? code, string? expectedOutput)
    {
        //Arrange
        var fieldParserBuilder = new PpnProductCodeParserBuilder();
        var productCode = ProductCode.ParsePpn(code);

        //Act
        var output = fieldParserBuilder.Build(productCode);

        //Assert
        output.Should().Be(expectedOutput);
    }

    [Fact]
    public void FieldParserBuilderRejectsWrongProductCode()
    {
        //Arrange
        var fieldParserBuilder = new PpnProductCodeParserBuilder();
        var productCode = ProductCode.ParseGtin("91197253403428");

        //Act
        Action buildAction = () => fieldParserBuilder.Build(productCode);

        //Assert
        buildAction.Should()
            .Throw<PPNValidateException>()
            .WithMessage("Invalid ProductCode type 'GTIN'.");
    }
}
