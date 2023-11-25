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
    public class Code39BarcodeTestFixture
    {
       
        [Theory]
        [InlineData("A0", Code39ReaderModifier.NoFullASCIINoChecksumValue)]
        [InlineData("A1", Code39ReaderModifier.NoFullASCIIMod43ChecksumTransmittedValue)]
        [InlineData("A2", Code39ReaderModifier.NoFullASCIIMod43ChecksumStrippedValue)]
        [InlineData("A3", Code39ReaderModifier.FullASCIIOlnyModChecksumValue)]
        [InlineData("A4", Code39ReaderModifier.FullASCIINoChecksumValue)]
        [InlineData("A5", Code39ReaderModifier.FullASCIIMod43ChecksumTransmittedValue)]
        [InlineData("A7", Code39ReaderModifier.FullASCIIMod43ChecksumStrippedValue)]
        public void CanValidateCorrectCode39ReaderModifier(string readerModifier, string readerModifierValue)
        {
            
            Action parseAction = () =>
            {
                var result = Code39Barcode.ParseReaderModifier(readerModifier);
                result.Value.Should().Be(readerModifierValue);
            };
            
            parseAction.Should().NotThrow($"for {readerModifier}");
            
        }

        [Theory]
        [InlineData("")]
        [InlineData("6")]
        [InlineData("anything")]
        public void CanValidateIncorrectCode39ReaderModifier(string readerModifier)
        {
            Action parseAction = () => Code39Barcode.ParseReaderModifier(readerModifier);
            parseAction.Should().Throw<Code39ParseException>();
        }
    }
}
