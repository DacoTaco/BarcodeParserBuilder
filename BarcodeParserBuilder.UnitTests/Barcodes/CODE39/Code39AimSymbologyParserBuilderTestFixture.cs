using BarcodeParserBuilder.Aim;
using BarcodeParserBuilder.Barcodes.CODE39;
using BarcodeParserBuilder.Exceptions.CODE39;
using Bogus;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.CODE39
{
    public class Code39AimSymbologyParserBuilderTestFixture
    {
        [Theory]
        [InlineData(null)]
        [InlineData("A0")]
        [InlineData("A1")]
        [InlineData("A2")]
        [InlineData("A3")]
        [InlineData("A4")]
        [InlineData("A5")]
        [InlineData("A7")]
        public void FieldParserBuilderAcceptsValidValues(string identifier)
        {
            //Arrange
            var fieldParserBuilder = new Code39AimSymbologyParserBuilder();
            var expectedResult = identifier == null ? null : new Code39SymbologyIdentifier(identifier);
            AimSymbologyIdentifier result = null;

            //Act
            Action parseAction = () => result = (AimSymbologyIdentifier)fieldParserBuilder.Parse(identifier, null, null);

            //Assert
            parseAction.Should().NotThrow();
            if (identifier != null)
                result.Should().BeOfType<Code39SymbologyIdentifier>();
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void FieldParserBuilderRejectsInvalidStringCharacters()
        {
            //Arrange
            var fieldParserBuilder = new Code39AimSymbologyParserBuilder();
            var faker = new Faker();
            var characters = faker.Random.Chars(count: 3);
            for (int i = 0; i < 3; i++)
            {
                var character = characters[i];
                if (character == 'A' || (char.IsDigit(character) && (character <= '5' || character == '7')))
                    characters[i] = faker.Random.Char();
            }
            var rejectedString = new string(characters);

            //Act
            Action parseAction = () => fieldParserBuilder.Parse(rejectedString, null, null);

            //Assert
            parseAction.Should()
                .Throw<Code39ValidateException>()
                .WithMessage($"Invalid Code39 symbology : '{rejectedString}'.");
        }
    }
}
