using BarcodeParserBuilder.Abstraction;
using BarcodeParserBuilder.Barcodes;
using BarcodeParserBuilder.Barcodes.GS1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BarcodeParserBuilder
{
    public class BarcodeParserBuilder : IBarcodeParserBuilder
    {
        internal static IEnumerable<Type> ParserBuilders { get; set; } = null;

        public BarcodeParserBuilder()
        {
            ParserBuilders ??= CompileParserBuildersList();
        }

        private static int GetParserBuilderOrderNumber(Type type)
        {
            PropertyInfo property = type.GetProperty(nameof(GS1BarcodeParserBuilder.ParsingOrderNumber), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var value = property?.GetValue(null, null) as int?;
            if (!value.HasValue && type.BaseType != null)
                value = GetParserBuilderOrderNumber(type.BaseType);

            return value ?? 0;
        }
        internal static IEnumerable<Type> CompileParserBuildersList()
        {
            var parserBuilders = Assembly
                        .GetAssembly(typeof(IBaseBarcodeParserBuilder))
                        .GetTypes()
                        .Where(c => c.IsClass &&
                                    !c.IsAbstract &&
                                    c.GetInterfaces().Contains(typeof(IBaseBarcodeParserBuilder)) &&
                                    (c.BaseType?.GenericTypeArguments?.Any(t => t.IsClass &&
                                                                                !t.IsAbstract &&
                                                                                t.IsSubclassOf(typeof(Barcode))) ?? false))
                        .OrderBy(c => GetParserBuilderOrderNumber(c))
                        .ToList();

            var parserBuilderList = new List<Type>();

            foreach (var parserBuilder in parserBuilders)
            {
                //validate parse method
                var methodInfo = parserBuilder.GetMethod(nameof(GS1BarcodeParserBuilder.TryParse));
                if (methodInfo == null || methodInfo.IsStatic == false || methodInfo.IsPublic == false)
                    continue;

                var parameters = methodInfo
                    .GetParameters()
                    .ToList();

                //check if the method signature is how we expect it to : public static boolean TryParse(string, out Barcode)
                if (methodInfo.ReturnType != typeof(bool) ||
                    parameters.Count != 2 ||
                    parameters.First().ParameterType != typeof(string) ||
                    !parameters.Last().IsOut)
                    continue;


                //validate build method
                methodInfo = parserBuilder?.GetMethod(nameof(GS1BarcodeParserBuilder.Build));
                parameters = methodInfo?
                    .GetParameters()
                    .ToList();

                if (parserBuilder == null ||
                    methodInfo == null || methodInfo.IsStatic == false || methodInfo.IsPublic == false || methodInfo.ReturnType != typeof(string) ||
                    parameters.Count != 1 || !parameters.First().ParameterType.IsSubclassOf(typeof(Barcode)))
                    continue;


                parserBuilderList.Add(parserBuilder);
            }

            return parserBuilderList;
        }


        public bool TryParse(string barcodeString, out Barcode barcode) => TryParse(barcodeString, out barcode, out var _ );
        public bool TryParse(string barcodeString, out Barcode barcode, out string feedback)
        {
            try
            {
                barcode = null;
                feedback = null;

                if (string.IsNullOrWhiteSpace(barcodeString))
                    return true;

                foreach(var parserBuilder in ParserBuilders)
                {
                    var methodInfo = parserBuilder.GetMethod(nameof(GS1BarcodeParserBuilder.TryParse));

                    //setup parameters and execute the method
                    var tryParseParameters = new object[2];
                    tryParseParameters[0] = barcodeString;
                    tryParseParameters[1] = barcode;
                    var returnValue = methodInfo.Invoke(null, tryParseParameters);

                    if (!(returnValue is bool canParse) || !canParse)
                        continue;

                    //retrieve output parameter and return true
                    barcode = (Barcode)tryParseParameters[1];
                    return true;
                }

                throw new Exception("Failed to parse barcode : no parser could accept barcode.");
            }
            catch(Exception e)
            {
                feedback = e.Message;
                barcode = null;
                return false;
            }
        }
        public string Build(Barcode barcode)
        {
            if (barcode == null)
                return null;

            var parserBuilder = ParserBuilders.SingleOrDefault(c => c.BaseType?.GenericTypeArguments?.Any(t => t == barcode.GetType()) ?? false);
            var methodInfo = parserBuilder?.GetMethod(nameof(GS1BarcodeParserBuilder.Build));

            if (methodInfo == null)
                return null;

            var buildParameters = new object[] { barcode };
            var returnValue = methodInfo.Invoke(null, buildParameters);

            return (string)returnValue;
        }
    }
}
