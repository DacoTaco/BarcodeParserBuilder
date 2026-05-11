using System.Reflection;
using BarcodeParserBuilder.Abstraction;
using BarcodeParserBuilder.Barcodes.EAN;
using BarcodeParserBuilder.Barcodes.GS1;
using BarcodeParserBuilder.Barcodes.HIBC;
using BarcodeParserBuilder.Barcodes.MSI;
using BarcodeParserBuilder.Barcodes.PPN;
using BarcodeParserBuilder.Infrastructure;
using BarcodeParserBuilder.Infrastructure.ProductCodes;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Infrastructure;

public class FieldParserBuilderFactoryTestFixture
{
    [Theory]
    [InlineData(BarcodeType.GS1, typeof(string), typeof(GS1StringParserBuilder), null)] //GS1 stringParserBuilder
    [InlineData(BarcodeType.GS1, typeof(ProductCode), typeof(GS1ProductCodeParserBuilder), null)] //GS1 ProductCodeParserBuilder
    [InlineData(BarcodeType.GS1, typeof(BarcodeDateTime), typeof(GS1DateParserBuilder), null)] //GS1 BarcodeDateTimeParserBuilder
    [InlineData(BarcodeType.GS1, typeof(BarcodeDateTime), typeof(GS1DateParserBuilder), "yyyyMMdd")] //GS1 BarcodeDateTimeParserBuilder
    [InlineData(BarcodeType.GS1, typeof(double?), typeof(GS1DoubleParserBuilder), null)] //GS1 GS1NullableDoubleParserBuilder
    [InlineData(BarcodeType.EAN, typeof(ProductCode), typeof(EanProductCodeParserBuilder), null)] //EAN ProductCodeParserBuilder
    [InlineData(BarcodeType.PPN, typeof(string), typeof(PpnStringParserBuilder), null)] //PPN stringParserBuilder
    [InlineData(BarcodeType.PPN, typeof(ProductCode), typeof(PpnProductCodeParserBuilder), null)] //PPN ProductCodeParserBuilder
    [InlineData(BarcodeType.PPN, typeof(BarcodeDateTime), typeof(PpnDateParserBuilder), null)] //PPN BarcodeDateTimeParserBuilder
    [InlineData(BarcodeType.MSI, typeof(ProductCode), typeof(MsiProductCodeParserBuilder), null)] //MSI ProductCodeParserBuilder
    [InlineData(BarcodeType.HIBC, typeof(ProductCode), typeof(HibcProductCodeParserBuilder), null)] //HIBC ProductCodeParserBuilder
    [InlineData(BarcodeType.HIBC, typeof(BarcodeDateTime), typeof(HibcDateParserBuilder), null)] //HIBC HibcDateParserBuilder
    [InlineData(BarcodeType.HIBC, typeof(string), typeof(HibcStringParserBuilder), null)] //HIBC HibcStringParserBuilder
    [InlineData(BarcodeType.HIBC, typeof(int?), typeof(HibcIntegerParserBuilder), null)] //HIBC HibcIntegerParserBuilder
    public void FactoryGeneratesCorrectFieldParserBuilderObject(BarcodeType barcodeType, Type objectType, Type expectedParserBuilderType, string? format)
    {
        //Arrange & Act
        var result = FieldParserBuilderFactory.CreateFieldParserBuilder(barcodeType, objectType, format);

        //Assert
        result.Should().BeOfType(expectedParserBuilderType);
        result.FieldFormat.Should().Be(format);
    }

    [Fact]
    public void EveryBarcodeTypeHasABarcodeClass()
    {
        //Arrange
        var barcodeTypes = Enum.GetNames(typeof(BarcodeType)).ToList();
        var AssemblyTypes = Assembly
            .GetAssembly(typeof(IFieldParserBuilder))!
            .GetTypes();

        //Act & Assert
        foreach (var barcodeType in barcodeTypes.Where(b => b != nameof(BarcodeType.Unknown)))
        {
            var type = AssemblyTypes.SingleOrDefault(t => t.IsClass &&
                                                        !t.IsAbstract &&
                                                        t.Name.Equals($"{barcodeType}Barcode", StringComparison.CurrentCultureIgnoreCase));

            type.Should().NotBeNull($"'{barcodeType}Barcode' should exist");
        }
    }
}
