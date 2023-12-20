using BarcodeParserBuilder.Barcodes.GS1;
using BarcodeParserBuilder.Exceptions;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.GS1;

public class GS1DoubleParserBuilderTestFixture
{
    public static TheoryData<double?, string?> ValidDoubleCases() => new()
    {
        {
            null, null
        },

        {
            0d, "0000000"
        },

        {
            5d, "0000005"
        },

        {
            0.59d, "2000059"
        },

        {
            0.59574d, "5059574"
        },

        {
            0.595742d, "6595742"
        },

        {
            595745d, "0595745"
        },

        {
            59574.5d, "1595745"
        },
    };

    [Theory]
    [MemberData(nameof(ValidDoubleCases))]
    public void NullableFieldParserBuilderBuildsStringCorrectly(double? value, string? expectedOutput)
    {
        //Arrange
        var fieldParserBuilder = new GS1DoubleParserBuilder();
        var result = "";

        //Act
        Action buildAction = () => result = fieldParserBuilder.Build(value);

        //Assert
        buildAction.Should().NotThrow();
        result.Should().Be(expectedOutput);
    }

    [Theory]
    [MemberData(nameof(ValidDoubleCases))]
    [InlineData(null, "")]
    public void NullableFieldParserBuilderParsesStringCorrectly(double? expectedOutput, string? value)
    {
        //Arrange
        var fieldParserBuilder = new GS1DoubleParserBuilder();
        var result = (double?)null;

        //Act
        Action parseAction = () => result = (double?)fieldParserBuilder.Parse(value, 7, 7);

        //Assert
        parseAction.Should().NotThrow();
        if (!expectedOutput.HasValue)
            result.HasValue.Should().BeFalse();
        else
            result.Should().Be(expectedOutput);
    }

    [Theory]
    [InlineData("       ", "Invalid GS1 double value '       '.")]
    [InlineData("abcdefg", "Invalid GS1 double value 'abcdefg'.")]
    [InlineData("02467f9", "Invalid GS1 double value '02467f9'.")]
    public void InvalidStringValuesThrowException(string value, string expectedMessage)
    {
        //Arrange
        var fieldParserBuilder = new GS1DoubleParserBuilder();

        //Act
        Action parseAction = () => _ = fieldParserBuilder.Parse(value, 7, 7);

        //Assert
        parseAction.Should()
            .Throw<ValidateException>()
            .WithMessage(expectedMessage);
    }
}
