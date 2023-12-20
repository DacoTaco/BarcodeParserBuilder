using BarcodeParserBuilder.Aim;
using BarcodeParserBuilder.Barcodes;
using BarcodeParserBuilder.Barcodes.GS1;
using BarcodeParserBuilder.Exceptions.GS1;
using BarcodeParserBuilder.Infrastructure;
using BarcodeParserBuilder.Infrastructure.ProductCodes;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.GS1;

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
        var aimIdentifier = expectedBarcode is GS1Barcode
            ? new Code128SymbologyIdentifier("C1")
            : expectedBarcode.ReaderInformation;

        barcode = expectedBarcode.ReaderInformation?.StripSymbologyIdentifier(barcode!) ?? barcode!;
        barcode = barcode.Replace(GroupSeparator.ToString(), SymbologyPrefix);
        if (!barcode.StartsWith(SymbologyPrefix))
            barcode = $"{SymbologyPrefix}{barcode}";
        typeof(Barcode).GetProperty(nameof(Barcode.ReaderInformation))!.SetValue(expectedBarcode, aimIdentifier);

        //Act
        var parsed = GS1128BarcodeParserBuilder.TryParse(barcode, aimIdentifier, out var result);
        Action parseAction = () => GS1128BarcodeParserBuilder.Parse(barcode, aimIdentifier);

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
        string? result = null;

        //Act
        Action buildAction = () => result = GS1128BarcodeParserBuilder.Build(barcode);

        //Assert
        buildAction.Should().NotThrow();
        result.Should().Be(expectedBarcode);
    }

    public static TheoryData<string, GS1Barcode> ValidGs1Barcodes() => GS1BarcodeParserBuilderTestFixture.ValidGs1Barcodes();
    public static TheoryData<string, GS1Barcode> ValidGs1ParsingBarcodes() => GS1BarcodeParserBuilderTestFixture.ValidGs1ParsingBarcodes();
    public static TheoryData<string, GS1128Barcode> ValidGs1128Barcodes()
    {
        var gs1128Barcode = new GS1128Barcode(new Code128SymbologyIdentifier("C1"))
        {
            ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
            {
                productCode.Type = ProductCodeType.GTIN;
                productCode.Value = "357466145194";
                productCode.Indicator = 0;
            }),
            BatchNumber = null,
            SerialNumber = null,
        };
        gs1128Barcode.Fields["240"].SetValue("40600199T");
        gs1128Barcode.Fields["30"].SetValue(1);
        gs1128Barcode.Fields["71"].SetValue("025862471");

        return new TheoryData<string, GS1128Barcode>()
        {
            {
                $"{SymbologyPrefix}0103574661451947{SymbologyPrefix}24040600199T{SymbologyPrefix}301{SymbologyPrefix}71025862471",
                gs1128Barcode
            },

            //GS1128 - Single barcode
            {
                $"{SymbologyPrefix}2121896418-5M",
                new GS1128Barcode(new Code128SymbologyIdentifier("C1"))
                {
                    SerialNumber = "21896418-5M",
                }
            },
        };
    }

    public static TheoryData<string, GS1128Barcode> ValidGs1128ParsingBarcodes()
    {
        var gs128barcode = new GS1128Barcode(new Code128SymbologyIdentifier("C1"))
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

        return new TheoryData<string, GS1128Barcode>()
        {
            //GS1128 - Random Order #1
            {
                $"{SymbologyPrefix}0103574661451947{SymbologyPrefix}301{SymbologyPrefix}24040600199T{SymbologyPrefix}71025862471",
                new GS1128Barcode(new Code128SymbologyIdentifier("C1"))
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "357466145194";
                        productCode.Indicator = 0;
                    }),
                    BatchNumber = null,
                    SerialNumber = null,
                }
            },

            //GS128 with AIM prefix 
            {
                "]C1011234567890123115991231",
                gs128barcode
            },
        };
    }

    [Theory]
    [MemberData(nameof(InValidGs1Barcodes))]
    public void InvalidGS1BarcodeStringThrowsException(string barcode, string expectedMessage)
    {
        //Arrange
        //prepare the GS1 barcodes by converting the GS to the GS1-128.
        barcode = barcode.Replace(GroupSeparator.ToString(), SymbologyPrefix);
        var identifier = new Code128SymbologyIdentifier("C1");

        //Act
        var parsed = GS1128BarcodeParserBuilder.TryParse(barcode, identifier, out var result);
        Action parseAction = () => GS1128BarcodeParserBuilder.Parse(barcode, identifier);

        //Assert
        parsed.Should().BeFalse();
        parseAction.Should()
            .Throw<GS1128ParseException>()
            .WithMessage($"Failed to parse GS1-128 Barcode :{Environment.NewLine}{expectedMessage}");
        result.Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(InValidGs1128Barcodes))]
    public void InvalidGS1128BarcodeStringThrowsException(string barcode, AimSymbologyIdentifier? identifier, string expectedMessage)
    {
        //Arrange
        //prepare the GS1 barcodes by converting the GS to the GS1-128.
        barcode = barcode.Replace(GroupSeparator.ToString(), SymbologyPrefix);

        //Act
        var parsed = GS1128BarcodeParserBuilder.TryParse(barcode, identifier, out var result);
        Action parseAction = () => GS1128BarcodeParserBuilder.Parse(barcode, identifier);

        //Assert
        parsed.Should().BeFalse();
        parseAction.Should()
            .Throw<GS1128ParseException>()
            .WithMessage($"Failed to parse GS1-128 Barcode :{Environment.NewLine}{expectedMessage}");
        result.Should().BeNull();
    }

    public static TheoryData<string, string> InValidGs1Barcodes()
    {
        var theoryData = new TheoryData<string, string>();
        foreach (var testCase in GS1BarcodeParserBuilderTestFixture.InValidGs1Barcodes())
            theoryData.Add($"{SymbologyPrefix}{testCase[0]}", (string)testCase[1]);

        return theoryData;
    }
    public static TheoryData<string, AimSymbologyIdentifier?, string> InValidGs1128Barcodes() => new()
    {
        //Invalid Prefix
        {
            $"0134567890123457103456789{GroupSeparator}",
            null,
            $"Invalid GS1-128 identifier."
        },

        //bogus Prefix
        {
            $"]C0134567890123457103456789{GroupSeparator}",
            AimSymbologyIdentifier.ParseString<Code128SymbologyIdentifier>("]C0"),
            $"Barcode does not start with the Symbology Prefix."
        },
    };
}
