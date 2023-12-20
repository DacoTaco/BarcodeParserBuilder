using BarcodeParserBuilder.Aim;
using BarcodeParserBuilder.Barcodes.EAN;
using BarcodeParserBuilder.Exceptions.EAN;
using BarcodeParserBuilder.Infrastructure.ProductCodes;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.EAN;

public class EanBarcodeParserBuilderTestFixture : BaseBarcodeTestFixture
{
    [Theory]
    [MemberData(nameof(ValidEanBarcodes))]
    [MemberData(nameof(ValidEanParsingBarcodes))]
    public void CanParseBarcodeString(string barcode, EanBarcode expectedBarcode)
    {
        //Arrange & Act
        EanBarcodeParserBuilder.TryParse(barcode, expectedBarcode.ReaderInformation, out var result).Should().BeTrue($"'{barcode}' should be parsable");
        Action parseAction = () => EanBarcodeParserBuilder.Parse(barcode, expectedBarcode.ReaderInformation);

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
        string? result = null;
        Action buildAction = () => result = EanBarcodeParserBuilder.Build(barcode);

        //Assert
        buildAction.Should().NotThrow($"'{barcode}' should be buildable");
        result.Should().NotBeNull();
        result.Should().Be(expectedString);
    }

    public static TheoryData<string, EanBarcode> ValidEanParsingBarcodes() => new()
    {
        //EAN13
        {
            $"]E05420046520228",
            new EanBarcode(new EanSymbologyIdentifier("E0"))
            {
                ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("5420046520228", (productCode) =>
                {
                    productCode.Type = ProductCodeType.EAN;
                    productCode.Schema = GtinProductScheme.Unknown;
                    productCode.Value = "542004652022";
                })
            }
        },

        //EAN13
        {
            "]E01234567890128",
            new EanBarcode(new EanSymbologyIdentifier("E0"))
            {
                ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("1234567890128", (productCode) =>
                {
                    productCode.Type = ProductCodeType.EAN;
                    productCode.Value = "123456789012";
                    productCode.Code = "1234567890128";
                })
            }
        },
    };

    public static TheoryData<string, EanBarcode> ValidEanBarcodes() => new()
    {
        //EAN13
        {
            $"5420046520228",
            new EanBarcode
            {
                ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("5420046520228", (productCode) =>
                {
                    productCode.Type = ProductCodeType.EAN;
                    productCode.Schema = GtinProductScheme.Unknown;
                    productCode.Value = "542004652022";
                }),
            }
        },

        //UPC in EAN13
        {
            $"0420046520223",
            new EanBarcode
            {
                ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("0420046520223", (productCode) =>
                {
                    productCode.Type = ProductCodeType.EAN;
                    productCode.Schema = GtinProductScheme.ReservedCoupons;
                    productCode.Value = "2004652022";
                }),
            }
        },

        //UPC
        {
            $"045496730086",
            new EanBarcode
            {
                ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("045496730086", (productCode) =>
                {
                    productCode.Type = ProductCodeType.EAN;
                    productCode.Schema = GtinProductScheme.ManufacturerAndProduct;
                    productCode.Value = "73008";
                    productCode.CompanyIdentifier = "4549";
                }),
            }
        },

        //UPC-A / NDC
        {
            $"300450449108",
            new EanBarcode
            {
                ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("300450449108", (productCode) =>
                {
                    productCode.Type = ProductCodeType.EAN;
                    productCode.Schema = GtinProductScheme.NationalDrugCode;
                    productCode.Value = "0045044910";
                }),
            }
        },

        //EAN8
        {
            $"27066027",
            new EanBarcode
            {
                ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("27066027", (productCode) =>
                {
                    productCode.Type = ProductCodeType.EAN;
                    productCode.Schema = GtinProductScheme.Unknown;
                    productCode.Value = "2706602";
                }),
            }
        },

        //ISBN - Witcher
        {
            $"9780316029186",
            new EanBarcode
            {
                ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("9780316029186", (productCode) =>
                {
                    productCode.Type = ProductCodeType.EAN;
                    productCode.Schema = GtinProductScheme.Unknown;
                    productCode.Value = "978031602918";
                }),
            }
        },
    };

    [Theory]
    [MemberData(nameof(InValidEanBarcodes))]
    public void InvalidBarcodeStringThrowsException(string barcode, string expectedMessage)
    {
        //Arrange & Act
        EanBarcodeParserBuilder.TryParse(barcode, null, out var result).Should().BeFalse($"'{barcode}' should not be parsable");
        Action parseAction = () => EanBarcodeParserBuilder.Parse(barcode, null);

        //Assert
        parseAction.Should()
            .Throw<EanParseException>()
            .WithMessage(expectedMessage);
    }

    public static TheoryData<string, string> InValidEanBarcodes() => new()
    {
        //ProductCode Too Short
        {
            $"911972",
            $"Failed to parse Ean Barcode :{Environment.NewLine}Invalid GTIN/EAN Length of 6."
        },

        //ProductCode Too long
        {
            $"91197254896410",
            $"Failed to parse Ean Barcode :{Environment.NewLine}Invalid ProductCode type 'GTIN'."
        },

        //Invalid CheckDigit
        {
            $"27066028",
            $"Failed to parse Ean Barcode :{Environment.NewLine}Invalid GTIN/EAN CheckDigit '8', Expected '7'."
        },

        //Bogus Data
        {
            $"+$$4BOGUS254",
            $"Failed to parse Ean Barcode :{Environment.NewLine}Invalid GTIN/EAN value '+$$4BOGUS254'."
        },
    };
}
