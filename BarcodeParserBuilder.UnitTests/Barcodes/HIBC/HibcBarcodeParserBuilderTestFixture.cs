using BarcodeParserBuilder.Barcodes.HIBC;
using BarcodeParserBuilder.Exceptions.HIBC;
using BarcodeParserBuilder.Infrastructure;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.HIBC
{
    public class HibcBarcodeParserBuilderTestFixture : BaseBarcodeTestFixture
    {
        [Theory]
        [MemberData(nameof(ValidHibcParsingBarcodes))]
        [MemberData(nameof(ValidHibcParserBuilderBarcodes))]
        public void CanParseBarcodeString(string barcode, HibcBarcode expectedBarcode)
        {
            //Arrange & Act
            var parsed = HibcBarcodeParserBuilder.TryParse(barcode, out var result);
            Action parseAction = () => HibcBarcodeParserBuilder.Parse(barcode);

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
            string result = null;
            Action buildAction = () => result = HibcBarcodeParserBuilder.Build(barcode);

            //Assert
            buildAction.Should().NotThrow();
            result.Should().NotBeNull();
            result.Should().Be(expectedBarcode);
        }

        public static IEnumerable<object[]> ValidHibcParsingBarcodes()
        {
            //1D - Multiple Lines concat - Old Format
            yield return new object[]
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
            };

            //1D - Multiple Lines concat - Old Format - aztec prefix
            yield return new object[]
            {
                "]zB+A123BJC5D6E71G+2001510X3GG",
                new HibcBarcode(false)
                {
                    ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("BJC5D6E7"),
                    LabelerIdentificationCode = "A123",
                    UnitOfMeasure = 1,
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 01, 15), "20015", "yyJJJ"),
                    BatchNumber = "10X3",
                }
            };

            //1D - Old Format
            yield return new object[]
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
            };

            //2D - Old Format
            yield return new object[]
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
            };

            //2D - Old Format - QRCode
            yield return new object[]
            {
                "]Q0+J123AQ3451/25330BC34567/S4012X",
                new HibcBarcode()
                {
                    LabelerIdentificationCode = "J123",
                    ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("AQ345"),
                    UnitOfMeasure = 1,
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2025, 11, 26), "25330", "yyJJJ"),
                    BatchNumber = "BC34567",
                    SerialNumber = "4012"
                }
            };

            //2D - Online Example #1
            yield return new object[]
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
            };

            //2D - Online Example #1 - DataMatrix
            yield return new object[]
            {
                "]d1+A123ABCDEFGHI1234567891/$$420020216LOT123456789012345/SXYZ456789012345678/16D20130202$",
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
            };

            //2D - Quantity
            yield return new object[]
            {
                "+A99912345/$$81210X3/16D20111212/14D20200906/S77DEFG45$",
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
            };

            //2D - Quantity #2
            yield return new object[]
            {
                "+A99912345/$$90234510X3/16D20111212/14D20200906/S77DEFG458",
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
            };
        }

        public static IEnumerable<object[]> ValidHibcBuildingBarcodes()
        {
            //2D - Online Example #1
            yield return new object[]
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
            };

            //2D - Changing DateTime format
            yield return new object[]
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
            };

            //2D - Quantity
            yield return new object[]
            {
                "+A99912345/$$81210X3/$$+320090677DEFG45/16D20111212U",
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
            };

            //2D - Quantity #2
            yield return new object[]
            {
                "+A99912345/$$90234510X3/$$+320090677DEFG45/16D20111212%",
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
            };
        }

        public static IEnumerable<object[]> ValidHibcParserBuilderBarcodes()
        {
            //1D - Multiple Lines concat
            yield return new object[]
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
            };

            //1D - Multiple Lines with batch
            yield return new object[]
            {
                "+A123BJC5D6E71G+$$710X3G7",
                new HibcBarcode(false)
                {
                    ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("BJC5D6E7"),
                    LabelerIdentificationCode = "A123",
                    UnitOfMeasure = 1,
                    BatchNumber = "10X3",
                }
            };

            //1D - ExpirationDate + SerialNumber
            yield return new object[]
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
            };

            //1D - Multiple Lines with special link character
            yield return new object[]
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
            };

            //1D - ProductCode
            yield return new object[]
            {
                "+A123BJC5D6E71G",
                new HibcBarcode(false)
                {
                    ProductCode = TestProductCode.CreateProductCode<HibcProductCode>("BJC5D6E7"),
                    LabelerIdentificationCode = "A123",
                    UnitOfMeasure = 1
                }
            };

            //2D - MultipleSegments
            yield return new object[]
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
            };

            //2D - Expiration format 1
            yield return new object[]
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
            };

            //2D - Expiration format 0
            yield return new object[]
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
            };

            //2D - Online Example #2
            yield return new object[]
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
            };

            //2D - Example with YYYYMMDD format
            yield return new object[]
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
            };
        }

        [Theory]
        [MemberData(nameof(InValidHibcBarcodes))]
        public void InvalidBarcodeStringThrowsException(string barcode, string expectedMessage)
        {
            //Arrange & Act
            var parsed = HibcBarcodeParserBuilder.TryParse(barcode, out var result);
            Action parseAction = () => HibcBarcodeParserBuilder.Parse(barcode);

            //Assert
            parsed.Should().BeFalse();
            parseAction.Should()
                .Throw<HIBCParseException>()
                .WithMessage(expectedMessage);
            result.Should().BeNull();
        }

        public static IEnumerable<object[]> InValidHibcBarcodes()
        {
            //Bogus Data
            yield return new object[]
            {
                "+A99912345/$$520´01510X^0/16D20111#2é12/S77DEFG45/",
                $"Failed to parse HIBC Barcode :{Environment.NewLine}Invalid HIBC Character detected."
            };

            //2D - Invalid CheckCharacter
            yield return new object[]
            {
                "+A99912345/$$52001510X3/16D20111212/S77DEFG45/",
                $"Failed to parse HIBC Barcode :{Environment.NewLine}Check Character did not match: expected '7' but got '/'."
            };

            //1D - Invalid Primary CheckCharacter
            yield return new object[]
            {
                "+A123BJC5D6E71++$$52001510X3+D",
                $"Failed to parse HIBC Barcode :{Environment.NewLine}Check Character did not match: expected 'G' but got '+'."
            };

            //1D - Invalid Secondary LinkCharacter
            yield return new object[]
            {
                "+A123BJC5D6E71G+$$52001510X3+ ",
                $"Failed to parse HIBC Barcode :{Environment.NewLine}Link Character did not match: expected 'G' but got '+'."
            };
        }
    }
}
