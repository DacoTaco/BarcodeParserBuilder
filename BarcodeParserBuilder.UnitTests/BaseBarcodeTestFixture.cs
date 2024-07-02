using BarcodeParserBuilder.Barcodes;
using BarcodeParserBuilder.Exceptions;
using BarcodeParserBuilder.Infrastructure;
using BarcodeParserBuilder.Infrastructure.ProductCodes;
using FluentAssertions;

namespace BarcodeParserBuilder.UnitTests;

public class BaseBarcodeTestFixture
{
    public static void CompareBarcodeObjects(Barcode? expectedResult, Barcode? parsedResult, BarcodeType? expectedBarcodeType = null)
    {
        if (expectedResult == null)
        {
            parsedResult.Should().BeNull();
            return;
        }

        parsedResult.Should().NotBeNull();
        parsedResult!.BarcodeType.Should().Be(expectedBarcodeType ?? expectedResult.BarcodeType);

        //Some barcode fields are unused in some types. we still need to compare those though.
        foreach (var propertyInfo in expectedResult.GetType().GetProperties())
        {
            if (propertyInfo.Name == nameof(Barcode.Fields) || propertyInfo.Name == nameof(Barcode.BarcodeType))
                continue;

            switch (GetPossibleUnusedField(() => propertyInfo.GetValue(expectedResult)))
            {
                case ProductCode expectedProductCode:
                    var parsedProductCode = parsedResult.ProductCode;
                    if (expectedProductCode == null)
                    {
                        parsedProductCode.Should().BeNull();
                    }
                    else
                    {
                        parsedProductCode.Should().NotBeNull();
                        parsedProductCode.Should().BeOfType(expectedProductCode.GetType());

                        foreach (var property in expectedProductCode.GetType().GetProperties())
                        {
                            var actualValue = property.GetValue(parsedProductCode, null);
                            var expectedValue = property.GetValue(expectedProductCode, null);

                            if (expectedValue == null)
                                actualValue.Should().BeNull($"'{property.Name}' should be null");
                            else
                                actualValue.Should().NotBeNull($"'{property.Name}' should be equal to {expectedValue}");

                            actualValue.Should().Be(expectedValue, $"'{property.Name}' should be equal");
                        }
                    }

                    break;
                case BarcodeDateTime _:
                    var datetimeProperties = typeof(BarcodeDateTime).GetProperties();
                    foreach (var property in datetimeProperties)
                    {
                        ComparePossibleUnusedFields(
                        () =>
                            {
                                var v = propertyInfo.GetValue(expectedResult);
                                return v == null ? null : property.GetValue(v);
                            },
                        () =>
                            {
                                var v = propertyInfo.GetValue(parsedResult);
                                return v == null ? null : property.GetValue(v);
                            },
                        propertyInfo.Name);
                    }
                    break;
                default:
                    ComparePossibleUnusedFields(() => propertyInfo.GetValue(expectedResult), () => propertyInfo.GetValue(parsedResult), propertyInfo.Name);
                    break;
            }
        }
    }

    private static object? GetPossibleUnusedField(Func<object?> getter)
    {
        try
        {
            return getter();
        }
        catch (Exception e)
        {
            if (e is not UnusedFieldException && e.InnerException is not UnusedFieldException)
                throw;

            return null;
        }
    }

    private static void ComparePossibleUnusedFields(Func<object?> expectedGetter, Func<object?> actualGetter, string propertyName)
    {
        var because = "";
        if (!string.IsNullOrWhiteSpace(propertyName))
            because = $"{propertyName} should be equal";

        var expectedValue = GetPossibleUnusedField(expectedGetter);
        var actualValue = GetPossibleUnusedField(actualGetter);

        if (expectedValue == null)
            actualValue.Should().BeNull($"'{propertyName}' should be null");
        else
            actualValue.Should().NotBeNull($"'{propertyName}' should not be null");

        actualValue.Should().Be(expectedValue, because);
    }
}
