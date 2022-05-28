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
                new EanBarcode
                {
                    ProductCode = new TestProductCode("5420046520228", ProductCodeType.EAN),
                    ProductSystem = EanProductSystem.Create(5),
                } 
            };

            //UPC
            yield return new object[]
            {
                $"045496730086",
                new EanBarcode
                {
                    CompanyPrefix = "45496",
                    ProductCode = new TestProductCode("73008", ProductCodeType.EAN),
                    ProductSystem = EanProductSystem.Create(0),
                }
            };

            //UPC-A / NDC
            yield return new object[]
            {
                $"300450449108",
                new EanBarcode
                {
                    CompanyPrefix = null,
                    ProductCode = new TestProductCode("0045044910", ProductCodeType.NDC),
                    ProductSystem = EanProductSystem.Create(EanProductSystemScheme.NationalDrugCode),
                }
            };

            //EAN8
            yield return new object[]
            {
                $"27066027",
                new EanBarcode
                {
                    ProductCode = new TestProductCode("2706602", ProductCodeType.EAN),
                    ProductSystem = EanProductSystem.Create(2),
                }
            };

            //ISBN - Witcher
            yield return new object[]
            {
                $"9780316029186",
                new EanBarcode
                {
                    ProductCode = new TestProductCode("9780316029186", ProductCodeType.EAN),
                    ProductSystem = EanProductSystem.Create(9),
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
                $"9119727",
                $"Failed to parse Ean Barcode :{Environment.NewLine}Invalid value Length 7. Expected 8, 12 or 13 Bytes."
            };

            //ProductCode Too long
            yield return new object[]
            {
                $"91197254896410",
                $"Failed to parse Ean Barcode :{Environment.NewLine}Invalid value Length 14. Expected 8, 12 or 13 Bytes."
            };

            //Invalid CheckDigit
            yield return new object[]
            {
                $"27066028",
                $"Failed to parse Ean Barcode :{Environment.NewLine}Invalid Ean CheckDigit '8', Expected '7'."
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
