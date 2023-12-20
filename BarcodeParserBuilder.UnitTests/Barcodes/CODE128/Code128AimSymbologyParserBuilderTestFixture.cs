using BarcodeParserBuilder.Aim;
using BarcodeParserBuilder.Barcodes.CODE128;
using BarcodeParserBuilder.Exceptions.CODE128;
using Bogus;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.CODE128;

public class Code128AimSymbologyParserBuilderTestFixture
{
    [Theory]
    [InlineData(null)]
    [InlineData("C0")]
    [InlineData("C1")]
    [InlineData("C2")]
    [InlineData("C3")]
    [InlineData("C4")]
    public void FieldParserBuilderAcceptsValidValues(string? identifier)
    {
        //Arrange
        var fieldParserBuilder = new Code128AimSymbologyParserBuilder();
        var expectedResult = identifier == null ? null : new Code128SymbologyIdentifier(identifier);
        AimSymbologyIdentifier? result = null;

        //Act
        Action parseAction = () => result = (AimSymbologyIdentifier?)fieldParserBuilder.Parse(identifier, null, null);

        //Assert
        parseAction.Should().NotThrow();
        if (identifier != null)
            result.Should().BeOfType<Code128SymbologyIdentifier>();
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void FieldParserBuilderRejectsInvalidStringCharacters()
    {
        //Arrange
        var fieldParserBuilder = new Code128AimSymbologyParserBuilder();
        var faker = new Faker();
        var characters = faker.Random.Chars(count: 3);
        for (int i = 0; i < 3; i++)
        {
            var character = characters[i];
            if (character == 'C' || (char.IsDigit(character) && character <= '4'))
                characters[i] = faker.Random.Char();
        }
        var rejectedString = new string(characters);

        //Act
        Action parseAction = () => fieldParserBuilder.Parse(rejectedString, null, null);

        //Assert
        parseAction.Should()
            .Throw<Code128ValidateException>()
            .WithMessage($"Invalid Code128 symbology : '{rejectedString}'.");
    }
}
