using BarcodeParserBuilder.GS1;
using BarcodeParserBuilder.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BarcodeParserBuilder
{
    public static class ParserBuilder
    {
        private static readonly List<Type> _parserBuilders;

        //Library startup
        static ParserBuilder()
        {
            var parserBuilders = Assembly
                        .GetAssembly(typeof(IBarcodeParserBuilder))
                        .GetTypes()
                        .Where(c => c.IsClass &&
                                    !c.IsAbstract &&
                                    c.GetInterfaces().Contains(typeof(IBarcodeParserBuilder)) &&
                                    (c.BaseType?.GenericTypeArguments?.Any(t => t.IsClass &&
                                                                                !t.IsAbstract &&
                                                                                t.IsSubclassOf(typeof(Barcode))) ?? false))
                        .OrderBy(c => GetParserBuilderOrderNumber(c))
                        .ToList();

            _parserBuilders = new List<Type>();

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


                _parserBuilders.Add(parserBuilder);
            }
        }

        internal static int GetParserBuilderOrderNumber(Type type)
        {
            PropertyInfo property = type.GetProperty(nameof(GS1BarcodeParserBuilder.ParsingOrderNumber), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var value = property?.GetValue(null, null) as int?;
            if (!value.HasValue && type.BaseType != null)
                value = GetParserBuilderOrderNumber(type.BaseType);

            return value ?? 0;
        }

        public static bool TryParse(string barcodeString, out Barcode barcode) => TryParse(barcodeString, out barcode, out var _ );
        public static bool TryParse(string barcodeString, out Barcode barcode, out string feedback)
        {
            try
            {
                barcode = null;
                feedback = null;

                if (string.IsNullOrWhiteSpace(barcodeString))
                    return true;

                foreach(var parserBuilder in _parserBuilders)
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

        public static string Build(Barcode barcode)
        {
            if (barcode == null)
                return null;

            var parserBuilder = _parserBuilders.SingleOrDefault(c => c.BaseType?.GenericTypeArguments?.Any(t => t == barcode.GetType()) ?? false);
            var methodInfo = parserBuilder?.GetMethod(nameof(GS1BarcodeParserBuilder.Build));

            if (methodInfo == null)
                return null;

            var buildParameters = new object[] { barcode };
            var returnValue = methodInfo.Invoke(null, buildParameters);

            return (string)returnValue;
        }
    }
}
