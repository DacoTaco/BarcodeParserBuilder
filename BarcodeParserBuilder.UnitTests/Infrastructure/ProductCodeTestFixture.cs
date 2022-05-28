using BarcodeParserBuilder.Infrastructure;
using FluentAssertions;
using System;
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
        [InlineData("91197253403427", "Invalid GTIN CheckDigit '7', Expected '8'.")] //GTIN - Invalid CheckDigit
        [InlineData("7038319110383", "Invalid GTIN CheckDigit '3', Expected '0'.")] //EAN13 - Invalid CheckDigit
        [InlineData("111899#881193", "Invalid GTIN value '111899#881193'.")] //GTIN - Invalid Character
        [InlineData("911972534034274895", "Invalid GTIN value '911972534034274895'.")] //GTIN - Value too long
        [InlineData("NONUMBERS", "Invalid GTIN value 'NONUMBERS'.")] //GTIN - Only Letters
        [InlineData("\0", "Invalid GTIN value '\0'.")] //GTIN - null character
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
        [InlineData("3831911038", ProductCodeType.EAN)] //GTIN (EAN13)
        [InlineData("1899988119", ProductCodeType.EAN)] //EAN13/GTIN-13/GLN
        [InlineData("1197253403", ProductCodeType.EAN)] //GTIN-12/EAN-12/UPC-12
        [InlineData("586173", ProductCodeType.EAN)] //GTIN-8/EAN-8
        public void CanParseEan(string value, ProductCodeType expectedSchema)
        {
            //Arrange & Act
            var result = ProductCode.ParseEan(value);

            //Assert
            result.Should().NotBeNull();
            result.Type.Should().Be(expectedSchema);
            result.Code.Should().Be(value);
        }

        [Theory]
        [InlineData("3831911038", ProductCodeType.NDC)]
        [InlineData("1899988119", ProductCodeType.NDC)]
        public void CanParseNdc(string value, ProductCodeType expectedSchema)
        {
            //Arrange & Act
            var result = ProductCode.ParseNdc(value);

            //Assert
            result.Should().NotBeNull();
            result.Type.Should().Be(expectedSchema);
            result.Code.Should().Be(value);
        }

        [Theory]
        [InlineData("11189#", "Invalid Ean Product Code '11189#'.")] //EAN - Invalid Character
        [InlineData("91197253403428", "Invalid Ean Product Code '91197253403428'.")] //EAN - Value too long
        [InlineData("NONUMBERS", "Invalid Ean Product Code 'NONUMBERS'.")] //EAN - Only Letters
        [InlineData("\0", "Invalid Ean Product Code '\0'.")] //EAN - null character
        public void CanDetectInvalidEan(string value, string expectedMessage)
        {
            //Arrange & Act
            ProductCode result = null;
            Action parseAction = () => result = ProductCode.ParseEan(value);

            //Assert
            parseAction.Should()
                .Throw<ArgumentException>()
                .WithMessage(expectedMessage);
            result.Should().BeNull();
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
