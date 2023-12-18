using BarcodeParserBuilder.Infrastructure.ProductCodes;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Infrastructure
{
    public class ProductCodeTestFixture
    {
        [Theory]
        [InlineData("91197253403428", ProductCodeType.GTIN)] //GTIN
        [InlineData("07038319110380", ProductCodeType.GTIN)] //GTIN (EAN13)
        [InlineData("1118999881193", ProductCodeType.EAN)] //EAN13/GTIN-13/GLN
        public void CanParseGtin(string value, ProductCodeType expectedSchema)
        {
            //Arrange & Act
            var result = ProductCode.ParseGtin(value);

            //Assert
            result.Should().NotBeNull();
            result.Type.Should().Be(expectedSchema);
            result.Code.Should().Be(value);
        }

        [Theory]
        [InlineData("91197253403427", "Invalid GTIN/EAN CheckDigit '7', Expected '8'.")] //GTIN - Invalid CheckDigit
        [InlineData("7038319110383", "Invalid GTIN/EAN CheckDigit '3', Expected '0'.")] //EAN13 - Invalid CheckDigit
        [InlineData("111899#881193", "Invalid GTIN/EAN value '111899#881193'.")] //GTIN - Invalid Character
        [InlineData("911972534034274895", "Invalid GTIN/EAN Length of 18.")] //GTIN - Value too long
        [InlineData("NONUMBERS", "Invalid GTIN/EAN value 'NONUMBERS'.")] //GTIN - Only Letters
        [InlineData("\0", "Invalid GTIN/EAN value '\0'.")] //GTIN - null character
        public void CanDetectInvalidGtin(string value, string expectedMessage)
        {
            //Arrange & Act
            ProductCode result = null;
            Action parseAction = () => result = ProductCode.ParseGtin(value);

            //Assert
            parseAction.Should()
                .Throw<ArgumentException>()
                .WithMessage(expectedMessage);
            result.Should().BeNull();
        }

        [Theory]
        [InlineData("0367457153032", ProductCodeType.EAN, "6745715303", GtinProductScheme.NationalDrugCode)] //GTIN (EAN13), also NDC
        [InlineData("9781593070564", ProductCodeType.EAN, "978159307056", GtinProductScheme.Unknown)] //EAN13/GTIN-13/GLN
        [InlineData("045496428679", ProductCodeType.EAN, "42867", GtinProductScheme.ManufacturerAndProduct)] //GTIN-12/EAN-12/UPC-12/UPC-A
        [InlineData("02345673", ProductCodeType.EAN, "0234567", GtinProductScheme.Unknown)] //GTIN-8/EAN-8
        [InlineData("2345673", ProductCodeType.EAN, "234567", GtinProductScheme.Unknown)] //UPC-E
        public void CanParseEan(string barcode, ProductCodeType expectedType, string expectedValue, GtinProductScheme expectedSchema)
        {
            //Arrange & Act
            var result = ProductCode.ParseGtin(barcode);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<GtinProductCode>();

            var productCode = (GtinProductCode)result;
            productCode.Type.Should().Be(expectedType);
            productCode.Code.Should().Be(barcode);
            productCode.Schema.Should().Be(expectedSchema);
            productCode.Value.Should().Be(expectedValue);
        }

        [Theory]
        [InlineData("110375286414")] //PPN - PZN
        [InlineData("14144692180")] //PPN - APB
        [InlineData("14HAHA92146")] //PPN - Letters
        public void CanParsePpn(string value)
        {
            //Arrange & Act
            var result = ProductCode.ParsePpn(value);

            //Assert
            result.Should().NotBeNull();
            result.Type.Should().Be(ProductCodeType.PPN);
            result.Code.Should().Be(value);
        }

        [Theory]
        [InlineData("91197253403427", "Invalid PPN CheckDigit '27', Expected '82'.")] //PPN - Invalid CheckDigit
        [InlineData("111899#881193", "Invalid PPN value '111899#881193'.")] //PPN - Invalid Character
        [InlineData("911972534034274895489245", "Invalid PPN value '911972534034274895489245'.")] //PPN - too long
        [InlineData("NONUMBERS", "Invalid PPN value 'NONUMBERS'.")] //PPN - CheckDigit letters
        [InlineData("14Haha92140", "Invalid PPN value '14Haha92140'.")] //PPN - small letters
        [InlineData("\0", "Invalid PPN value '\0'.")] //PPN - null character
        public void CanDetectInvalidPPN(string value, string expectedMessage)
        {
            //Arrange & Act
            ProductCode result = null;
            Action parseAction = () => result = ProductCode.ParsePpn(value);

            //Assert
            parseAction.Should()
                .Throw<ArgumentException>()
                .WithMessage(expectedMessage);
            result.Should().BeNull();
        }

        [Theory]
        [InlineData("79927398713")] //MSI - Luhn/Mod10
        [InlineData("1446921")] //MSI - APB
        [InlineData("14469217")] //MSI - APB + Checkdigit
        [InlineData("12345674")] //MSI - Mod 11
        public void CanParseMSI(string value)
        {
            //Arrange & Act
            var result = ProductCode.ParseMsi(value);

            //Assert
            result.Should().NotBeNull();
            result.Type.Should().Be(ProductCodeType.MSI);
            result.Code.Should().Be(value);
        }

        [Theory]
        [InlineData("79927398712", "Invalid MSI CheckDigit '2'.")] //MSI - Invalid Checkdigit
        [InlineData("79", "Invalid MSI value '79'.")] //MSI - Length
        [InlineData("\0", "Invalid MSI value '\0'.")] //MSI - null character
        public void CanDetectInvalidMSI(string value, string expectedMessage)
        {
            //Arrange & Act
            ProductCode result = null;
            Action parseAction = () => result = ProductCode.ParseMsi(value);

            //Assert
            parseAction.Should()
                .Throw<ArgumentException>()
                .WithMessage(expectedMessage);
            result.Should().BeNull();
        }

        [Theory]
        [InlineData("AGBDOSIZ45973456O8")] //HIBC - Max Size
        [InlineData("14F")] //MSI - Minimum Size
        public void CanParseHIBC(string value)
        {
            //Arrange & Act
            var result = ProductCode.ParseHibc(value);

            //Assert
            result.Should().NotBeNull();
            result.Type.Should().Be(ProductCodeType.HIBC);
            result.Code.Should().Be(value);
        }

        [Theory]
        [InlineData("799273#98712", "Invalid HIBC value '799273#98712'.")] //HIBC - Random Character
        [InlineData("7", "Invalid HIBC value '7'.")] //HIBC - Min Length
        [InlineData("1234567890123456789", "Invalid HIBC value '1234567890123456789'.")] //HIBC - Max Length
        [InlineData("DSGHD\095489", "Invalid HIBC value 'DSGHD\095489'.")] //HIBC - Invalid Character
        public void CanDetectInvalidHIBC(string value, string expectedMessage)
        {
            //Arrange & Act
            ProductCode result = null;
            Action parseAction = () => result = ProductCode.ParseHibc(value);

            //Assert
            parseAction.Should()
                .Throw<ArgumentException>()
                .WithMessage(expectedMessage);
            result.Should().BeNull();
        }
    }
}
