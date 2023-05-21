using BarcodeParserBuilder.Barcodes.EAN;
using BarcodeParserBuilder.Exceptions.EAN;
using BarcodeParserBuilder.Infrastructure;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.EAN
{
    public class EanBarcodeParserBuilderTestFixture : BaseBarcodeTestFixture
    {
        [Theory]
        [MemberData(nameof(ValidEanBarcodes))]
        [MemberData(nameof(ValidEanParsingBarcodes))]
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

        public static IEnumerable<object[]> ValidEanParsingBarcodes()
        {
            //EAN13
            yield return new object[]
            {
                $"]E05420046520228",
                new EanBarcode
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("5420046520228", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.EAN;
                        productCode.Schema = GtinProductScheme.Unknown;
                        productCode.Value = "542004652022";
                    }),
                }
            };

            //EAN13
            yield return new object[]
            {
                "]E01234567890128",
                new EanBarcode
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("1234567890128", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.EAN;
                        productCode.Value = "123456789012";
                        productCode.Code = "1234567890128";
                    }),
                }
            };
        }

        public static IEnumerable<object[]> ValidEanBarcodes()
        {
            //EAN13
            yield return new object[]
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
            };

            //UPC in EAN13
            yield return new object[]
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
            };

            //UPC
            yield return new object[]
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
            };

            //UPC-A / NDC
            yield return new object[]
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
            };

            //EAN8
            yield return new object[]
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
            };

            //ISBN - Witcher
            yield return new object[]
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
                $"911972",
                $"Failed to parse Ean Barcode :{Environment.NewLine}Invalid GTIN/EAN Length of 6."
            };

            //ProductCode Too long
            yield return new object[]
            {
                $"91197254896410",
                $"Failed to parse Ean Barcode :{Environment.NewLine}Invalid ProductCode type 'GTIN'."
            };

            //Invalid CheckDigit
            yield return new object[]
            {
                $"27066028",
                $"Failed to parse Ean Barcode :{Environment.NewLine}Invalid GTIN/EAN CheckDigit '8', Expected '7'."
            };

            //Bogus Data
            yield return new object[]
            {
                $"+$$4BOGUS254",
                $"Failed to parse Ean Barcode :{Environment.NewLine}Invalid GTIN/EAN value '+$$4BOGUS254'."
            };
        }
    }
}
