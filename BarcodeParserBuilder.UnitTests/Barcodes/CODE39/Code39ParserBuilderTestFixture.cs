﻿using BarcodeParserBuilder.Aim;
using BarcodeParserBuilder.Barcodes.CODE39;
using BarcodeParserBuilder.Exceptions.CODE39;
using BarcodeParserBuilder.Infrastructure.ProductCodes;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.CODE39;

public class Code39ParserBuilderTestFixture : BaseBarcodeTestFixture
{
    [Theory]
    [MemberData(nameof(ValidCode39ParsingBarcodes))]
    public void CanParseBarcodeString(string barcode, Code39Barcode expectedBarcode)
    {
        //Arrange & Act
        Code39BarcodeParserBuilder.TryParse(barcode, expectedBarcode.ReaderInformation, out var result).Should().BeTrue($"'{barcode}' should be parsable");
        Action parseAction = () => Code39BarcodeParserBuilder.Parse(barcode, expectedBarcode.ReaderInformation);

        //Assert
        parseAction.Should().NotThrow($"'{barcode}' should be parsable");
        result.Should().NotBeNull();
        CompareBarcodeObjects(expectedBarcode, result!);
    }

    [Theory]
    [MemberData(nameof(InvalidCode39ParsingBarcodes))]
    public void CanInvalidateBarcodeString<TException>(string barcode, TException exception) where TException : Exception
    {
        //Arrange
        var symbologyParser = barcode.First() == ']'
            ? AimSymbologyIdentifier.ParseString<Code39SymbologyIdentifier>(barcode)
            : null;

        //Act
        Code39BarcodeParserBuilder.TryParse(barcode, symbologyParser, out var result).Should().BeFalse($"'{barcode}' should not be parsable");
        Action parseAction = () => Code39BarcodeParserBuilder.Parse(barcode, symbologyParser);

        //Assert
        parseAction.Should().ThrowExactly<TException>().Which.Message.Should().Be(exception.Message);
    }

    public static TheoryData<string, Code39Barcode> ValidCode39ParsingBarcodes() => new()
    {
        // base symbols, no checksum
        {
            $"]A01293AAS-.$/+% ",
            new Code39Barcode(new Code39SymbologyIdentifier("A0"))
            {
                ProductCode = new Code39ProductCode("1293AAS-.$/+% "),
            }
        },

        // base symbols, checksum transmitted
        {
            $"]A1CODE39W",
            new Code39Barcode(new Code39SymbologyIdentifier("A1"))
            {
                ProductCode = new Code39ProductCode("CODE39"),
            }
        },

        // base symbols, checksum stripped
        {
            $"]A2CODE39",
            new Code39Barcode(new Code39SymbologyIdentifier("A2"))
            {
                ProductCode = new Code39ProductCode("CODE39"),
            }
        },

        // base symbols, checksum stripped
        {
            $"]A3CODE39",
            new Code39Barcode(new Code39SymbologyIdentifier("A3"))
            {
                ProductCode = new Code39ProductCode("CODE39"),
            }
        },

        // full ascii, no checksum
        {
            $"]A4aaabbcre",
            new Code39Barcode(new Code39SymbologyIdentifier("A4"))
            {
                ProductCode = new Code39ProductCode("aaabbcre"),
            }
        },

        // full ascii, checksum, transmitted
        {
            $"]A5C39exO",
            new Code39Barcode(new Code39SymbologyIdentifier("A5"))
            {
                ProductCode = new Code39ProductCode("C39ex"),
            }
        },

        // full ascii, checksum, stripped
        {
            $"]A7C39ex",
            new Code39Barcode(new Code39SymbologyIdentifier("A7"))
            {
                ProductCode = new Code39ProductCode("C39ex"),
            }
        },
    };

    public static TheoryData<string, Code39ParseException> InvalidCode39ParsingBarcodes() => new()
    {
        {
            $"]A0aaaabbbb",
            new Code39ParseException($"Failed to parse Code39 Barcode :{Environment.NewLine}Code content does not match reader information")
        },
        {
            $"]A7abcüüü",
            new Code39ParseException($"Failed to parse Code39 Barcode :{Environment.NewLine}Code content does not match reader information")
        },
    };
}
