using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using BarcodeParserBuilder.Barcodes.CODE39;
using FluentAssertions;
using BarcodeParserBuilder.Exceptions.CODE39;
using BarcodeParserBuilder.Infrastructure;

namespace BarcodeParserBuilder.UnitTests.Barcodes.CODE39
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

            
            Action parseAction = () =>
            {
                var result = AimSymbologyIdentifier.FromRawReading<Code39SymbologyIdentifier>(readerModifier);
                result.SymbologyIdentifier.Should().Be(readerModifierValue);
            };

            parseAction.Should().NotThrow($"for {readerModifier}");

        }


        [Theory]
        [InlineData("")]
        [InlineData("6")]
        [InlineData("anything")]
        public void CanValidateIncorrectCode39ReaderModifier(string readerModifier)
        {
            Action parseAction = () =>
            {
                var result = AimSymbologyIdentifier.FromRawReading<Code39SymbologyIdentifier>(readerModifier);
            };

            parseAction.Should().Throw<ArgumentException>();

            
            
        }
    }
}
