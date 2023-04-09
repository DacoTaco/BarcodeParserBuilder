using BarcodeParserBuilder.Exceptions.MSI;
using BarcodeParserBuilder.Infrastructure;
using BarcodeParserBuilder.Barcodes.MSI;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.MSI
{
    public class MsiBarcodeParserBuilderTestFixture : BaseBarcodeTestFixture
    {
        [Theory]
        [MemberData(nameof(ValidMsiBarcodes))]
        [MemberData(nameof(ValidMsiParseBarcodes))]
        public void CanParseBarcodeString(string barcode, MsiBarcode expectedBarcode)
        {
            //Arrange & Act
            Action buildAction = () => MsiBarcodeParserBuilder.Parse(barcode);
            var parsed = MsiBarcodeParserBuilder.TryParse(barcode, out var result);

            //Assert
            parsed.Should().BeTrue("barcode should be parsable");
            buildAction.Should().NotThrow();
            CompareBarcodeObjects(expectedBarcode, result);
        }

        [Theory]
        [MemberData(nameof(ValidMsiBarcodes))]
        public void CanBuildBarcodeString(string expectedString, MsiBarcode barcode)
        {
            //Arrange & Act
            string result = null;
            Action buildAction = () => result = MsiBarcodeParserBuilder.Build(barcode);

            //Assert
            buildAction.Should().NotThrow();
            result.Should().Be(expectedString);
        }

        public static IEnumerable<object[]> ValidMsiBarcodes()
        {
            //Empty Barcode/Object
            yield return new object[]
            {
                null,
                null
            };

            //APB - Motilium
            yield return new object[]
            {
                $"1446921",
                new MsiBarcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<MsiProductCode>("1446921"),
                }
            };

            //APB - Lexatif Doux
            yield return new object[]
            {
                $"3339355",
                new MsiBarcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<MsiProductCode>("3339355"),
                }
            };

            //MSI - Mod11
            yield return new object[]
            {
                $"12345674",
                new MsiBarcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<MsiProductCode>("12345674"),
                }
            };

            //MSI - Random Code #1
            yield return new object[]
            {
                $"80523",
                new MsiBarcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<MsiProductCode>("80523"),
                }
            };

            //MSI - Random Code #2 (mod1010)
            yield return new object[]
            {
                $"123456741",
                new MsiBarcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<MsiProductCode>("123456741"),
                }
            };
        }

        public static IEnumerable<object[]> ValidMsiParseBarcodes()
        {
            //APB - Dafalgan 500mg
            yield return new object[]
            {
                $"]M033915059",
                new MsiBarcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<MsiProductCode>("33915059"),
                }
            };
        }

        [Theory]
        [MemberData(nameof(InvalidMsiBarcodes))]
        public void InvalidBarcodeStringThrowsException(string barcode, string expectedMessage)
        {
            //Arrange & Act
            Action parseAction = () => MsiBarcodeParserBuilder.Parse(barcode);
            var parsed = MsiBarcodeParserBuilder.TryParse(barcode, out var result);

            //Assert
            parsed.Should().BeFalse("barcode should be not parsable");
            parseAction.Should()
                .Throw<MsiParseException>()
                .WithMessage(expectedMessage);
            result.Should().BeNull();
        }

        public static IEnumerable<object[]> InvalidMsiBarcodes()
        {
            //ProductCode Too Short
            yield return new object[]
            {
                $"91",
                $"Failed to parse MSI Barcode :{Environment.NewLine}Invalid string value '91' : Too small (2/3)."
            };

            //Invalid CheckDigit
            yield return new object[]
            {
                $"27066028",
                $"Failed to parse MSI Barcode :{Environment.NewLine}Invalid MSI CheckDigit '8'."
            };

            //Bogus Data
            yield return new object[]
            {
                $"+$$4BOGUS",
                $"Failed to parse MSI Barcode :{Environment.NewLine}Invalid MSI value '+$$4BOGUS'."
            };
        }
    }
}
