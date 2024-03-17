using BarcodeParserBuilder.Aim;
using BarcodeParserBuilder.Barcodes;
using BarcodeParserBuilder.Barcodes.CODE128;
using BarcodeParserBuilder.Barcodes.CODE39;
using BarcodeParserBuilder.Barcodes.EAN;
using BarcodeParserBuilder.Barcodes.GS1;
using BarcodeParserBuilder.Barcodes.HIBC;
using BarcodeParserBuilder.Barcodes.MSI;
using BarcodeParserBuilder.Barcodes.PPN;
using BarcodeParserBuilder.Infrastructure.ProductCodes;
using BarcodeParserBuilder.UnitTests.Barcodes.CODE128;
using BarcodeParserBuilder.UnitTests.Barcodes.CODE39;
using BarcodeParserBuilder.UnitTests.Barcodes.EAN;
using BarcodeParserBuilder.UnitTests.Barcodes.GS1;
using BarcodeParserBuilder.UnitTests.Barcodes.HIBC;
using BarcodeParserBuilder.UnitTests.Barcodes.MSI;
using BarcodeParserBuilder.UnitTests.Barcodes.PPN;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests;

public class BarcodeParserBuilderTestFixture : BaseBarcodeTestFixture
{
    public static char GroupSeparator => GS1BarcodeParserBuilderTestFixture.GroupSeparator;
    public static string SymbologyPrefix => GS1128BarcodeParserBuilderTestFixture.SymbologyPrefix;
    public static string Prefix => PpnBarcodeParserBuilderTestFixture.Prefix;
    public static string Suffix => PpnBarcodeParserBuilderTestFixture.Suffix;

    public static TheoryData<string> InvalidGs1Barcodes()
    {
        var theoryData = new TheoryData<string>();
        foreach (var data in GS1BarcodeParserBuilderTestFixture.InValidGs1Barcodes())
            theoryData.Add((string)data[0]);

        return theoryData;
    }
    public static TheoryData<string, GS1Barcode> ValidGs1Barcodes() => GS1BarcodeParserBuilderTestFixture.ValidGs1Barcodes();
    public static TheoryData<string, GS1128Barcode> ValidGs1128Barcodes() => GS1128BarcodeParserBuilderTestFixture.ValidGs1128Barcodes();
    public static TheoryData<string, EanBarcode> ValidEanBarcodes() => EanBarcodeParserBuilderTestFixture.ValidEanBarcodes();
    public static TheoryData<string?, MsiBarcode?> ValidMsiBarcodes() => MsiBarcodeParserBuilderTestFixture.ValidMsiBarcodes();
    public static TheoryData<string?, PpnBarcode?> ValidPpnBarcodes() => PpnBarcodeParserBuilderTestFixture.ValidPpnBarcodes();
    public static TheoryData<string, HibcBarcode> ValidHibcBarcodes() => HibcBarcodeParserBuilderTestFixture.ValidHibcParserBuilderBarcodes();
    public static TheoryData<string, Code39Barcode> ValidCode39Barcodes() => Code39ParserBuilderTestFixture.ValidCode39ParsingBarcodes();
    public static TheoryData<string, Code128Barcode> ValidCode128Barcodes() => Code128ParserBuilderTestFixture.ValidCode128ParsingBarcodes();

    private readonly BarcodeParserBuilder _parserBuilder;

    public BarcodeParserBuilderTestFixture()
    {
        _parserBuilder = new BarcodeParserBuilder();
    }

    [Theory]
    [MemberData(nameof(ValidBarcodes))]
    [MemberData(nameof(ValidParseBarcodes))]
    [MemberData(nameof(ValidGs1Barcodes))]
    [MemberData(nameof(ValidGs1128Barcodes))]
    [MemberData(nameof(ValidEanBarcodes))]
    [MemberData(nameof(ValidMsiBarcodes))]
    [MemberData(nameof(ValidPpnBarcodes))]
    [MemberData(nameof(ValidHibcBarcodes))]
    [MemberData(nameof(ValidCode39Barcodes))]
    [MemberData(nameof(ValidCode128Barcodes))]
    public void CanParseBarcodes(string? barcode, Barcode? expectedBarcode)
    {
        //Arrange & Act
        var parsed = _parserBuilder.TryParse(barcode, out var result, out var feedback);

        //Assert
        feedback.Should().BeNull();
        parsed.Should().BeTrue();
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
    [MemberData(nameof(ValidCode39Barcodes))]
    [MemberData(nameof(ValidCode128Barcodes))]
    public void CanBuildBarcodes(string? expectedString, Barcode? barcode)
    {
        //Arrange
        string? result = null;

        //Act
        Action buildAction = () => result = _parserBuilder.Build(barcode);

        //Assert
        buildAction.Should().NotThrow();
        result.Should().Be(expectedString);
    }

    [Theory]
    [MemberData(nameof(InvalidGs1Barcodes))]
    public void InvalidBarcodeThrowsError(string? barcode)
    {
        //Arrange & Act
        var result = _parserBuilder.TryParse(barcode, out var _, out var feedback);

        //Assert
        result.Should().BeFalse($" feedback must not signal success: {feedback}");
        feedback.Should().Be($"Failed to parse barcode : no parser could accept barcode '{barcode}'.");
    }

    public static TheoryData<string, Barcode> ValidParseBarcodes()
    {
        var dataList = EanBarcodeParserBuilderTestFixture.ValidEanParsingBarcodes()
            .Concat(GS1BarcodeParserBuilderTestFixture.ValidGs1ParsingBarcodes())
            .Concat(GS1128BarcodeParserBuilderTestFixture.ValidGs1128ParsingBarcodes())
            .Concat(HibcBarcodeParserBuilderTestFixture.ValidHibcParsingBarcodes())
            .Concat(PpnBarcodeParserBuilderTestFixture.ValidPpnParsingBarcodes());

        var theoryData = new TheoryData<string, Barcode>();
        foreach (var data in dataList)
            theoryData.Add((string)data[0], (Barcode)data[1]);

        return theoryData;
    }

    public static TheoryData<string, Barcode> ValidBarcodes()
    {
        var gs1Barcode = new GS1Barcode()
        {
            ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
            {
                productCode.Type = ProductCodeType.GTIN;
                productCode.Value = "357466145194";
                productCode.Indicator = 0;
            }),
            BatchNumber = null,
            SerialNumber = null,
            ExpirationDate = new TestBarcodeDateTime(new DateTime(2099, 12, 31), "991200", GS1BarcodeParserBuilderTestFixture.GS1DateFormat),
            ProductionDate = new TestBarcodeDateTime(new DateTime(2002, 05, 04), "020504", GS1BarcodeParserBuilderTestFixture.GS1DateFormat),
        };
        gs1Barcode.Fields["20"].SetValue("BL");
        gs1Barcode.Fields["240"].SetValue("40600199T");
        gs1Barcode.Fields["30"].SetValue(1);
        gs1Barcode.Fields["71"].SetValue("025862471");
        gs1Barcode.Fields["98"].SetValue("15647");
        gs1Barcode.Fields["99"].SetValue("15489");

        return new TheoryData<string, Barcode>()
        {
            //GS1
            {
                $"0103574661451947110205041799120020BL24040600199T{GroupSeparator}301{GroupSeparator}71025862471{GroupSeparator}9815647{GroupSeparator}9915489",
                gs1Barcode
            },

            //GS1(Inuvair Nexthaler)
            {
                $"0108025153000365101096438{GroupSeparator}172206172155H02AE137",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("08025153000365", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "802515300036";
                        productCode.Indicator = 0;
                    }),
                    BatchNumber = "1096438",
                    SerialNumber = "55H02AE137",
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2022, 06, 17), "220617" , GS1BarcodeParserBuilderTestFixture.GS1DateFormat ),
                    ProductionDate = null
                }
            },

            //GS1128
            {
                $"{SymbologyPrefix}0134567890123457{SymbologyPrefix}103456789{SymbologyPrefix}11020504{SymbologyPrefix}213456789-012",
                new GS1128Barcode(new Code128SymbologyIdentifier("C1"))
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("34567890123457", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "456789012345";
                        productCode.Indicator = 3;
                    }),
                    BatchNumber = "3456789",
                    SerialNumber = "3456789-012",
                    ExpirationDate = null,
                    ProductionDate = new TestBarcodeDateTime(new DateTime(2002, 05, 04), "020504", GS1BarcodeParserBuilderTestFixture.GS1DateFormat),
                }
            },

            //EAN13
            {
                "5420046520228",
                new EanBarcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("5420046520228", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.EAN;
                        productCode.Value = "542004652022";
                    }),
                }
            },

            //MSI
            {
                "799273982",
                new MsiBarcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<MsiProductCode>("799273982")
                }
            },

            //PPN
            {
                $"{Prefix}9N111234568408{GroupSeparator}1TANDSOMEBatchNumber20{GroupSeparator}SHAHASERIAL12385{GroupSeparator}D201220{GroupSeparator}16D20201200{Suffix}",
                new PpnBarcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<PpnProductCode>("111234568408"),
                    BatchNumber = "ANDSOMEBatchNumber20",
                    SerialNumber = "HAHASERIAL12385",
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 12, 20), "201220", GS1BarcodeParserBuilderTestFixture.GS1DateFormat),
                    ProductionDate = new TestBarcodeDateTime(new DateTime(2020, 12, 31), "20201200", PpnBarcodeParserBuilderTestFixture.PPNDateFormat)
                }
            },

            //HIBC - 2D
            {
                "+A99912345/$$909999710X3/$$+320011577DEFG45/16D20111212K",
                new HibcBarcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("1234"),
                    LabelerIdentificationCode = "A999",
                    UnitOfMeasure = 5,
                    SerialNumber = "77DEFG45",
                    BatchNumber = "10X3",
                    Quantity = 9999,
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 01, 15), "200115", "yyMMdd"),
                    ProductionDate = new TestBarcodeDateTime(new DateTime(2011, 12, 12), "20111212", "yyyyMMdd")
                }
            },

            //HIBC - 1D
            {
                "+A999123457+$$909999710X370+$$+320011577DEFG4572+16D201112127Z",
                new HibcBarcode(false)
                {
                    ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("1234"),
                    LabelerIdentificationCode = "A999",
                    UnitOfMeasure = 5,
                    SerialNumber = "77DEFG45",
                    BatchNumber = "10X3",
                    Quantity = 9999,
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 01, 15), "200115", "yyMMdd"),
                    ProductionDate = new TestBarcodeDateTime(new DateTime(2011, 12, 12), "20111212", "yyyyMMdd")
                }
            },
        };
    }
}
