using BarcodeParserBuilder.Barcodes.EAN;
using BarcodeParserBuilder.Exceptions.EAN;
using BarcodeParserBuilder.Infrastructure;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.EAN
{
    public class EanBarcodeParserBuilderTestFixture : BaseBarcodeTestFixture
    {
        [Theory]
        [MemberData(nameof(ValidEanBarcodes))]
        public void CanParseBarcodeString(string barcode, EanBarcode expectedBarcode)
        {
            //Arrange & Act
            EanBarcodeParserBuilder.TryParse(barcode, out var result).Should().BeTrue($"'{barcode}' should be parsable");
            Action parseAction = () => EanBarcodeParserBuilder.Parse(barcode);

            //Assert
            parseAction.Should().NotThrow($"'{barcode}' should be parsable");
            result.Should().NotBeNull();
            CompareBarcodeObjects(expectedBarcode, result);
        }

        [Theory]
        [MemberData(nameof(ValidEanBarcodes))]
        public void CanBuildBarcodeString(string expectedString, EanBarcode barcode)
        {
            //Arrange & Act
            string result = null;
            Action buildAction = () => result = EanBarcodeParserBuilder.Build(barcode);

            //Assert
            buildAction.Should().NotThrow($"'{barcode}' should be buildable");
            result.Should().NotBeNull();
            result.Should().Be(expectedString);
        }

        public static IEnumerable<object[]> ValidEanBarcodes()
        {
            //EAN13
            yield return new object[] 
            { 
                $"5420046520228",
                new EanBarcode()
                {
                    ProductCode = new TestProductCode("5420046520228", ProductCodeType.EAN),
                } 
            };

            //UPC
            yield return new object[]
            {
                $"045496730086",
                new EanBarcode()
                {
                    ProductCode = new TestProductCode("045496730086", ProductCodeType.EAN),
                }
            };

            //EAN8
            yield return new object[]
            {
                $"27066027",
                new EanBarcode()
                {
                    ProductCode = new TestProductCode("27066027", ProductCodeType.EAN),
                }
            };

            //ISBN - Witcher
            yield return new object[]
            {
                $"9780316029186",
                new EanBarcode()
                {
                    ProductCode = new TestProductCode("9780316029186", ProductCodeType.EAN),
                }
            };
        }

        [Theory]
        [MemberData(nameof(InValidEanBarcodes))]
        public void InvalidBarcodeStringThrowsException(string barcode, string expectedMessage)
        {
            //Arrange & Act
            EanBarcodeParserBuilder.TryParse(barcode, out var result).Should().BeFalse($"'{barcode}' should not be parsable");
            Action parseAction = () => EanBarcodeParserBuilder.Parse(barcode);

            //Assert
            parseAction.Should()
                .Throw<EanParseException>()
                .WithMessage(expectedMessage);
        }

        public static IEnumerable<object[]> InValidEanBarcodes()
        {
            //ProductCode Too Short
            yield return new object[]
            {
                $"9119725",
                $"Failed to parse Ean Barcode :{Environment.NewLine}Invalid Ean value '9119725'."
            };

            //ProductCode Too long
            yield return new object[]
            {
                $"91197254896418",
                $"Failed to parse Ean Barcode :{Environment.NewLine}Invalid value Length 14. Expected Max 13 Bytes."
            };

            //Invalid CheckDigit
            yield return new object[]
            {
                $"27066028",
                $"Failed to parse Ean Barcode :{Environment.NewLine}Invalid GTIN CheckDigit '8', Expected '7'."
            };

            //Bogus Data
            yield return new object[]
            {
                $"+$$4BOGUS254",
                $"Failed to parse Ean Barcode :{Environment.NewLine}Invalid Ean value '+$$4BOGUS254'."
            };
        }
    }
}
