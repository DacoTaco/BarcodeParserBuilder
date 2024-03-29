﻿using BarcodeParserBuilder.Aim;
using BarcodeParserBuilder.Barcodes.MSI;
using BarcodeParserBuilder.Exceptions.MSI;
using BarcodeParserBuilder.Infrastructure.ProductCodes;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.MSI;

public class MsiBarcodeParserBuilderTestFixture : BaseBarcodeTestFixture
{
    [Theory]
    [MemberData(nameof(ValidMsiBarcodes))]
    public void CanParseBarcodeString(string? barcode, MsiBarcode? expectedBarcode)
    {
        //Arrange & Act
        Action buildAction = () => MsiBarcodeParserBuilder.Parse(barcode, expectedBarcode?.ReaderInformation);
        var parsed = MsiBarcodeParserBuilder.TryParse(barcode, expectedBarcode?.ReaderInformation, out var result);

        //Assert
        parsed.Should().BeTrue("barcode should be parsable");
        buildAction.Should().NotThrow();
        CompareBarcodeObjects(expectedBarcode, result);
    }

    [Theory]
    [MemberData(nameof(ValidMsiBarcodes))]
    public void CanBuildBarcodeString(string? expectedString, MsiBarcode? barcode)
    {
        //Arrange & Act
        string? result = null;
        Action buildAction = () => result = MsiBarcodeParserBuilder.Build(barcode);

        //Assert
        buildAction.Should().NotThrow();
        result.Should().Be(expectedString);
    }

    public static TheoryData<string?, MsiBarcode?> ValidMsiBarcodes() => new()
    {
        //Empty Barcode/Object
        {
            null,
            null
        },

        //APB - Motilium
        {
            $"1446921",
            new MsiBarcode()
            {
                ProductCode = TestProductCode.CreateProductCode<MsiProductCode>("1446921"),
            }
        },

        //APB - Lexatif Doux
        {
            $"3339355",
            new MsiBarcode()
            {
                ProductCode = TestProductCode.CreateProductCode<MsiProductCode>("3339355"),
            }
        },

        //MSI - Mod11
        {
            $"12345674",
            new MsiBarcode()
            {
                ProductCode = TestProductCode.CreateProductCode<MsiProductCode>("12345674"),
            }
        },

        //MSI - Random Code #1
        {
            $"80523",
            new MsiBarcode()
            {
                ProductCode = TestProductCode.CreateProductCode<MsiProductCode>("80523"),
            }
        },

        //MSI - Random Code #2 (mod1010)
        {
            $"123456741",
            new MsiBarcode()
            {
                ProductCode = TestProductCode.CreateProductCode<MsiProductCode>("123456741"),
            }
        },

        //APB - Dafalgan 500mg
        {
            $"]M033915059",
            new MsiBarcode(AimSymbologyIdentifier.ParseString("]M0"))
            {
                ProductCode = TestProductCode.CreateProductCode<MsiProductCode>("33915059"),
            }
        },
    };

    [Theory]
    [MemberData(nameof(InvalidMsiBarcodes))]
    public void InvalidBarcodeStringThrowsException(string barcode, string expectedMessage)
    {
        //Arrange & Act
        Action parseAction = () => MsiBarcodeParserBuilder.Parse(barcode, null);
        var parsed = MsiBarcodeParserBuilder.TryParse(barcode, null, out var result);

        //Assert
        parsed.Should().BeFalse("barcode should be not parsable");
        parseAction.Should()
            .Throw<MsiParseException>()
            .WithMessage(expectedMessage);
        result.Should().BeNull();
    }

    public static TheoryData<string, string> InvalidMsiBarcodes() => new()
    {
        //ProductCode Too Short
        {
            $"91",
            $"Failed to parse MSI Barcode :{Environment.NewLine}Invalid string value '91' : Too small (2/3)."
        },

        //Invalid CheckDigit
        {
            $"27066028",
            $"Failed to parse MSI Barcode :{Environment.NewLine}Invalid MSI CheckDigit '8'."
        },

        //Bogus Data
        {
            $"+$$4BOGUS",
            $"Failed to parse MSI Barcode :{Environment.NewLine}Invalid MSI value '+$$4BOGUS'."
        },
    };
}
