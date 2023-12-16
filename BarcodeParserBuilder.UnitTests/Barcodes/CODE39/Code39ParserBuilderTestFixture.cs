using BarcodeParserBuilder.Barcodes.CODE39;
using BarcodeParserBuilder.Exceptions.CODE39;
using FluentAssertions;
using Xunit;


namespace BarcodeParserBuilder.UnitTests.Barcodes.CODE39
{
    public class Code39ParserBuilderTestFixture : BaseBarcodeTestFixture
    {
        [Theory]
        [MemberData(nameof(ValidCode39arsingBarcodes))]
        public void CanParseBarcodeString(string barcode, Code39Barcode expectedBarcode)
        {
            //Arrange & Act
            Code39BarcodeParserBuilder.TryParse(barcode, out var result).Should().BeTrue($"'{barcode}' should be parsable");
            Action parseAction = () => Code39BarcodeParserBuilder.Parse(barcode);

            //Assert
            parseAction.Should().NotThrow($"'{barcode}' should be parsable");
            result.Should().NotBeNull();
            CompareBarcodeObjects(expectedBarcode, result);
        }

        [Theory]
        [MemberData(nameof(InvalidCode39ParsingBarcodes))]
        public void CanInvalidateBarcodeString<TException>(string barcode, TException exceptionType) where TException : Exception
        {
            //Arrange & Act
            Code39BarcodeParserBuilder.TryParse(barcode, out var result).Should().BeFalse($"'{barcode}' should not be parsable");
            Action parseAction = () => Code39BarcodeParserBuilder.Parse(barcode);

            //Assert
            parseAction.Should().ThrowExactly<TException>();
        }

        public static IEnumerable<object[]> ValidCode39arsingBarcodes()
        {
            // base symbols, no checksum
            yield return new object[]
            {
                $"]A01293AAS-.$/+% ",
                new Code39Barcode()
                {
                    ProductCode = new Code39Productcode("1293AAS-.$/+% ")
                }
            };

            // base symbols, checksum transmitted
            yield return new object[]
            {
                $"]A1CODE39W",
                new Code39Barcode()
                {
                    ProductCode = new Code39Productcode("CODE39")
                }
            };

            // base symbols, checksum stripped
            yield return new object[]
            {
                $"]A2CODE39",
                new Code39Barcode()
                {
                    ProductCode = new Code39Productcode("CODE39")
                }
            };

            // base symbols, checksum stripped
            yield return new object[]
            {
                $"]A3CODE39",
                new Code39Barcode()
                {
                    ProductCode = new Code39Productcode("CODE39")
                }
            };

            // full ascii, no checksum
            yield return new object[]
            {
                $"]A4aaabbcre",
                new Code39Barcode()
                {
                    ProductCode = new Code39Productcode("aaabbcre")
                }
            };

            // full ascii, checksum, transmitted
            yield return new object[]
            {
                $"]A5C39exO",
                new Code39Barcode()
                {
                    ProductCode = new Code39Productcode("C39ex")
                }
            };

            // full ascii, checksum, stripped
            yield return new object[]
            {
                $"]A7C39ex",
                new Code39Barcode()
                {
                    ProductCode = new Code39Productcode("C39ex")
                }
            };
        }

        public static IEnumerable<object[]> InvalidCode39ParsingBarcodes()
        {
            yield return new object[]
            {
                $"]A0aaaabbbb",
                new Code39ParseException("Modifier 0 allows only base Code39 characters")
            };
            yield return new object[]
            {
                $"]A7abcüüü",
                new Code39ParseException("characters outside of full ascii")
            };
        }
    }
}
