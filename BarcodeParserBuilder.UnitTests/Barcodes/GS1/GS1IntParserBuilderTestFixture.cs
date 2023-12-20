using BarcodeParserBuilder.Barcodes.GS1;
using BarcodeParserBuilder.Exceptions;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.GS1;

public class GS1IntParserBuilderTestFixture
{
    public static TheoryData<int?, string?> ValidIntCases() => new()
    {
        { null, null },
        { 0, "0" },
        { 5, "5" },
        { 2000059, "2000059" }
    };

    [Theory]
    [MemberData(nameof(ValidIntCases))]
    public void NullableFieldParserBuilderBuildsStringCorrectly(int? value, string? expectedOutput)
    {
        //Arrange
        var fieldParserBuilder = new GS1IntParserBuilder();
        var result = "";

        //Act
        Action buildAction = () => result = fieldParserBuilder.Build(value);

        //Assert
        buildAction.Should().NotThrow();
        result.Should().Be(expectedOutput);
    }

    [Theory]
    [MemberData(nameof(ValidIntCases))]
    [InlineData(null, "")]
    public void NullableFieldParserBuilderParsesStringCorrectly(int? expectedOutput, string? value)
    {
        //Arrange
        var fieldParserBuilder = new GS1IntParserBuilder();
        var result = (int?)null;

        //Act
        Action parseAction = () => result = (int?)fieldParserBuilder.Parse(value, 1, 8);

        //Assert
        parseAction.Should().NotThrow();
        if (!expectedOutput.HasValue)
            result.HasValue.Should().BeFalse();
        else
            result.Should().Be(expectedOutput);
    }

    [Theory]
    [InlineData("       ", "Invalid GS1 int value '       '.")]
    [InlineData("abcdefg", "Invalid GS1 int value 'abcdefg'.")]
    public void InvalidStringValuesThrowException(string value, string expectedMessage)
    {
        //Arrange
        var fieldParserBuilder = new GS1IntParserBuilder();

        //Act
        Action parseAction = () => _ = fieldParserBuilder.Parse(value, 1, 8);

        //Assert
        parseAction.Should()
            .Throw<ValidateException>()
            .WithMessage(expectedMessage);
    }
}
