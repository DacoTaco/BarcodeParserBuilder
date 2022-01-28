using BarcodeParserBuilder.Barcodes.EAN;
using BarcodeParserBuilder.Barcodes.GS1;
using BarcodeParserBuilder.Barcodes.HIBC;
using BarcodeParserBuilder.Infrastructure;
using BarcodeParserBuilder.Barcodes.MSI;
using BarcodeParserBuilder.Barcodes.PPN;
using FluentAssertions;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Infrastructure
{
    public class FieldParserBuilderFactoryTestFixture
    {
        [Theory]
        [InlineData(BarcodeType.GS1, typeof(string), typeof(GS1StringParserBuilder))] //GS1 stringParserBuilder
        [InlineData(BarcodeType.GS1, typeof(ProductCode), typeof(GS1ProductCodeParserBuilder))] //GS1 ProductCodeParserBuilder
        [InlineData(BarcodeType.GS1, typeof(BarcodeDateTime), typeof(GS1DateParserBuilder))] //GS1 BarcodeDateTimeParserBuilder
        [InlineData(BarcodeType.GS1, typeof(double?), typeof(GS1DoubleParserBuilder))] //GS1 GS1NullableDoubleParserBuilder
        [InlineData(BarcodeType.EAN, typeof(ProductCode), typeof(EanProductCodeParserBuilder))] //EAN ProductCodeParserBuilder
        [InlineData(BarcodeType.PPN, typeof(string), typeof(PpnStringParserBuilder))] //PPN stringParserBuilder
        [InlineData(BarcodeType.PPN, typeof(ProductCode), typeof(PpnProductCodeParserBuilder))] //PPN ProductCodeParserBuilder
        [InlineData(BarcodeType.PPN, typeof(BarcodeDateTime), typeof(PpnDateParserBuilder))] //PPN BarcodeDateTimeParserBuilder
        [InlineData(BarcodeType.MSI, typeof(ProductCode), typeof(MsiProductCodeParserBuilder))] //MSI ProductCodeParserBuilder
        [InlineData(BarcodeType.HIBC, typeof(ProductCode), typeof(HibcProductCodeParserBuilder))] //HIBC ProductCodeParserBuilder
        [InlineData(BarcodeType.HIBC, typeof(BarcodeDateTime), typeof(HibcDateParserBuilder))] //HIBC HibcDateParserBuilder
        [InlineData(BarcodeType.HIBC, typeof(string), typeof(HibcStringParserBuilder))] //HIBC HibcStringParserBuilder
        [InlineData(BarcodeType.HIBC, typeof(int?), typeof(HibcIntegerParserBuilder))] //HIBC HibcIntegerParserBuilder
        public void FactoryGeneratesCorrectFieldParserBuilderObject(BarcodeType barcodeType, Type objectType, Type expectedParserBuilderType)
        {
            //Arrange & Act
            var result = FieldParserBuilderFactory.CreateFieldParserBuilder(barcodeType, objectType);

            //Assert
            result.GetType().Should().Be(expectedParserBuilderType);
        }

        [Fact]
        public void EveryBarcodeTypeHasABarcodeClass()
        {
            //Arrange
            var barcodeTypes = Enum.GetNames(typeof(BarcodeType)).ToList();
            var AssemblyTypes = Assembly
                .GetAssembly(typeof(IFieldParserBuilder))
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
}
