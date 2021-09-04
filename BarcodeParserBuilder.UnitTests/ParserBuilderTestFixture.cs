using BarcodeParserBuilder.EAN;
using BarcodeParserBuilder.GS1;
using BarcodeParserBuilder.HIBC;
using BarcodeParserBuilder.Infrastructure;
using BarcodeParserBuilder.MSI;
using BarcodeParserBuilder.PPN;
using BarcodeParserBuilder.UnitTests.EAN;
using BarcodeParserBuilder.UnitTests.GS1;
using BarcodeParserBuilder.UnitTests.HIBC;
using BarcodeParserBuilder.UnitTests.MSI;
using BarcodeParserBuilder.UnitTests.PPN;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace BarcodeParserBuilder.UnitTests
{
    public class ParserBuilderTestFixture : BaseBarcodeTestFixture
    {
        public static char GroupSeparator => GS1BarcodeParserBuilderTestFixture.GroupSeparator;
        public static string SymbologyPrefix => GS1128BarcodeParserBuilderTestFixture.SymbologyPrefix;
        public static string Prefix => PpnBarcodeParserBuilderTestFixture.Prefix;
        public static string Suffix => PpnBarcodeParserBuilderTestFixture.Suffix;

        public static IEnumerable<object[]> ValidGs1Barcodes() => GS1BarcodeParserBuilderTestFixture.ValidGs1Barcodes();
        public static IEnumerable<object[]> ValidGs1128Barcodes() => GS1128BarcodeParserBuilderTestFixture.ValidGs1128Barcodes();
        public static IEnumerable<object[]> ValidEanBarcodes() => EanBarcodeParserBuilderTestFixture.ValidEanBarcodes();
        public static IEnumerable<object[]> ValidMsiBarcodes() => MsiBarcodeParserBuilderTestFixture.ValidMsiBarcodes();
        public static IEnumerable<object[]> ValidPpnBarcodes() => PpnBarcodeParserBuilderTestFixture.ValidPpnBarcodes();
        public static IEnumerable<object[]> ValidHibcBarcodes() => HibcBarcodeParserBuilderTestFixture.ValidHibcParserBuilderBarcodes();

        [Theory]
        [MemberData(nameof(ValidBarcodes))]
        [MemberData(nameof(ValidGs1Barcodes))]
        [MemberData(nameof(ValidGs1128Barcodes))]
        [MemberData(nameof(ValidEanBarcodes))]
        [MemberData(nameof(ValidMsiBarcodes))]
        [MemberData(nameof(ValidPpnBarcodes))]
        [MemberData(nameof(ValidHibcBarcodes))]
        public void CanParseBarcodes(string barcode, Barcode expectedBarcode)
        {
            //Arrange & Act
            var parsed = ParserBuilder.TryParse(barcode, out var result, out var feedback);

            //Assert
            parsed.Should().BeTrue();
            feedback.Should().BeNull();
            CompareBarcodeObjects(expectedBarcode, result);
        }

        [Theory]
        [MemberData(nameof(ValidBarcodes))]
        [MemberData(nameof(ValidGs1Barcodes))]
        [MemberData(nameof(ValidGs1128Barcodes))]
        [MemberData(nameof(ValidEanBarcodes))]
        [MemberData(nameof(ValidMsiBarcodes))]
        [MemberData(nameof(ValidPpnBarcodes))]
        [MemberData(nameof(ValidHibcBarcodes))]
        public void CanBuildBarcodes(string expectedString, Barcode barcode)
        {
            //Arrange
            string result = null;

            //Act
            Action buildAction = () => result = ParserBuilder.Build(barcode);

            //Assert
            buildAction.Should().NotThrow();
            result.Should().Be(expectedString);
        }

        public static IEnumerable<object[]> ValidBarcodes()
        {
            var gs1Barcode = new GS1Barcode()
            {
                ProductCode = new TestProductCode("03574661451947", ProductCodeType.GTIN),
                BatchNumber = null,
                SerialNumber = null,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2099, 12, 31), "991200", GS1BarcodeParserBuilderTestFixture.GS1DateFormat),
                ProductionDate = new TestBarcodeDateTime(new DateTime(2002, 05, 04), "020504", GS1BarcodeParserBuilderTestFixture.GS1DateFormat)
            };
            gs1Barcode.Fields["20"].SetValue("BL");
            gs1Barcode.Fields["240"].SetValue("40600199T");
            gs1Barcode.Fields["30"].SetValue("1");
            gs1Barcode.Fields["71"].SetValue("025862471");
            gs1Barcode.Fields["98"].SetValue("15647");
            gs1Barcode.Fields["99"].SetValue("15489");

            //GS1
            yield return new object[]
            {
                $"0103574661451947110205041799120020BL24040600199T{GroupSeparator}301{GroupSeparator}71025862471{GroupSeparator}9815647{GroupSeparator}9915489",
                gs1Barcode
            };

            //GS1(Inuvair Nexthaler)
            yield return new object[]
            {
                $"0108025153000365101096438{GroupSeparator}172206172155H02AE137",
                new GS1Barcode()
                {
                    ProductCode = new TestProductCode("08025153000365", ProductCodeType.GTIN),
                    BatchNumber = "1096438",
                    SerialNumber = "55H02AE137",
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2022, 06, 17), "220617" , GS1BarcodeParserBuilderTestFixture.GS1DateFormat ),
                    ProductionDate = null
                }
            };

            //GS1128
            yield return new object[]
            {
                $"{SymbologyPrefix}0134567890123457{SymbologyPrefix}103456789{SymbologyPrefix}11020504{SymbologyPrefix}213456789-012",
                new GS1128Barcode()
                {
                    ProductCode = new TestProductCode("34567890123457", ProductCodeType.GTIN),
                    BatchNumber = "3456789",
                    SerialNumber = "3456789-012",
                    ExpirationDate = null,
                    ProductionDate = new TestBarcodeDateTime(new DateTime(2002, 05, 04), "020504", GS1BarcodeParserBuilderTestFixture.GS1DateFormat)
                }
            };

            //EAN13
            yield return new object[]
            {
                "5420046520228",
                new EanBarcode()
                {
                    ProductCode = new TestProductCode("5420046520228", ProductCodeType.EAN),
                }
            };

            //MSI
            yield return new object[]
            {
                "799273982",
                new MsiBarcode()
                {
                    ProductCode = new TestProductCode("799273982", ProductCodeType.MSI)
                }
            };

            //PPN
            yield return new object[]
            {
                $"{Prefix}9N111234568408{GroupSeparator}1TANDSOMEBatchNumber20{GroupSeparator}SHAHASERIAL12385{GroupSeparator}D201220{GroupSeparator}16D20201200{Suffix}",
                new PpnBarcode()
                {
                    ProductCode = new TestProductCode("111234568408", ProductCodeType.PPN),
                    BatchNumber = "ANDSOMEBatchNumber20",
                    SerialNumber = "HAHASERIAL12385",
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 12, 20), "201220", GS1BarcodeParserBuilderTestFixture.GS1DateFormat),
                    ProductionDate = new TestBarcodeDateTime(new DateTime(2020, 12, 31), "20201200", PpnBarcodeParserBuilderTestFixture.PPNDateFormat)
                }
            };

            //HIBC - 2D
            yield return new object[]
            {
                "+A99912345/$$90999910X3/$$+320011577DEFG45/16D20111212D",
                new HibcBarcode()
                {
                    ProductCode = new TestProductCode("1234", ProductCodeType.HIBC),
                    LabelerIdentificationCode = "A999",
                    UnitOfMeasure = 5,
                    SerialNumber = "77DEFG45",
                    BatchNumber = "10X3",
                    Quantity = 9999,
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 01, 15), "200115", "yyMMdd"),
                    ProductionDate = new TestBarcodeDateTime(new DateTime(2011, 12, 12), "20111212", "yyyyMMdd")
                }
            };

            //HIBC - 1D
            yield return new object[]
            {
                "+A999123457+$$90999910X37-+$$+320011577DEFG4572+16D201112127Z",
                new HibcBarcode(false)
                {
                    ProductCode = new TestProductCode("1234", ProductCodeType.HIBC),
                    LabelerIdentificationCode = "A999",
                    UnitOfMeasure = 5,
                    SerialNumber = "77DEFG45",
                    BatchNumber = "10X3",
                    Quantity = 9999,
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 01, 15), "200115", "yyMMdd"),
                    ProductionDate = new TestBarcodeDateTime(new DateTime(2011, 12, 12), "20111212", "yyyyMMdd")
                }
            };
        }
    }
}
