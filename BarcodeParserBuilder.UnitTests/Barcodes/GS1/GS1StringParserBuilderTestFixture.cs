using System;
using BarcodeParserBuilder.Barcodes.GS1;
using BarcodeParserBuilder.Exceptions.GS1;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.GS1
{
    public class GS1StringParserBuilderTestFixture
    {
        [Fact]
        public void FieldParserBuilderAcceptsStringCharacters()
        {
            //Arrange
            var fieldParserBuilder = new GS1StringParserBuilder();
            var acceptedCharacters = "!\"%&'()*+,-./0123456789:;<=>?ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_";
            var result = "";

            //Act
            Action parseAction = () => result = (string)fieldParserBuilder.Parse(acceptedCharacters, null, null);

            //Assert
            parseAction.Should().NotThrow();
            result.Should().Be(acceptedCharacters);
        }

        [Fact]
        public void FieldParserBuilderRejectsInvalidStringCharacters()
        {
            //Arrange
            var fieldParserBuilder = new GS1StringParserBuilder();
            var rejectedString = $"!\"$012^3456789ABCDEF{(char)0x01}KL#MTU Vh{Environment.NewLine}i";

            //Act
            Action parseAction = () => fieldParserBuilder.Parse(rejectedString, null, null);

            //Assert
            parseAction.Should()
                .Throw<GS1ValidateException>()
                .WithMessage($"Invalid GS1 string value '{rejectedString}'.");
        }
    }
}
