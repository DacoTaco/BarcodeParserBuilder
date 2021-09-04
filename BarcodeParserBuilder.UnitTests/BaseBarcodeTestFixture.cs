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

            if(expectedResult.ProductCode == null)
            {
                parsedResult.ProductCode.Should().BeNull();
            }
            else
            {
                parsedResult.ProductCode.Should().NotBeNull();
                parsedResult.ProductCode.Type.Should().Be(expectedResult.ProductCode.Type);
                parsedResult.ProductCode.Code.Should().Be(expectedResult.ProductCode.Code);
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
