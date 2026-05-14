using BarcodeParserBuilder.Barcodes.GS1;
using BarcodeParserBuilder.Exceptions.GS1;
using BarcodeParserBuilder.Infrastructure;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.GS1;

public class GS1DateTimeRangeParserBuilderTestFixture
{
    private static string CompileFormat(string startFormat, string endFormat) => $"{startFormat}{BarcodeDateTimeRange.DateTimeFormatSeparator}{endFormat}";
    public static TheoryData<BarcodeDateTimeRange?, string, string?> ValidRangeCases() => new()
    {
        { null, CompileFormat("yyMMdd", "yyMMdd"), null },
        {
            new TestBarcodeDateTimeRange(new TestBarcodeDateTime(new DateTime(2020, 1, 1), "200101", "yyMMdd"), new TestBarcodeDateTime(new DateTime(2020, 2, 1), "200201", "yyMMdd")),
            CompileFormat("yyMMdd", "yyMMdd"),
            "200101200201"
        },
        {
            new TestBarcodeDateTimeRange(new TestBarcodeDateTime(new DateTime(1920, 2, 29), "19200229", "yyyyMMdd"), new TestBarcodeDateTime(new DateTime(1920, 3, 1), "19200301", "yyyyMMdd")),
            CompileFormat("yyyyMMdd", "yyyyMMdd"),
            "1920022919200301"
        },
        {
            new TestBarcodeDateTimeRange(new TestBarcodeDateTime(new DateTime(2001, 2, 19, 18, 40, 0), "0102191840", "yyMMddHHmm"), new TestBarcodeDateTime(new DateTime(2001, 2, 19, 18, 40, 0), "0102191840", "yyMMddHHmm")),
            CompileFormat("yyMMddHHmm", "yyMMddHHmm"),
            "01021918400102191840"
        },
    };

    [Theory]
    [MemberData(nameof(ValidRangeCases))]
    public void CanBuildRangeCorrectly(BarcodeDateTimeRange? value, string format, string? expectedOutput)
    {
        //Arrange
        var fieldParserBuilder = new GS1DateTimeRangeParserBuilder()
        {
            FieldFormat = format
        };

        //Act
        var result = fieldParserBuilder.Build(value);

        //Assert
        result.Should().Be(expectedOutput);
    }

    [Theory]
    [MemberData(nameof(ValidRangeCases))]
    public void CanParseRangeCorrectly(BarcodeDateTimeRange? expectedOutput, string format, string? value)
    {
        //Arrange
        var fieldParserBuilder = new GS1DateTimeRangeParserBuilder()
        {
            FieldFormat = format
        };

        //Act
        var result = (BarcodeDateTimeRange?)fieldParserBuilder.Parse(value, null, null);

        //Assert
        if (expectedOutput is null)
        {
            result.Should().BeNull();
        }
        else
        {
            result.Should().NotBeNull();
            result.StartDateTime.Should().Be(expectedOutput.StartDateTime);
            result.EndDateTime.Should().Be(expectedOutput.EndDateTime);
        }
    }

    [Theory]
    [InlineData("200101", null, "FieldFormat must be set for GS1 date range fields.")]
    [InlineData("2001012", "yyMMdd|yyMMdd", "Invalid GS1 Date Range value '2001012'.")]
    [InlineData("200101ABCDEF", "yyMMdd|yyMMdd", "Invalid GS1 Date Range value '200101ABCDEF'.")]
    [InlineData("20010120020", "yyMMdd|yyMMdd", "Invalid GS1 Date Range value '20010120020'.")]
    [InlineData("2001011920030", "yyMMdd|yyyyMMdd", "Invalid GS1 Date Range value '2001011920030'.")]
    [InlineData("200101ABCD0301", "yyMMdd|yyyyMMdd", "Invalid GS1 Date Range value '200101ABCD0301'.")]
    public void InvalidValueThrowsException(string value, string? format, string expectedMessage)
    {
        //Arrange
        var fieldParserBuilder = new GS1DateTimeRangeParserBuilder()
        {
            FieldFormat = format
        };

        //Act
        Action parseAction = () => _ = fieldParserBuilder.Parse(value, null, null);

        //Assert
        parseAction.Should()
            .Throw<GS1ValidateException>()
            .WithMessage(expectedMessage);
    }
}
