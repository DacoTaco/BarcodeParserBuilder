using BarcodeParserBuilder.Barcodes.CODE39;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.CODE39
{
    public class Code39StringParserBuilderTestFixture
    {
        [Theory] // Positive case - the validation of code39 string is explicit, listing all possible characters in the regular expression 
        [InlineData("A")] // code length 1 should be ok
        [InlineData("BB")] 
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVXYZ0123456789 -.$/+%")] // all allowed characters of code39 regular 
        public void CanValidateCorrectCode39String(string barcode)
        {
            Action parseAction = () => Code39StringParserBuilder.ValidateCode39String(barcode);
            //Assert
            parseAction.Should().NotThrow();
            Code39StringParserBuilder.ValidateCode39String(barcode).Should().BeTrue();
        }

        [Theory] // Negative case - anything outside the explicit listing of valid characters should invalidate the code
        [InlineData("Abcdef")]
        [InlineData("ABCD*!")]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVXYZ0123456789 -.$/+%!")]
        public void CanValidateInCorrectCode39String(string barcode)
        {
            Action parseAction = () => Code39StringParserBuilder.ValidateCode39String(barcode);
            //Assert
            parseAction.Should().NotThrow();
            Code39StringParserBuilder.ValidateCode39String(barcode).Should().BeFalse();
        }

        
        [Theory] // Positive case - code39 full ascii set is a range of characters of 0x0-0x7F and the regular expression is explicit so we leave the work to the regex
        [InlineData("A\u0000\u0001\u007f")] // character from the beginning of the range, second and last
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVXYZ0123456789 -.$/+%")] // subset, code 39 base
        public void CanValidateCorrectCode39FullASCIIString(string barcode)
        {
            Action parseAction = () => Code39StringParserBuilder.ValidateFullASCII(barcode);
            //Assert
            parseAction.Should().NotThrow();
            Code39StringParserBuilder.ValidateFullASCII(barcode).Should().BeTrue();
        }

        [Theory] // Negative case - anything outside the range 0x0-0x7F 
        [InlineData("A\u0000\u0001\u007f\u0080")] // corner case, first letter outside the range
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVXYZÄÖÜ")] // some human readable out-range characters
        public void CanValidateInCorrectCode39FullASCIIString(string barcode)
        {
            Action parseAction = () => Code39StringParserBuilder.ValidateFullASCII(barcode);
            //Assert
            parseAction.Should().NotThrow();
            Code39StringParserBuilder.ValidateFullASCII(barcode).Should().BeFalse();
        }

        [Fact] // there is no clear agreement what is the max length of the code39 content, it is limited by the scanner ability to read. But seems most vendors have limit on 55
        public void CanValidateCode39StringLength()
        {
            Code39StringParserBuilder.ValidateCode39ContentLength("AAAAAAAAAABBBBBBBBBBCCCCCCCCCCAAAAAAAAAABBBBBBBBBB01234").Should().BeTrue();
            Code39StringParserBuilder.ValidateCode39ContentLength("AAAAAAAAAABBBBBBBBBBCCCCCCCCCCAAAAAAAAAABBBBBBBBBB012345").Should().BeFalse();
            Code39StringParserBuilder.ValidateCode39ContentLength("A").Should().BeFalse();
            Code39StringParserBuilder.ValidateCode39ContentLength(String.Empty).Should().BeFalse();
            Action validateAction = () => Code39StringParserBuilder.ValidateCode39ContentLength(null);
            validateAction.Should().Throw<ArgumentNullException>();
        }


    }
}
