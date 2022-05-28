using BarcodeParserBuilder.Barcodes.EAN;
using BarcodeParserBuilder.Exceptions;
using FluentAssertions;
using System;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.EAN
{
    public class EanProductSystemParserBuilderTestFixture
    {
        private readonly EanProductSystemParserBuilder _parserBuilder;

        public EanProductSystemParserBuilderTestFixture()
        {
            _parserBuilder = new EanProductSystemParserBuilder();
        }

        [Fact]
        public void FieldParserBuilderAcceptsNumber()
        {
            //Arrange
            var acceptedNumber = 0;
            var expectedResult = EanProductSystem.Create(acceptedNumber);
            EanProductSystem result = null;

            //Act
            Action parseAction = () => result = (EanProductSystem)_parserBuilder.Parse(acceptedNumber.ToString(), null, null);

            //Assert
            parseAction.Should().NotThrow();
            result.Should().NotBeNull();
            result.Value.Should().Be(expectedResult.Value);
            result.Scheme.Should().Be(expectedResult.Scheme);
        }

        [Fact]
        public void FieldParserBuilderRejectsInvalidString()
        {
            //Arrange
            var rejectedNumber = "054";
            EanProductSystem result = null;

            //Act
            Action parseAction = () => result = (EanProductSystem)_parserBuilder.Parse(rejectedNumber, null, null);

            //Assert
            parseAction.Should()
                .Throw<ValidateException>()
                .WithMessage("Invalid EanProductSystem '054'.");
        }
    }
}
