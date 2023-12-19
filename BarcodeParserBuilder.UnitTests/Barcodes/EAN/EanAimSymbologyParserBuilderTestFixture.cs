using BarcodeParserBuilder.Aim;
using BarcodeParserBuilder.Barcodes.EAN;
using BarcodeParserBuilder.Exceptions.EAN;
using Bogus;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.EAN
{
    public class EanAimSymbologyParserBuilderTestFixture
    {
        [Theory]
        [InlineData(null)]
        [InlineData("E0")]
        [InlineData("E1")]
        [InlineData("E2")]
        [InlineData("E3")]
        [InlineData("E4")]
        public void FieldParserBuilderAcceptsValidValues(string identifier)
        {
            //Arrange
            var fieldParserBuilder = new EanAimSymbologyParserBuilder();
            var expectedResult = identifier == null ? null : new EanSymbologyIdentifier(identifier);
            AimSymbologyIdentifier result = null;

            //Act
            Action parseAction = () => result = (AimSymbologyIdentifier)fieldParserBuilder.Parse(identifier, null, null);

            //Assert
            parseAction.Should().NotThrow();
            if (identifier != null)
                result.Should().BeOfType<EanSymbologyIdentifier>();
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void FieldParserBuilderRejectsInvalidStringCharacters()
        {
            //Arrange
            var fieldParserBuilder = new EanAimSymbologyParserBuilder();
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
                .Throw<EanValidateException>()
                .WithMessage($"Invalid EAN symbology : '{rejectedString}'.");
        }
    }
}
