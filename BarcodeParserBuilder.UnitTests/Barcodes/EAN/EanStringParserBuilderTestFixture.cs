using BarcodeParserBuilder.Barcodes.EAN;
using BarcodeParserBuilder.Exceptions;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.EAN
{
    public class EanStringParserBuilderTestFixture
    {
        private readonly EanStringParserBuilder _parserBuilder;

        public EanStringParserBuilderTestFixture()
        {
            _parserBuilder = new EanStringParserBuilder();
        }

        [Fact]
        public void FieldParserBuilderAcceptsStringCharacters()
        {
            //Arrange
            var acceptedCharacters = "0123456789";
            var result = "";

            //Act
            Action parseAction = () => result = (string)_parserBuilder.Parse(acceptedCharacters, null, null);

            //Assert
            parseAction.Should().NotThrow();
            result.Should().Be(acceptedCharacters);
        }

        [Fact]
        public void FieldParserBuilderRejectsInvalidStringCharacters()
        {
            //Arrange
            var rejectedString = $"0123456789abcdefghijklmnopqrstuvwxyz";

            //Act
            Action parseAction = () => _parserBuilder.Parse(rejectedString, null, null);

            //Assert
            parseAction.Should()
                .Throw<ValidateException>()
                .WithMessage($"Failed to validate object (value rejected).");
        }
    }
}
