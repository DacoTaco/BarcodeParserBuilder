using BarcodeParserBuilder.Barcodes.GS1;
using BarcodeParserBuilder.Exceptions.GS1;
using BarcodeParserBuilder.Infrastructure;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.GS1;

public class GS1DateParserBuilderTestFixture
{
    public static TheoryData<BarcodeDateTime?, string, string?> ValidDateCases() => new()
    {
        { null, "yyMMdd", null },
        { null, "yyMMdd", "" },
        {
            new TestBarcodeDateTime(new DateTime(2099, 12, 31), "991231", "yyMMdd"),
            "yyMMdd",
            "991231"
        },
        {
            new TestBarcodeDateTime(new DateTime(1920, 2, 29), "19200229", "yyyyMMdd"),
            "yyyyMMdd",
            "19200229"
        },
        {
            new TestBarcodeDateTime(new DateTime(2001, 2, 19, 18, 40, 0), "0102191840", "yyMMddHHmm"),
            "yyMMddHHmm",
            "0102191840"
        },
        {
            new TestBarcodeDateTime(new DateTime(2025, 1, 10), "250110", "yyMMdd"),
            "yyMMdd/HHmm",
            "250110"
        },
        {
            new TestBarcodeDateTime(new DateTime(2025, 6, 15, 14, 30, 0), "2506151430", "yyMMddHHmm"),
            "yyMMdd/HHmm",
            "2506151430"
        },
        {
            new TestBarcodeDateTime(new DateTime(2025, 6, 16, 0, 0, 0), "250616", "yyMMdd"),
            "yyMMdd/HHmm",
            "250616"
        },
    };

    [Theory]
    [MemberData(nameof(ValidDateCases))]
    public void CanParseCorrectly(BarcodeDateTime? expectedOutput, string format, string? value)
    {
        //Arrange
        var fieldParserBuilder = new GS1DateParserBuilder() { FieldFormat = format };

        //Act
        var result = (BarcodeDateTime?)fieldParserBuilder.Parse(value, null, null);

        //Assert
        if (expectedOutput is null)
        {
            result.Should().BeNull();
        }
        else
        {
            result.Should().NotBeNull();
            result!.DateTime.Should().Be(expectedOutput.DateTime);
            result.StringValue.Should().Be(expectedOutput.StringValue);
        }
    }

    [Theory]
    [MemberData(nameof(ValidDateCases))]
    public void CanBuildCorrectly(BarcodeDateTime? value, string format, string? expectedOutput)
    {
        //Arrange
        var fieldParserBuilder = new GS1DateParserBuilder() { FieldFormat = format };

        //Act
        var result = fieldParserBuilder.Build(value);

        //Assert
        result.Should().Be(string.IsNullOrEmpty(expectedOutput) ? null : expectedOutput);
    }

    public static TheoryData<string, string?, string> InvalidValueCases() => new()
    {
        { "991231", null, "FieldFormat must be set for GS1 date fields." },
        { "ABCDEF", "yyMMdd", "Invalid GS1 Date value 'ABCDEF'." },
        { "9912", "yyMMdd", "Invalid GS1 Date value '9912' for format 'yyMMdd'." },
        { "99123100", "yyMMdd", "Invalid GS1 Date value '99123100' for format 'yyMMdd'." },
        { "250110/1430", "yyMMdd/HHmm", "Invalid GS1 Date value '250110/1430'." },
    };

    [Theory]
    [MemberData(nameof(InvalidValueCases))]
    public void InvalidValueThrowsException(string value, string? format, string expectedMessage)
    {
        //Arrange
        var fieldParserBuilder = new GS1DateParserBuilder() { FieldFormat = format };

        //Act
        Action parseAction = () => _ = fieldParserBuilder.Parse(value, null, null);

        //Assert
        parseAction.Should()
            .Throw<GS1ValidateException>()
            .WithMessage(expectedMessage);
    }

    [Theory]
    [MemberData(nameof(InvalidValueCases))]
    public void InvalidObjectValueThrowsException(string value, string? format, string expectedMessage)
    {
        //Arrange
        var fieldParserBuilder = new GS1DateParserBuilder() { FieldFormat = format };
        var invalidObject = new TestBarcodeDateTime(new DateTime(2099, 12, 31), value, format!);

        //Act
        Action buildAction = () => _ = fieldParserBuilder.Build(invalidObject);

        //Assert
        buildAction.Should()
            .Throw<GS1ValidateException>()
            .WithMessage(expectedMessage);
    }
}
