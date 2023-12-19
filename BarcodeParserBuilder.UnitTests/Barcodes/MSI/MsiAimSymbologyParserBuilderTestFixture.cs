using BarcodeParserBuilder.Aim;
using BarcodeParserBuilder.Barcodes.MSI;
using BarcodeParserBuilder.Exceptions.MSI;
using Bogus;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.MSI
{
    public class MsiAimSymbologyParserBuilderTestFixture
    {
        [Theory]
        [InlineData(null)]
        [InlineData("M0")]
        [InlineData("M1")]
        [InlineData("M2")]
        [InlineData("M3")]
        public void FieldParserBuilderAcceptsValidValues(string identifier)
        {
            //Arrange
            var fieldParserBuilder = new MSIAimSymbologyParserBuilder();
            var expectedResult = identifier == null ? null : new AimSymbologyIdentifier(identifier);
            AimSymbologyIdentifier result = null;

            //Act
            Action parseAction = () => result = (AimSymbologyIdentifier)fieldParserBuilder.Parse(identifier, null, null);

            //Assert
            parseAction.Should().NotThrow();
            if (identifier != null)
                result.Should().BeOfType<AimSymbologyIdentifier>();
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void FieldParserBuilderRejectsInvalidStringCharacters()
        {
            //Arrange
            var fieldParserBuilder = new MSIAimSymbologyParserBuilder();
            var faker = new Faker();
            var characters = faker.Random.Chars(count: 3);
            for (int i = 0; i < 3; i++)
            {
                var character = characters[i];
                if (character == 'M' || (char.IsDigit(character) && character <= '3'))
                    characters[i] = faker.Random.Char();
            }
            var rejectedString = new string(characters);

            //Act
            Action parseAction = () => fieldParserBuilder.Parse(rejectedString, null, null);

            //Assert
            parseAction.Should()
                .Throw<MsiValidateException>()
                .WithMessage($"Invalid MSI symbology : '{rejectedString}'.");
        }
    }
}
