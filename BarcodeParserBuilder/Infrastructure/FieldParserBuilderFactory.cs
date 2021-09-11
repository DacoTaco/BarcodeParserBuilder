using System;
using System.Linq;
using System.Reflection;

namespace BarcodeParserBuilder.Infrastructure
{
    internal static class FieldParserBuilderFactory
    {
        public static IFieldParserBuilder CreateFieldParserBuilder(BarcodeType barcodeType, Type objectType)
        {
            var AssemblyTypes = Assembly
                .GetAssembly(typeof(IFieldParserBuilder))
                .GetTypes();

            var nameSpace = AssemblyTypes.SingleOrDefault(t => t.IsClass &&
                                                        !t.IsAbstract &&
                                                        t.Name.Equals($"{barcodeType}Barcode", StringComparison.OrdinalIgnoreCase))?.Namespace;
            if (string.IsNullOrWhiteSpace(nameSpace))
                throw new ArgumentException($"Failed to find Barcode class for barcode type {barcodeType}.");

            //get base object type, not its nullable version
            //Currently unused. not sure if we want nullable and not nullable to be parsed by the same parser
            /*
            objectType = Nullable.GetUnderlyingType(objectType) ?? objectType;
            var nullableObjectType = (objectType.IsValueType)
                ? typeof(Nullable<>).MakeGenericType(objectType)
                : objectType;*/

            var parserBuilderType = AssemblyTypes.SingleOrDefault(t => t.IsClass &&
                            !t.IsAbstract &&
                            t.Namespace == nameSpace &&
                            t.GetInterfaces().Contains(typeof(IFieldParserBuilder)) &&
                            (t.BaseType?.GenericTypeArguments?.Contains(objectType) ?? false));

            if (parserBuilderType == null)
                throw new ArgumentException($"Failed to find FieldParserBuilder for barcode type {barcodeType} and variable type {objectType.Name}.");

            return (IFieldParserBuilder)Activator.CreateInstance(parserBuilderType);
        }
    }
}
