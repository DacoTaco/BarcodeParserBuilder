using BarcodeParserBuilder.Barcodes;
using BarcodeParserBuilder.Exceptions;
using BarcodeParserBuilder.Infrastructure;
using FluentAssertions;
using System;

namespace BarcodeParserBuilder.UnitTests
{
    public class BaseBarcodeTestFixture
    {
        public static void CompareBarcodeObjects(Barcode expectedResult, Barcode parsedResult, BarcodeType? expectedBarcodeType = null)
        {
            if (expectedResult == null)
            {
                parsedResult.Should().BeNull();
                return;
            }

            parsedResult.Should().NotBeNull();
            parsedResult.BarcodeType.Should().Be(expectedBarcodeType ?? expectedResult.BarcodeType);

            var parsedProductCode = parsedResult.ProductCode;
            var expectedProductCode = expectedResult.ProductCode;
            if (expectedProductCode == null)
            {
                parsedProductCode.Should().BeNull();
            }
            else
            {
                parsedProductCode.Should().NotBeNull();
                parsedProductCode.Should().BeOfType(expectedProductCode.GetType());

                foreach(var property in expectedProductCode.GetType().GetProperties())
                {
                    var actualValue = property.GetValue(parsedProductCode, null);
                    var expectedValue = property.GetValue(expectedProductCode, null);

                    if (expectedValue == null)
                        actualValue.Should().BeNull();
                    else
                        actualValue.Should().NotBeNull($"{property.Name} should be equal to {expectedValue}.");

                    actualValue.Should().Be(expectedValue, $"{property.Name} should be equal.");
                }
            }

            //Some barcode fields are unused in some types. we still need to compare those though.
            ComparePossibleUnusedFields(() => expectedResult.BatchNumber, () => parsedResult.BatchNumber, "BatchNumber");
            ComparePossibleUnusedFields(() => expectedResult.SerialNumber, () => parsedResult.SerialNumber, "SerialNumber");
            ComparePossibleUnusedFields(() => expectedResult.ExpirationDate?.DateTime, () => parsedResult.ExpirationDate?.DateTime, "ExpirationDate");
            ComparePossibleUnusedFields(() => expectedResult.ExpirationDate?.StringValue, () => parsedResult.ExpirationDate?.StringValue, "ExpirationDate (StringValue)");
            ComparePossibleUnusedFields(() => expectedResult.ExpirationDate?.FormatString, () => parsedResult.ExpirationDate?.FormatString, "ExpirationDate (Format)");
        }

        private static void ComparePossibleUnusedFields(Func<object> expectedGetter, Func<object> actualGetter, string propertyName = null)
        {
            object expectedValue;
            object actualValue;

            var because = "";
            if (!string.IsNullOrWhiteSpace(propertyName))
                because = $"{propertyName} should be equal";

            try
            {
                expectedValue = expectedGetter();
            }
            catch(UnusedFieldException)
            {
                expectedValue = null;
            }

            try
            {
                actualValue = actualGetter();
            }
            catch (UnusedFieldException)
            {
                actualValue = null;
            }

            if (expectedValue == null)
                actualValue.Should().BeNull();
            else
                actualValue.Should().NotBeNull();

            actualValue.Should().Be(expectedValue, because);
        }
    }
}
