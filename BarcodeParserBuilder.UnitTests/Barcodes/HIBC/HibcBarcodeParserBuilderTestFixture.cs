using BarcodeParserBuilder.Aim;
using BarcodeParserBuilder.Barcodes.HIBC;
using BarcodeParserBuilder.Exceptions.HIBC;
using BarcodeParserBuilder.Infrastructure.ProductCodes;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.HIBC;

public class HibcBarcodeParserBuilderTestFixture : BaseBarcodeTestFixture
{
    [Theory]
    [MemberData(nameof(ValidHibcParsingBarcodes))]
    [MemberData(nameof(ValidHibcParserBuilderBarcodes))]
    public void CanParseBarcodeString(string barcode, HibcBarcode expectedBarcode)
    {
        //Arrange & Act
        var parsed = HibcBarcodeParserBuilder.TryParse(barcode, expectedBarcode?.ReaderInformation, out var result);
        Action parseAction = () => HibcBarcodeParserBuilder.Parse(barcode, expectedBarcode?.ReaderInformation);

        //Assert
        parsed.Should().BeTrue();
        parseAction.Should().NotThrow();
        result.Should().NotBeNull();
        CompareBarcodeObjects(expectedBarcode, result);
    }

    [Theory]
    [MemberData(nameof(ValidHibcBuildingBarcodes))]
    [MemberData(nameof(ValidHibcParserBuilderBarcodes))]
    public void CanBuildBarcodeString(string expectedBarcode, HibcBarcode barcode)
    {
        //Arrange & Act
        string? result = null;
        Action buildAction = () => result = HibcBarcodeParserBuilder.Build(barcode);

        //Assert
        buildAction.Should().NotThrow();
        result.Should().NotBeNull();
        result.Should().Be(expectedBarcode);
    }

    public static TheoryData<string, HibcBarcode> ValidHibcParsingBarcodes() => new()
    {
        //1D - Multiple Lines concat - Old Format
        {
            "+A123BJC5D6E71G+2001510X3GG",
            new HibcBarcode(false)
            {
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("BJC5D6E7"),
                LabelerIdentificationCode = "A123",
                UnitOfMeasure = 1,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 01, 15), "20015", "yyJJJ"),
                BatchNumber = "10X3",
            }
        },

        //1D - Multiple Lines concat - Old Format - aztec prefix
        {
            "]zB+A123BJC5D6E71G+2001510X3GG",
            new HibcBarcode(AimSymbologyIdentifier.ParseString("]zB"), false)
            {
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("BJC5D6E7"),
                LabelerIdentificationCode = "A123",
                UnitOfMeasure = 1,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 01, 15), "20015", "yyJJJ"),
                BatchNumber = "10X3",
            }
        },

        //1D - Old Format
        {
            "+J123AQ3451T+25330BC34567T2+S4012TJ",
            new HibcBarcode(false)
            {
                LabelerIdentificationCode = "J123",
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("AQ345"),
                UnitOfMeasure = 1,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2025, 11, 26), "25330", "yyJJJ"),
                BatchNumber = "BC34567",
                SerialNumber = "4012"
            }
        },

        //2D - Old Format
        {
            "+J123AQ3451/25330BC34567/S4012X",
            new HibcBarcode()
            {
                LabelerIdentificationCode = "J123",
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("AQ345"),
                UnitOfMeasure = 1,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2025, 11, 26), "25330", "yyJJJ"),
                BatchNumber = "BC34567",
                SerialNumber = "4012"
            }
        },

        //2D - Old Format - QRCode
        {
            "]Q0+J123AQ3451/25330BC34567/S4012X",
            new HibcBarcode(AimSymbologyIdentifier.ParseString("]Q0"))
            {
                LabelerIdentificationCode = "J123",
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("AQ345"),
                UnitOfMeasure = 1,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2025, 11, 26), "25330", "yyJJJ"),
                BatchNumber = "BC34567",
                SerialNumber = "4012",
            }
        },

        //2D - Online Example #1
        {
            "+A123ABCDEFGHI1234567891/$$420020216LOT123456789012345/SXYZ456789012345678/16D20130202$",
            new HibcBarcode()
            {
                LabelerIdentificationCode = "A123",
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("ABCDEFGHI123456789"),
                UnitOfMeasure = 1,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 02, 02, 16, 00, 00), "20020216", "yyMMddHH"),
                BatchNumber = "LOT123456789012345",
                SerialNumber = "XYZ456789012345678",
                ProductionDate = new TestBarcodeDateTime(new DateTime(2013, 02, 02), "20130202", "yyyyMMdd")
            }
        },

        //2D - Online Example #1 - DataMatrix
        {
            "]d1+A123ABCDEFGHI1234567891/$$420020216LOT123456789012345/SXYZ456789012345678/16D20130202$",
            new HibcBarcode(AimSymbologyIdentifier.ParseString("]d1"))
            {
                LabelerIdentificationCode = "A123",
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("ABCDEFGHI123456789"),
                UnitOfMeasure = 1,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 02, 02, 16, 00, 00), "20020216", "yyMMddHH"),
                BatchNumber = "LOT123456789012345",
                SerialNumber = "XYZ456789012345678",
                ProductionDate = new TestBarcodeDateTime(new DateTime(2013, 02, 02), "20130202", "yyyyMMdd"),
            }
        },

        //2D - Quantity
        {
            "+A99912345/$$812710X3/16D20111212/14D20200906/S77DEFG453",
            new HibcBarcode()
            {
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("1234"),
                LabelerIdentificationCode = "A999",
                UnitOfMeasure = 5,
                SerialNumber = "77DEFG45",
                BatchNumber = "10X3",
                Quantity = 12,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 09, 06), "20200906", "yyyyMMdd"),
                ProductionDate = new TestBarcodeDateTime(new DateTime(2011, 12, 12), "20111212", "yyyyMMdd")
            }
        },

        //2D - Quantity #2
        {
            "+A99912345/$$902345710X3/16D20111212/14D20200906/S77DEFG45F",
            new HibcBarcode()
            {
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("1234"),
                LabelerIdentificationCode = "A999",
                UnitOfMeasure = 5,
                SerialNumber = "77DEFG45",
                BatchNumber = "10X3",
                Quantity = 2345,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 09, 06), "20200906", "yyyyMMdd"),
                ProductionDate = new TestBarcodeDateTime(new DateTime(2011, 12, 12), "20111212", "yyyyMMdd")
            }
        },

        //+E203PB414109/$$8243280112R00089525J
        {
            "+E203PB414109/$$8243280112R00089525J",
            new HibcBarcode()
            {
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("PB41410"),
                LabelerIdentificationCode = "E203",
                UnitOfMeasure = 9,
                BatchNumber = "R00089525",
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2028, 01, 12), "280112", "yyMMdd"),
                Quantity = 24
            }
        },

        // +A99912349/$10X3/16D20111231/14D20200131/Q500Z  sample data from HIBC 2.6 Spec
        {
            "+A99912349/$10X3/16D20111231/14D20200131/Q500Z",
            new HibcBarcode()
            {
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("1234"),
                LabelerIdentificationCode = "A999",
                UnitOfMeasure = 9,
                BatchNumber = "10X3",
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 01, 31), "20200131", "yyyyMMdd"),
                ProductionDate = new TestBarcodeDateTime(new DateTime(2011, 12, 31), "20111231", "yyyyMMdd"),
                Quantity = 500
            }
        },

        //2D - example of quantity without batch number
        {
            "+EHWD3551419/$$900100F",
            new HibcBarcode()
            {
                LabelerIdentificationCode = "EHWD",
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("355141"),
                UnitOfMeasure = 9,
                Quantity = 100
            }
        },
        // HIBC 2.3.2.4 has note about Q, saying that that Q should (not shall!) be used with UOM of 9. So Q with other UOMs is also valid.
        {
            "]d1+EFOR78712011/$26485266/16D20240626/Q10S",
            new HibcBarcode(AimSymbologyIdentifier.ParseString("]d1"))
            {
                LabelerIdentificationCode = "EFOR",
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("7871201"),
                UnitOfMeasure = 1, // Valid UOM other than 9 with Q
                Quantity = 10,
                BatchNumber = "26485266",
                ProductionDate = new TestBarcodeDateTime(new DateTime(2024, 06, 26), "20240626", "yyyyMMdd")

            }
        }
    };

    public static TheoryData<string, HibcBarcode> ValidHibcBuildingBarcodes() => new()
    {
        //1D - Multiple Lines concat - Old Format - aztec prefix
        {
            "]zB+A123BJC5D6E71G+$$52001510X3GD",
            new HibcBarcode(AimSymbologyIdentifier.ParseString("]zB"), false)
            {
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("BJC5D6E7"),
                LabelerIdentificationCode = "A123",
                UnitOfMeasure = 1,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 01, 15), "20015", "yyJJJ"),
                BatchNumber = "10X3",
            }
        },

        //2D - Online Example #1
        {
            "+A123ABCDEFGHI1234567891/$$420020216LOT123456789012345/16D20130202/SXYZ456789012345678$",
            new HibcBarcode()
            {
                LabelerIdentificationCode = "A123",
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("ABCDEFGHI123456789"),
                UnitOfMeasure = 1,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 02, 02, 16, 00, 00), "20020216", "yyMMddHH"),
                BatchNumber = "LOT123456789012345",
                SerialNumber = "XYZ456789012345678",
                ProductionDate = new TestBarcodeDateTime(new DateTime(2013, 02, 02), "20130202", "yyyyMMdd")
            }
        },

        //2D - Changing DateTime format
        {
            "+A123ABCDEFGHI1234567891/$$3200202LOT123456789012345/16D20130202/SXYZ456789012345678V",
            new HibcBarcode()
            {
                LabelerIdentificationCode = "A123",
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("ABCDEFGHI123456789"),
                UnitOfMeasure = 1,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 02, 02), "200202", "yyMMdd"),
                BatchNumber = "LOT123456789012345",
                SerialNumber = "XYZ456789012345678",
                ProductionDate = new TestBarcodeDateTime(new DateTime(2013, 02, 02), "130202", "yyMMdd")
            }
        },

        //2D - Quantity
        {
            "+A99912345/$$812710X3/$$+320090677DEFG45/16D20111212.",
            new HibcBarcode()
            {
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("1234"),
                LabelerIdentificationCode = "A999",
                UnitOfMeasure = 5,
                SerialNumber = "77DEFG45",
                BatchNumber = "10X3",
                Quantity = 12,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 09, 06), "200906", "yyMMdd"),
                ProductionDate = new TestBarcodeDateTime(new DateTime(2011, 12, 12), "111212", "yyMMdd")
            }
        },

        //2D - Quantity #2
        {
            "+A99912345/$$902345710X3/$$+320090677DEFG45/16D201112126",
            new HibcBarcode()
            {
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("1234"),
                LabelerIdentificationCode = "A999",
                UnitOfMeasure = 5,
                SerialNumber = "77DEFG45",
                BatchNumber = "10X3",
                Quantity = 2345,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 09, 06), "200906", "yyMMdd"),
                ProductionDate = new TestBarcodeDateTime(new DateTime(2011, 12, 12), "111212", "yyMMdd")
            }
        },
    };

    public static TheoryData<string, HibcBarcode> ValidHibcParserBuilderBarcodes() => new()
    {
        //1D - Multiple Lines concat
        {
            "+A123BJC5D6E71G+$$52001510X3GD",
            new HibcBarcode(false)
            {
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("BJC5D6E7"),
                LabelerIdentificationCode = "A123",
                UnitOfMeasure = 1,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 01, 15), "20015", "yyJJJ"),
                BatchNumber = "10X3",
            }
        },

        //1D - Multiple Lines with batch
        {
            "+A123BJC5D6E71G+$$710X3G7",
            new HibcBarcode(false)
            {
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("BJC5D6E7"),
                LabelerIdentificationCode = "A123",
                UnitOfMeasure = 1,
                BatchNumber = "10X3",
            }
        },

        //1D - ExpirationDate + SerialNumber
        {
            "+A123BJC5D6E71G+$$+52001510X3GB",
            new HibcBarcode(false)
            {
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("BJC5D6E7"),
                LabelerIdentificationCode = "A123",
                UnitOfMeasure = 1,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 01, 15), "20015", "yyJJJ"),
                SerialNumber = "10X3"
            }
        },

        //1D - Multiple Lines with special link character
        {
            "+A123BJCMD6E79++$$52001510X3+ ",
            new HibcBarcode(false)
            {
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("BJCMD6E7"),
                LabelerIdentificationCode = "A123",
                UnitOfMeasure = 9,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 01, 15), "20015", "yyJJJ"),
                BatchNumber = "10X3"
            }
        },

        //1D - ProductCode
        {
            "+A123BJC5D6E71G",
            new HibcBarcode(false)
            {
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("BJC5D6E7"),
                LabelerIdentificationCode = "A123",
                UnitOfMeasure = 1
            }
        },

        //2D - MultipleSegments
        {
            "+A99912345/$$52025010X3/16D20111212/S77DEFG458",
            new HibcBarcode()
            {
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("1234"),
                LabelerIdentificationCode = "A999",
                UnitOfMeasure = 5,
                SerialNumber = "77DEFG45",
                BatchNumber = "10X3",
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 09, 06), "20250", "yyJJJ"),
                ProductionDate = new TestBarcodeDateTime(new DateTime(2011, 12, 12), "20111212", "yyyyMMdd")
            }
        },

        //2D - Expiration format 1
        {
            "+J123AQ3451/$$1125BC34567/S4012L",
            new HibcBarcode()
            {
                LabelerIdentificationCode = "J123",
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("AQ345"),
                UnitOfMeasure = 1,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2025, 11, 01), "1125", "MMyy"),
                BatchNumber = "BC34567",
                SerialNumber = "4012"
            }
        },

        //2D - Expiration format 0
        {
            "+J123AQ3451/$$0125BC34567/S4012K",
            new HibcBarcode()
            {
                LabelerIdentificationCode = "J123",
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("AQ345"),
                UnitOfMeasure = 1,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2025, 01, 01), "0125", "MMyy"),
                BatchNumber = "BC34567",
                SerialNumber = "4012"
            }
        },

        //2D - Online Example #2
        {
            "+J123AQ3451/$$3231231BC34567/S4012R",
            new HibcBarcode()
            {
                LabelerIdentificationCode = "J123",
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("AQ345"),
                UnitOfMeasure = 1,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2023, 12, 31), "231231", "yyMMdd"),
                BatchNumber = "BC34567",
                SerialNumber = "4012"
            }
        },

        //2D - Example with YYYYMMDD format
        {
            "+E308T800206X0/$$7230513/14D20260413F",
            new HibcBarcode()
            {
                LabelerIdentificationCode = "E308",
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("T800206X"),
                UnitOfMeasure = 0,
                ExpirationDate = new TestBarcodeDateTime(new DateTime(2026, 04, 13), "20260413", "yyyyMMdd"),
                BatchNumber = "230513",
            }
        },
        
        //2D - example of quantity without batch number
        {
            "+EHWD3551419/$/Q100X",
            new HibcBarcode()
            {
                LabelerIdentificationCode = "EHWD",
                ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("355141"),
                UnitOfMeasure = 9,
                Quantity = 100
            }
        },
    };

    [Theory]
    [MemberData(nameof(InValidHibcBarcodes))]
    public void InvalidBarcodeStringThrowsException(string barcode, string expectedMessage)
    {
        //Arrange & Act
        var parsed = HibcBarcodeParserBuilder.TryParse(barcode, null, out var result);
        Action parseAction = () => HibcBarcodeParserBuilder.Parse(barcode, null);

        //Assert
        parsed.Should().BeFalse();
        parseAction.Should()
            .Throw<HIBCParseException>()
            .WithMessage(expectedMessage);
        result.Should().BeNull();
    }

    public static TheoryData<string, string> InValidHibcBarcodes() => new()
    {
        //Bogus Data
        {
            "+A99912345/$$520´01510X^0/16D20111#2é12/S77DEFG45/",
            $"Failed to parse HIBC Barcode :{Environment.NewLine}Invalid HIBC Character detected."
        },

        //2D - Invalid CheckCharacter
        {
            "+A99912345/$$52001510X3/16D20111212/S77DEFG45/",
            $"Failed to parse HIBC Barcode :{Environment.NewLine}Check Character did not match: expected '7' but got '/'."
        },

        //1D - Invalid Primary CheckCharacter
        {
            "+A123BJC5D6E71++$$52001510X3+D",
            $"Failed to parse HIBC Barcode :{Environment.NewLine}Check Character did not match: expected 'G' but got '+'."
        },

        //1D - Invalid Secondary LinkCharacter
        {
            "+A123BJC5D6E71G+$$52001510X3+ ",
            $"Failed to parse HIBC Barcode :{Environment.NewLine}Link Character did not match: expected 'G' but got '+'."
        },
        
    };
}
