using BarcodeParserBuilder.Barcodes.GS1;
using BarcodeParserBuilder.Exceptions;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.GS1;

public class GS1Int64ParserBuilderTestFixture
{
    public static TheoryData<Int64?, string?> ValidIntCases()
    {
        var data = new TheoryData<Int64?, string?>();
        foreach (var row in GS1IntParserBuilderTestFixture.ValidIntCases())
        {
            var value = row[0] is int intValue ? (Int64?)intValue : null;
            data.Add(value, (string?)row[1]);
        }

        data.Add(9223372036854775807, "9223372036854775807");
        return data;
    }

    [Theory]
    [MemberData(nameof(ValidIntCases))]
    public void NullableFieldParserBuilderBuildsStringCorrectly(Int64? value, string? expectedOutput)
    {
        //Arrange
        var fieldParserBuilder = new GS1Int64ParserBuilder();

        //Act
        var result = fieldParserBuilder.Build(value);

        //Assert
        result.Should().Be(expectedOutput);
    }

    [Theory]
    [MemberData(nameof(ValidIntCases))]
    [InlineData(null, "")]
    public void NullableFieldParserBuilderParsesStringCorrectly(Int64? expectedOutput, string? value)
    {
        //Arrange
        var fieldParserBuilder = new GS1Int64ParserBuilder();
        var result = (Int64?)null;

        //Act
        Action parseAction = () => result = (Int64?)fieldParserBuilder.Parse(value, 1, 20);

        //Assert
        parseAction.Should().NotThrow();
        if (!expectedOutput.HasValue)
            result.HasValue.Should().BeFalse();
        else
            result.Should().Be(expectedOutput);
    }

    [Theory]
    [InlineData("       ", "Invalid GS1 int64 value '       '.")]
    [InlineData("abcdefg", "Invalid GS1 int64 value 'abcdefg'.")]
    public void InvalidStringValuesThrowException(string value, string expectedMessage)
    {
        //Arrange
        var fieldParserBuilder = new GS1Int64ParserBuilder();

        //Act
        Action parseAction = () => _ = fieldParserBuilder.Parse(value, 1, 20);

        //Assert
        parseAction.Should()
            .Throw<ValidateException>()
            .WithMessage(expectedMessage);
    }
}
