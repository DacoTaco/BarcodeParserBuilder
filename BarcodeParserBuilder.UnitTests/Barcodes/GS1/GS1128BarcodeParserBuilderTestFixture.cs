using System;
using System.Collections.Generic;
using BarcodeParserBuilder.Barcodes;
using BarcodeParserBuilder.Barcodes.GS1;
using BarcodeParserBuilder.Exceptions.GS1;
using BarcodeParserBuilder.Infrastructure;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.GS1
{
    public class GS1128BarcodeParserBuilderTestFixture : BaseBarcodeTestFixture
    {
        public static char GroupSeparator => (char)0x1D;
        public static string SymbologyPrefix => "]C1";

        [Theory]
        [MemberData(nameof(ValidGs1Barcodes))]
        [MemberData(nameof(ValidGs1ParsingBarcodes))]
        [MemberData(nameof(ValidGs1128Barcodes))]
        [MemberData(nameof(ValidGs1128ParsingBarcodes))]
        public void CanParseBarcodeString(string barcode, Barcode expectedBarcode)
        {
            //Arrange 
            //prepare the GS1 barcodes by converting the GS & other prefixes to the GS1-128.
            //after that, add the Symbology prefix to the GS1 barcodes
            if (barcode.StartsWith(']'))
                barcode = AimParser.StripBarcodePrefix(barcode);
            barcode = barcode.Replace(GroupSeparator.ToString(), SymbologyPrefix);
            if (!barcode.StartsWith(SymbologyPrefix))
                barcode = $"{SymbologyPrefix}{barcode}";

            //Act
            var parsed = GS1128BarcodeParserBuilder.TryParse(barcode, out var result);
            Action parseAction = () => GS1128BarcodeParserBuilder.Parse(barcode);

            //Assert
            parsed.Should().BeTrue();
            parseAction.Should().NotThrow();
            result.Should().NotBeNull();
            CompareBarcodeObjects(expectedBarcode, result, BarcodeType.GS1128);
        }

        [Theory]
        [MemberData(nameof(ValidGs1128Barcodes))]
        public void CanBuildBarcodeString(string expectedBarcode, GS1128Barcode barcode)
        {
            //Arrange
            string result = null;

            //Act
            Action buildAction = () => result = GS1128BarcodeParserBuilder.Build(barcode);

            //Assert
            buildAction.Should().NotThrow();
            result.Should().Be(expectedBarcode);
        }

        public static IEnumerable<object[]> ValidGs1Barcodes() => GS1BarcodeParserBuilderTestFixture.ValidGs1Barcodes();
        public static IEnumerable<object[]> ValidGs1ParsingBarcodes() => GS1BarcodeParserBuilderTestFixture.ValidGs1ParsingBarcodes();
        public static IEnumerable<object[]> ValidGs1128Barcodes()
        {
            var gs1128Barcode = new GS1128Barcode()
            {
                ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
                {
                    productCode.Type = ProductCodeType.GTIN;
                    productCode.Value = "357466145194";
                    productCode.Indicator = 0;
                }),
                BatchNumber = null,
                SerialNumber = null
            };
            gs1128Barcode.Fields["240"].SetValue("40600199T");
            gs1128Barcode.Fields["30"].SetValue(1);
            gs1128Barcode.Fields["71"].SetValue("025862471");

            //GS1128 - Multiple barcodes in 1 string
            yield return new object[]
            {
                $"{SymbologyPrefix}0103574661451947{SymbologyPrefix}24040600199T{SymbologyPrefix}301{SymbologyPrefix}71025862471",
                gs1128Barcode
            };

            //GS1128 - Single barcode
            yield return new object[]
            {
                $"{SymbologyPrefix}2121896418-5M",
                new GS1128Barcode()
                {
                    SerialNumber = "21896418-5M"
                }
            };
        }

        public static IEnumerable<object[]> ValidGs1128ParsingBarcodes()
        {
            //GS1128 - Random Order #1
            yield return new object[]
            {
                $"{SymbologyPrefix}0103574661451947{SymbologyPrefix}301{SymbologyPrefix}24040600199T{SymbologyPrefix}71025862471",
                new GS1128Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "357466145194";
                        productCode.Indicator = 0;
                    }),
                    BatchNumber = null,
                    SerialNumber = null
                }
            };

            //GS128 with AIM prefix
            var gs128barcode = new GS1128Barcode
            {
                ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("12345678901231", (productCode) =>
                {
                    productCode.Type = ProductCodeType.GTIN;
                    productCode.Value = "234567890123";
                    productCode.Indicator = 1;
                    productCode.Code = "12345678901231";
                }),
            };
            gs128barcode.Fields["15"].SetValue(new TestBarcodeDateTime(new DateTime(2099, 12, 31), "991231", GS1BarcodeParserBuilderTestFixture.GS1DateFormat));
            yield return new object[]
            {
                "]C1011234567890123115991231",
                gs128barcode
            };
        }

        [Theory]
        [MemberData(nameof(InValidGs1Barcodes))]
        [MemberData(nameof(InValidGs1128Barcodes))]
        public void InvalidGS1BarcodeStringThrowsException(string barcode, string expectedMessage)
        {
            //Arrange
            //prepare the GS1 barcodes by converting the GS to the GS1-128.
            barcode = barcode.Replace(GroupSeparator.ToString(), SymbologyPrefix);

            //Act
            var parsed = GS1128BarcodeParserBuilder.TryParse(barcode, out var result);
            Action parseAction = () => GS1128BarcodeParserBuilder.Parse(barcode);

            //Assert
            parsed.Should().BeFalse();
            parseAction.Should()
                .Throw<GS1128ParseException>()
                .WithMessage($"Failed to parse GS1-128 Barcode :{Environment.NewLine}{expectedMessage}");
            result.Should().BeNull();
        }

        public static IEnumerable<object[]> InValidGs1Barcodes()
        {
            foreach (var testCase in GS1BarcodeParserBuilderTestFixture.InValidGs1Barcodes())
            {
                yield return new object[]
                {
                    $"{SymbologyPrefix}{testCase[0]}",
                    testCase[1]
                };
            }
        }
        public static IEnumerable<object[]> InValidGs1128Barcodes()
        {
            //Invalid Prefix
            yield return new object[]
            {
                $"0134567890123457103456789{GroupSeparator}",
                $"Barcode does not start with the Symbology Prefix."
            };

            //bogus Prefix
            yield return new object[]
            {
                $"]C0134567890123457103456789{GroupSeparator}",
                $"Barcode does not start with the Symbology Prefix."
            };
        }
    }
}
