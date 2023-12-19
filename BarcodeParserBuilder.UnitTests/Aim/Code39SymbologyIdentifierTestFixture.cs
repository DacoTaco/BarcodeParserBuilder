using BarcodeParserBuilder.Aim;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Aim
{
    public class Code39SymbologyIdentifierTestFixture
    {
        [Theory]
        [InlineData("]A0", Code39SymbologyIdentifier.NoFullASCIINoChecksumValue)]
        [InlineData("]A1", Code39SymbologyIdentifier.NoFullASCIIMod43ChecksumTransmittedValue)]
        [InlineData("]A2", Code39SymbologyIdentifier.NoFullASCIIMod43ChecksumStrippedValue)]
        [InlineData("]A3", Code39SymbologyIdentifier.FullASCIIOlnyModChecksumValue)]
        [InlineData("]A4", Code39SymbologyIdentifier.FullASCIINoChecksumValue)]
        [InlineData("]A5", Code39SymbologyIdentifier.FullASCIIMod43ChecksumTransmittedValue)]
        [InlineData("]A7", Code39SymbologyIdentifier.FullASCIIMod43ChecksumStrippedValue)]
        public void CanCreateValidCode39SymbologyIdentifier(string readerModifier, string readerModifierValue)
        {
            //Arrange & Act
            Action parseAction = () =>
            {
                var result = AimSymbologyIdentifier.ParseString<Code39SymbologyIdentifier>(readerModifier);
                result.SymbologyIdentifier.Should().Be(readerModifierValue);
            };

            //Assert
            parseAction.Should().NotThrow($"for {readerModifier}");
        }


        [Theory]
        [InlineData("")]
        [InlineData("6")]
        [InlineData("anything")]
        public void CanValidateIncorrectCode39ReaderModifier(string readerModifier)
        {
            //Arrange & Act
            Action parseAction = () =>
            {
                var result = AimSymbologyIdentifier.ParseString<Code39SymbologyIdentifier>(readerModifier);
            };

            //Assert
            parseAction.Should().Throw<ArgumentException>();
        }
    }
}
