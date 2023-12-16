using BarcodeParserBuilder.Barcodes.CODE128;
using BarcodeParserBuilder.Exceptions.CODE128;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.CODE128
{
    public class Code128ParserBuilderTestFixture : BaseBarcodeTestFixture
    {
        [Theory]
        [MemberData(nameof(ValidCode128ParsingBarcodes))]
        public void CanParseBarcodeString(string barcode, Code128Barcode expectedBarcode)
        {
            //Arrange & Act
            Code128BarcodeParserBuilder.TryParse(barcode, out var result).Should().BeTrue($"'{barcode}' should be parsable");
            Action parseAction = () => Code128BarcodeParserBuilder.Parse(barcode);

            //Assert
            parseAction.Should().NotThrow($"'{barcode}' should be parsable");
            result.Should().NotBeNull();
            CompareBarcodeObjects(expectedBarcode, result);
        }

        [Theory]
        [MemberData(nameof(InvalidCode128ParsingBarcodes))]
        public void CanInvalidateBarcodeString<TException>(string barcode, TException exceptionType) where TException : Exception
        {
            //Arrange & Act
            Code128BarcodeParserBuilder.TryParse(barcode, out var result).Should().BeFalse($"'{barcode}' should not be parsable");
            Action parseAction = () => Code128BarcodeParserBuilder.Parse(barcode);

            //Assert
            parseAction.Should().ThrowExactly<TException>();

        }

        public static IEnumerable<object[]> ValidCode128ParsingBarcodes()
        {
            yield return new object[]
            {
                $"]C01234567890ABSDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvxyz",
                new Code128Barcode()
                {
                    ProductCode = new Code128ProductCode("1234567890ABSDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvxyz")
                }
            };

            yield return new object[]
            {
                $"]C01234567890!#$%&/()=?",
                new Code128Barcode()
                {
                    ProductCode = new Code128ProductCode("1234567890!#$%&/()=?")
                }
            };

            yield return new object[]
            {
                $"]C01293AAS-.$/+% ",
                new Code128Barcode()
                {
                    ProductCode = new Code128ProductCode("1293AAS-.$/+% ")
                }
            };


            yield return new object[]
            {
                $"]C01293AAS-.$/+%õ ",
                new Code128Barcode()
                {
                    ProductCode = new Code128ProductCode("1293AAS-.$/+%õ ")
                }
            };
        }

        public static IEnumerable<object[]> InvalidCode128ParsingBarcodes()
        {
            yield return new object[]
            {
                $"1234444",
                new Code128ParseException("Should NOT parse as Code128 without symbology prefix, causing ambiguity")
            };
            yield return new object[]
            {
                $"]C01234444\u263A222",
                new Code128ParseException("Not extended ASCII")
            };
        }
    }
}
