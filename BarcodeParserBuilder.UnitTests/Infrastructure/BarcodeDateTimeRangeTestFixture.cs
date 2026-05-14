using System;
using BarcodeParserBuilder.Infrastructure;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Infrastructure;

public class BarcodeDateTimeRangeTestFixture
{
    public static TheoryData<string, string?, string?, DateTime, string, DateTime, string> ValidGS1RangeStrings() => new()
    {
        // Range (6+6) — default yyMMdd format
        { "200101200201", null, null, new DateTime(2020, 1, 1), "200101", new DateTime(2020, 2, 1), "200201" },

        // Range (8+8) — explicit yyyyMMdd format
        { "1920022919200301", "yyyyMMdd", "yyyyMMdd", new DateTime(1920, 2, 29), "19200229", new DateTime(1920, 3, 1), "19200301" },

        // Range (10+10) — explicit yyMMddHHmm format
        { "01021918400102191840", "yyMMddHHmm", "yyMMddHHmm", new DateTime(2001, 2, 19, 18, 40, 0), "0102191840", new DateTime(2001, 2, 19, 18, 40, 0), "0102191840" },
    };

    //refactor, use barcodedatetime objects instead of all the seperate fuckers
    [Theory]
    [MemberData(nameof(ValidGS1RangeStrings))]
    public void CanParseGs1DateRanges(string value, string? startFormat, string? endFormat, DateTime expectedStart, string expectedStartString, DateTime expectedEnd, string expectedEndString)
    {
        var result = BarcodeDateTimeRange.Gs1Range(value, startFormat, endFormat);

        result.Should().NotBeNull();
        result!.Start.DateTime.Should().Be(expectedStart);
        result.Start.StringValue.Should().Be(expectedStartString);
        result.End.DateTime.Should().Be(expectedEnd);
        result.End.StringValue.Should().Be(expectedEndString);
    }

    [Theory]
    [InlineData("2001012", null, null, "Invalid datetime value '2001012' for GS1 date range formats.")]
    [InlineData("20010A", null, null, "Invalid datetime value '20010A' for GS1 date range formats.")]
    [InlineData("200101", "yyyyJJdd", null, "Invalid datetime format 'yyyyJJdd' for GS1 date(/time).")]
    public void InvalidGS1DateRangeThrowsException(string value, string? startFormat, string? endFormat, string expectedMessage)
    {
        Action parse = () => BarcodeDateTimeRange.Gs1Range(value, startFormat, endFormat);

        parse.Should()
            .Throw<ArgumentException>()
            .WithMessage(expectedMessage);
    }

    [Fact]
    public void CanBuildGS1DateRange()
    {
        var start = new DateTime(2020, 1, 1);
        var end = new DateTime(2020, 2, 1, 18, 40, 0);

        var result = BarcodeDateTimeRange.FromDateTimes(start, end);

        result.Start.DateTime.Should().Be(start);
        result.Start.StringValue.Should().Be("200101");
        result.End.DateTime.Should().Be(end);
        result.End.StringValue.Should().Be("2002011840");
    }
}
