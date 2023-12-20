using BarcodeParserBuilder.Aim;
using BarcodeParserBuilder.Barcodes.CODE128;
using BarcodeParserBuilder.Exceptions.CODE128;
using BarcodeParserBuilder.Infrastructure.ProductCodes;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.CODE128;

public class Code128ParserBuilderTestFixture : BaseBarcodeTestFixture
{
    [Theory]
    [MemberData(nameof(ValidCode128ParsingBarcodes))]
    public void CanParseBarcodeString(string barcode, Code128Barcode expectedBarcode)
    {
        //Arrange & Act
        Code128BarcodeParserBuilder.TryParse(barcode, expectedBarcode.ReaderInformation, out var result).Should().BeTrue($"'{barcode}' should be parsable");
        Action parseAction = () => Code128BarcodeParserBuilder.Parse(barcode, expectedBarcode.ReaderInformation);

        //Assert
        parseAction.Should().NotThrow($"'{barcode}' should be parsable");
        result.Should().NotBeNull();
        CompareBarcodeObjects(expectedBarcode, result);
    }

    [Theory]
    [MemberData(nameof(InvalidCode128ParsingBarcodes))]
    public void CanInvalidateBarcodeString<TException>(string barcode, TException exception) where TException : Exception
    {
        //Arrange
        var symbologyParser = barcode.First() == ']'
            ? AimSymbologyIdentifier.ParseString<Code128SymbologyIdentifier>(barcode)
            : null;

        //Act
        Code128BarcodeParserBuilder.TryParse(barcode, symbologyParser, out var result).Should().BeFalse($"'{barcode}' should not be parsable");
        Action parseAction = () => Code128BarcodeParserBuilder.Parse(barcode, symbologyParser);

        //Assert
        parseAction.Should().ThrowExactly<TException>().Which.Message.Should().Be(exception.Message);
    }

    public static TheoryData<string, Code128Barcode> ValidCode128ParsingBarcodes() => new()
    {
        {
            $"]C01234567890ABSDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrs",
            new Code128Barcode(new Code128SymbologyIdentifier("C0"))
            {
                ProductCode = new Code128ProductCode("1234567890ABSDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrs"),
            }
        },
        {
            $"]C01234567890!#$%&/()=?",
            new Code128Barcode(new Code128SymbologyIdentifier("C0"))
            {
                ProductCode = new Code128ProductCode("1234567890!#$%&/()=?"),
            }
        },
        {
            $"]C01293AAS-.$/+% ",
            new Code128Barcode(new Code128SymbologyIdentifier("C0"))
            {
                ProductCode = new Code128ProductCode("1293AAS-.$/+% "),
            }
        },
        {
            $"]C01293AAS-.$/+%õ ",
            new Code128Barcode(new Code128SymbologyIdentifier("C0"))
            {
                ProductCode = new Code128ProductCode("1293AAS-.$/+%õ "),
            }
        },
    };

    public static TheoryData<string, Code128ParseException> InvalidCode128ParsingBarcodes() => new()
    {
        {
            $"1234444",
            new Code128ParseException($"Failed to parse Code128 Barcode :{Environment.NewLine}Invalid Code128 Identifier")
        },
        {
            $"]C01234444\u263A222",
            new Code128ParseException($"Failed to parse Code128 Barcode :{Environment.NewLine}Invalid Code128 value in '1234444\u263A222'.")
        }
    };
}
