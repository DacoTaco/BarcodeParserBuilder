using System;
using BarcodeParserBuilder.Barcodes.PPN;
using BarcodeParserBuilder.Exceptions.PPN;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.PPN
{
    public class PpnStringParserBuilderTestFixture
    {
        [Fact]
        public void FieldParserBuilderAcceptsStringCharacters()
        {
            //Arrange
            var fieldParserBuilder = new PpnStringParserBuilder();
            var acceptedCharacters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
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
            var fieldParserBuilder = new PpnStringParserBuilder();
            var rejectedString = $"!\"$012^3456789ABCDEF{(char)0x01}KL#MTU Vh{Environment.NewLine}i";

            //Act
            Action parseAction = () => fieldParserBuilder.Parse(rejectedString, null, null);

            //Assert
            parseAction.Should()
                .Throw<PPNValidateException>()
                .WithMessage($"Invalid PPN string value '{rejectedString}'.");
        }
    }
}
