using System.Reflection;
using BarcodeParserBuilder.Abstraction;
using BarcodeParserBuilder.Barcodes;
using BarcodeParserBuilder.Barcodes.EAN;
using BarcodeParserBuilder.Barcodes.GS1;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes
{
    public class BarcodeTestFixture
    {
        [Fact]
        public void AllBarcodeParserBuilderClassesHaveTryParseMethod()
        {
            //Arrange
            var barcodeTypes = Assembly
                        .GetAssembly(typeof(Barcode))
                        .GetTypes()
                        .ToList()
                        .Where(c => c.IsClass &&
                                    !c.IsAbstract &&
                                    c.GetInterfaces().Contains(typeof(IBaseBarcodeParserBuilder)) &&
                                    (c.BaseType?.GenericTypeArguments?.Any(t => t.IsSubclassOf(typeof(Barcode))) ?? false))
                        .ToList();

            //Act & Assert
            foreach (var type in barcodeTypes)
            {
                var methodInfo = type.GetMethod(nameof(GS1BarcodeParserBuilder.TryParse));

                methodInfo.Should().NotBeNull($"barcode type '{type.Name}' should contain '{nameof(GS1BarcodeParserBuilder.TryParse)}' method");
                methodInfo.IsStatic.Should().BeTrue($"'{type.Name}.{nameof(GS1BarcodeParserBuilder.TryParse)}' should be static");
                methodInfo.IsPublic.Should().BeTrue($"'{type.Name}.{nameof(GS1BarcodeParserBuilder.TryParse)}' should be public");
                methodInfo.ReturnType.Should().Be(typeof(bool), $"'{type.Name}.{nameof(GS1BarcodeParserBuilder.TryParse)}' should return a boolean");

                var parameters = methodInfo
                    .GetParameters()
                    .ToList();

                parameters.Should().HaveCount(2, $"'{type.Name}.{nameof(GS1BarcodeParserBuilder.TryParse)}' should have 2 parameters");
                parameters.First().ParameterType.Should().Be(typeof(string), $"'{type.Name}.{nameof(GS1BarcodeParserBuilder.TryParse)}' should be TryParse(string, out barcode)");
                parameters.Last().IsOut.Should().BeTrue($"'{type.Name}.{nameof(GS1BarcodeParserBuilder.TryParse)}' should be TryParse(string, out barcode)");
            }
        }

        [Fact]
        public void AllBarcodeParserBuilderClassesHaveBuildMethod()
        {
            //Arrange
            var barcodeTypes = Assembly
                        .GetAssembly(typeof(Barcode))
                        .GetTypes()
                        .ToList()
                        .Where(c => c.IsClass &&
                                    !c.IsAbstract &&
                                    c.GetInterfaces().Contains(typeof(IBaseBarcodeParserBuilder)) &&
                                    (c.BaseType?.GenericTypeArguments?.Any(t => t.IsSubclassOf(typeof(Barcode))) ?? false))
                        .ToList();

            //Act & Assert
            foreach (var type in barcodeTypes)
            {
                var methodInfo = type.GetMethod(nameof(EanBarcodeParserBuilder.Build));

                methodInfo.Should().NotBeNull($"barcode type '{type.Name}' should contain '{nameof(GS1BarcodeParserBuilder.Build)}' method");
                methodInfo.IsStatic.Should().BeTrue($"'{type.Name}.{nameof(GS1BarcodeParserBuilder.Build)}' should be static");
                methodInfo.IsPublic.Should().BeTrue($"'{type.Name}.{nameof(GS1BarcodeParserBuilder.Build)}' should be public");
                methodInfo.ReturnType.Should().Be(typeof(string), $"'{type.Name}.{nameof(GS1BarcodeParserBuilder.Build)}' should return a string");

                var parameters = methodInfo
                    .GetParameters()
                    .ToList();

                parameters.Should().ContainSingle($"'{type.Name}.{nameof(GS1BarcodeParserBuilder.Build)}' should have a single parameter");
                type.BaseType.GenericTypeArguments.Should().Contain(parameters.First().ParameterType, $"'{type.Name}.{nameof(GS1BarcodeParserBuilder.Build)}' should only accept it's barcode type");
            }
        }

        [Fact]
        public void AllBarcodeClassesHaveParserBuilder()
        {
            //Arrange
            var barcodeTypes = Assembly
                        .GetAssembly(typeof(Barcode))
                        .GetTypes()
                        .ToList()
                        .Where(c => c.IsClass &&
                                    !c.IsAbstract &&
                                    c.IsSubclassOf(typeof(Barcode)))
                        .ToList();

            //Act & Assert
            foreach (var type in barcodeTypes)
            {
                var barcodeType = Assembly
                        .GetAssembly(typeof(Barcode))
                        .GetTypes()
                        .ToList()
                        .Where(c => c.IsClass &&
                                    !c.IsAbstract &&
                                    c.GetInterfaces().Contains(typeof(IBaseBarcodeParserBuilder)) &&
                                    (c.BaseType?.GenericTypeArguments?.Any(t => t == type) ?? false))
                        .SingleOrDefault();

                barcodeType.Should().NotBeNull($"'{type}' should have a BarcodeParserBuilder");
            }
        }
    }
}
