using BarcodeParserBuilder.Exceptions.PPN;
using BarcodeParserBuilder.Infrastructure;
using BarcodeParserBuilder.Barcodes.PPN;
using BarcodeParserBuilder.UnitTests.Barcodes.GS1;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.PPN
{
    public class PpnBarcodeParserBuilderTestFixture : BaseBarcodeTestFixture
    {
        public static char GroupSeparator => (char)0x1D;
        public static char RecordSeparator => (char)0x1E;
        public static char EndOfTransmission => (char)0x04;
        public static string Prefix => $"[)>{RecordSeparator}06{GroupSeparator}";
        public static string Suffix => $"{RecordSeparator}{EndOfTransmission}";
        public static string PPNDateFormat => "yyyyMMdd";

        [Theory]
        [MemberData(nameof(ValidPpnBarcodes))]
        [MemberData(nameof(ValidPpnParsingBarcodes))]
        public void CanParseBarcodeString(string barcode, PpnBarcode expectedBarcode)
        {
            //Arrange & Act
            PpnBarcodeParserBuilder.TryParse(barcode, out var result).Should().BeTrue($"'{barcode}' should be parsable");
            Action parseAction = () => PpnBarcodeParserBuilder.Parse(barcode);

            //Assert
            parseAction.Should().NotThrow($"'{barcode}' should be parsable");
            CompareBarcodeObjects(expectedBarcode, result);
        }

        [Theory]
        [MemberData(nameof(ValidPpnBarcodes))]
        public void CanBuildBarcodeString(string expectedBarcode, PpnBarcode barcode)
        {
            //Arrange
            string result = null;

            //Act
            Action buildAction = () => result = PpnBarcodeParserBuilder.Build(barcode);

            //Assert
            buildAction.Should().NotThrow("barcode should be buildable");
            result.Should().Be(expectedBarcode);
        }

        public static IEnumerable<object[]> ValidPpnParsingBarcodes()
        {
            //Random Order #1
            yield return new object[]
            {
                $"{Prefix}9N111234568408{GroupSeparator}SHAHASERIAL12385{GroupSeparator}1TANDSOMEBatchNumber20{Suffix}",
                new PpnBarcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<PpnProductCode>("111234568408"),
                    BatchNumber = "ANDSOMEBatchNumber20",
                    SerialNumber = "HAHASERIAL12385"
                }
            };

            //Random Order #2
            yield return new object[]
            {
                $"{Prefix}SHAHASERIAL12385{GroupSeparator}9N111234568408{GroupSeparator}1TANDSOMEBatchNumber20{Suffix}",
                new PpnBarcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<PpnProductCode>("111234568408"),
                    BatchNumber = "ANDSOMEBatchNumber20",
                    SerialNumber = "HAHASERIAL12385"
                }
            };
        }

        public static IEnumerable<object[]> ValidPpnBarcodes()
        {
            //Empty string
            yield return new object[] 
            {
                null,
                null
            };

            //Empty Barcode
            yield return new object[] 
            {
                $"{Prefix}{Suffix}",
                new PpnBarcode()
            };

            //GTIN ProductCode + Unused AI's
            var barcode = new PpnBarcode()
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
            barcode.Fields["27Q"].SetValue("9915489");
            yield return new object[]
            {
                $"{Prefix}8P03574661451947{GroupSeparator}27Q9915489{Suffix}",
                barcode
            };

            //PPN ProductCode
            yield return new object[]
            {
                $"{Prefix}9N111234568408{Suffix}",
                new PpnBarcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<PpnProductCode>("111234568408"),
                    BatchNumber = null,
                    SerialNumber = null
                }
            };

            //Serial & Batch ID
            yield return new object[]
            {
                $"{Prefix}9N111234568408{GroupSeparator}1TANDSOMEBatchNumber20{GroupSeparator}SHAHASERIAL12385{Suffix}",
                new PpnBarcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<PpnProductCode>("111234568408"),
                    BatchNumber = "ANDSOMEBatchNumber20",
                    SerialNumber = "HAHASERIAL12385"
                }
            };

            //Expiration Date
            yield return new object[]
            {
                $"{Prefix}9N111234568408{GroupSeparator}1TANDSOMEBatchNumber20{GroupSeparator}SHAHASERIAL12385{GroupSeparator}D201220{Suffix}",
                new PpnBarcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<PpnProductCode>("111234568408"),
                    BatchNumber = "ANDSOMEBatchNumber20",
                    SerialNumber = "HAHASERIAL12385",
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 12, 20), "201220", GS1BarcodeParserBuilderTestFixture.GS1DateFormat)
                }
            };

            //Production Date
            yield return new object[]
            {
                $"{Prefix}9N111234568408{GroupSeparator}1TANDSOMEBatchNumber20{GroupSeparator}SHAHASERIAL12385{GroupSeparator}D201220{GroupSeparator}16D20201231{Suffix}",
                new PpnBarcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<PpnProductCode>("111234568408"),
                    BatchNumber = "ANDSOMEBatchNumber20",
                    SerialNumber = "HAHASERIAL12385",
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2020, 12, 20), "201220", GS1BarcodeParserBuilderTestFixture.GS1DateFormat),
                    ProductionDate = new TestBarcodeDateTime(new DateTime(2020, 12, 31), "20201231", PPNDateFormat)
                }
            };
        }

        [Theory]
        [MemberData(nameof(InValidPpnBarcodes))]
        public void InvalidBarcodeStringThrowsException(string barcode, string expectedMessage)
        {
            //Arrange & Act
            var parsed = PpnBarcodeParserBuilder.TryParse(barcode, out var result);
            Action parseAction = () => PpnBarcodeParserBuilder.Parse(barcode);

            //Assert
            parsed.Should().BeFalse($"'{barcode}' should not be parsable");
            parseAction.Should()
                .Throw<PPNParseException>($"'{barcode}' should not be parsable")
                .WithMessage(expectedMessage);
            result.Should().BeNull();
        }

        public static IEnumerable<object[]> InValidPpnBarcodes()
        {
            //InvalidFormat (too small)
            yield return new object[]
            {
                $"{Prefix}05614",
                $"Failed to parse PPN Barcode :{Environment.NewLine}Invalid PPN Barcode Prefix/Suffix/Size"
            };

            //PPN + GTIN
            yield return new object[]
            {
                $"{Prefix}8P03574661451947{GroupSeparator}27Q9915489{GroupSeparator}9N111234568408{Suffix}",
                $"Failed to parse PPN Barcode :{Environment.NewLine}Barcode can not contain both a PPN and GTIN."
            };

            //InvalidFormat (Missing Suffix)
            yield return new object[]
            {
                $"{Prefix}9N111234568408", 
                $"Failed to parse PPN Barcode :{Environment.NewLine}Invalid PPN Barcode Prefix/Suffix/Size"
            };

            //InvalidFormat (Missing Prefix)
            yield return new object[]
            {
                $"[)>9N111234568408{Suffix}",
                $"Failed to parse PPN Barcode :{Environment.NewLine}Invalid PPN Barcode Prefix/Suffix/Size"
            };

            //Invalid ProductCode
            yield return new object[]
            {
                $"{Prefix}9N111234568415{Suffix}",
                $"Failed to parse PPN Barcode :{Environment.NewLine}9N : Invalid PPN CheckDigit '15', Expected '08'."
            };

            //ProductCode Too Short
            yield return new object[]
            {
                $"{Prefix}9N111{Suffix}",
                $"Failed to parse PPN Barcode :{Environment.NewLine}9N : Invalid string value '111' : Too small (3/4)."
            };

            //Missing AI
            yield return new object[]
            {
                $"{Prefix}8P03574661451947{GroupSeparator}9915489{GroupSeparator}{Suffix}",
                $"Failed to parse PPN Barcode :\r\nInvalid character detected in AI '489{GroupSeparator}'."
            };

            //Random Character
            yield return new object[]
            {
                $"{Prefix}9N111234568408{GroupSeparator}SHAHASERIAL12385{(char)0x03}{GroupSeparator}1TANDSOMEBatchNumber0{GroupSeparator}D201220{GroupSeparator}16D20201200{Suffix}",
                $"Failed to parse PPN Barcode :\r\nS : Invalid PPN string value 'HAHASERIAL12385{(char)0x03}'."
            };

            //Invalid Production Date
            yield return new object[]
            {
                $"{Prefix}9N111234568408{GroupSeparator}SHAHASERIAL12385{GroupSeparator}1TANDSOMEBatchNumber20{GroupSeparator}D201220{GroupSeparator}16D202BOGUS{Suffix}",
                "Failed to parse PPN Barcode :\r\n16D : Invalid PPN Date value '202BOGUS'."
            };

            //Invalid Expiration Date
            yield return new object[]
            {
                $"{Prefix}9N111234568408{GroupSeparator}SHAHASERIAL12385{GroupSeparator}1TANDSOMEBatchNumber20{GroupSeparator}D0BOGUS{GroupSeparator}16D20201200{Suffix}",
                "Failed to parse PPN Barcode :\r\nD : Invalid PPN Date value '0BOGUS'."
            };

            //Invalid Batch
            yield return new object[]
            {
                $"{Prefix}9N111234568408{GroupSeparator}SHAHASERIAL12385{GroupSeparator}1TA#NDSOMEBatchNumber0{GroupSeparator}D201220{GroupSeparator}16D20201200{Suffix}",
                "Failed to parse PPN Barcode :\r\n1T : Invalid PPN string value 'A#NDSOMEBatchNumber0'."
            };

            //Batch too long
            yield return new object[]
            {
                $"{Prefix}9N111234568408{GroupSeparator}SHAHASERIAL12385{GroupSeparator}1TANDSOMEBatchNumber023{GroupSeparator}D201220{GroupSeparator}16D20201200{Suffix}",
                "Failed to parse PPN Barcode :\r\n1T : Invalid value Length 21. Expected Max 20 Bytes."
            };

            //Invalid SerialNumber
            yield return new object[]
            {
                $"{Prefix}9N111234568408{GroupSeparator}SH]AHASERIAL12385{GroupSeparator}1TANDSOMEBatchNumber0{GroupSeparator}D201220{GroupSeparator}16D20201200{Suffix}",
                "Failed to parse PPN Barcode :\r\nS : Invalid PPN string value 'H]AHASERIAL12385'."
            };

            //SerialNumber too Long
            yield return new object[]
            {
                $"{Prefix}9N111234568408{GroupSeparator}SHAHASERIAL12385975189674154165{GroupSeparator}1TANDSOMEBatchNumber0{GroupSeparator}D201220{GroupSeparator}16D20201200{Suffix}",
                "Failed to parse PPN Barcode :\r\nS : Invalid value Length 30. Expected Max 20 Bytes."
            };

            //Random Fields Contains Invalid string character
            yield return new object[]
            {
                $"{Prefix}9N111234568408{GroupSeparator}SHAHASERIAL^{Environment.NewLine}^${"{}"}\\{GroupSeparator}1TANDSOMEBatchNumber0{GroupSeparator}D201220{GroupSeparator}16D20201200{Suffix}",
                "Failed to parse PPN Barcode :\r\nS : Invalid PPN string value 'HAHASERIAL^\r\n^${}\\'."
            };
        }
    }
}
