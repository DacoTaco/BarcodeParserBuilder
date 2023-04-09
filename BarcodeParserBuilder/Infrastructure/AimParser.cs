using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BarcodeParserBuilder.Abstraction;
using BarcodeParserBuilder.Barcodes;
using BarcodeParserBuilder.Barcodes.EAN;
using BarcodeParserBuilder.Barcodes.GS1;
using BarcodeParserBuilder.Barcodes.HIBC;
using BarcodeParserBuilder.Barcodes.ISBT128;
using BarcodeParserBuilder.Barcodes.MSI;

namespace BarcodeParserBuilder.Infrastructure
{
    internal class AimParser : IAimParser
    {
        internal static IEnumerable<Type> ParserBuilders { get; set; } = null!;

        //this list is compiled using the Honeywell documentation found at https://support.honeywellaidc.com/s/article/List-of-barcode-symbology-AIM-Identifiers
        internal static Dictionary<string, Func<string, string, IEnumerable<Type>>> AimPrefixMap = new Dictionary<string, Func<string, string, IEnumerable<Type>>>()
        {
            { "A", ParseCode39Barcode },
            { "C", ParseCode128Barcode },
            { "d", ParseDataMatrix },
            { "E", ParseEanBarcode },
            { "e", ParseGS1Barcode },
            { "M", ParseMsiBarcode },
            { "J", (modifier, _) => { if(modifier != "1") throw new NotImplementedException($"GS1's J modifier '{modifier}' is not implemented."); return new [] { typeof(GS1BarcodeParserBuilder)}; } },
            { "Q", ParseQrCode },
            { "X", ParseCode39Barcode },
            //Z means no barcode
            { "Z", (_, __) => Enumerable.Empty<Type>() },
            { "z", ParseAztecBarcode }
        };

        static AimParser()
        {
            ParserBuilders ??= CompileParserBuildersList();
        }

        public IEnumerable<Type> GetParsers(string barcodeString)
        {
            if (!barcodeString.StartsWith(']') || barcodeString.Length <= 3)
                return ParserBuilders;

            var codeIdentifier = barcodeString[1].ToString();
            var modifier = barcodeString[2].ToString();
            if (!AimPrefixMap.ContainsKey(codeIdentifier))
                return ParserBuilders;

            return AimPrefixMap[codeIdentifier].Invoke(modifier, barcodeString);
        }

        public string StripPrefix(string barcodeString) => StripBarcodePrefix(barcodeString);
        internal static string StripBarcodePrefix(string barcodeString)
        {
            if (!barcodeString.StartsWith(']') || barcodeString.Length <= 3)
                return barcodeString;

            var codeIdentifier = barcodeString[1].ToString();
            if (!AimPrefixMap.ContainsKey(codeIdentifier))
                return barcodeString;

            return barcodeString[3..];
        }

        private static int GetParserBuilderOrderNumber(Type type)
        {
            if (type == null)
                return 0;

            var property = type.GetProperty(nameof(GS1BarcodeParserBuilder.ParsingOrderNumber), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var value = property?.GetValue(null, null) as int?;
            if (!value.HasValue && type.BaseType != null)
                value = GetParserBuilderOrderNumber(type.BaseType);

            return value ?? 0;
        }

        internal static IEnumerable<Type> CompileParserBuildersList()
        {
            var parserBuilders = Assembly
                        .GetAssembly(typeof(IBaseBarcodeParserBuilder))?
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
            foreach (var parserBuilder in parserBuilders ?? Enumerable.Empty<Type>())
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
                    parameters == null || parameters.Count != 1 || !parameters.First().ParameterType.IsSubclassOf(typeof(Barcode)))
                    continue;


                parserBuilderList.Add(parserBuilder);
            }

            return parserBuilderList;
        }

        internal static IEnumerable<Type> ParseCode39Barcode(string modifier, string _)
        {
            switch (modifier)
            {
                case "0":
                case "1":
                case "2":
                case "4":
                case "5":
                case "7":
                    return new[]
                    {
                        typeof(EanBarcodeParserBuilder),
                        typeof(HibcBarcodeParserBuilder),
                        typeof(MsiBarcodeParserBuilder),
                    };
                default:
                    throw new NotImplementedException($"Code39 modifier '{modifier}' is not implemented.");
            }
        }

        internal static IEnumerable<Type> ParseCode128Barcode(string modifier, string _)
        {
            switch (modifier)
            {
                case "1":
                    return new[] { typeof(GS1128BarcodeParserBuilder) };
                case "0":
                case "2":
                case "4":
                    return new[]
                    {
                        typeof(GS1128BarcodeParserBuilder),
                        typeof(ISBT128BarcodeParserBuilder),
                        typeof(HibcBarcodeParserBuilder),
                        typeof(MsiBarcodeParserBuilder),
                    };
                default:
                    throw new NotImplementedException($"Code128 modifier '{modifier}' is not implemented.");
            }
        }

        internal static IEnumerable<Type> ParseEanBarcode(string modifier, string _)
        {
            switch (modifier)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                    return new[]
                    {
                        typeof(EanBarcodeParserBuilder),
                    };
                default:
                    throw new NotImplementedException($"EAN modifier '{modifier}' is not implemented.");
            }
        }

        internal static IEnumerable<Type> ParseMsiBarcode(string modifier, string _)
        {
            switch (modifier)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                    return new[]
                    {
                        typeof(MsiBarcodeParserBuilder),
                    };
                default:
                    throw new NotImplementedException($"MSI modifier '{modifier}' is not implemented.");
            }
        }

        internal static IEnumerable<Type> ParseDataMatrix(string modifier, string _)
        {
            switch (modifier)
            {
                case "2":
                    return new[] { typeof(GS1BarcodeParserBuilder) };
                case "0":
                case "1":
                case "3":
                case "4":
                case "5":
                case "6":
                    return ParserBuilders;
                default:
                    throw new NotImplementedException($"DataMatrix modifier '{modifier}' is not implemented.");
            }
        }

        internal static IEnumerable<Type> ParseGS1Barcode(string modifier, string _)
        {
            switch (modifier)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                    return new[]
                    {
                        typeof(GS1BarcodeParserBuilder),
                    };
                default:
                    throw new NotImplementedException($"GS1 modifier '{modifier}' is not implemented.");
            }
        }

        internal static IEnumerable<Type> ParseQrCode(string modifier, string _)
        {
            switch (modifier)
            {
                case "3":
                    return new[]
                    {
                        typeof(GS1BarcodeParserBuilder),
                    };
                case "0":
                case "1":
                case "2":
                case "4":
                case "5":
                case "6":
                    return ParserBuilders;
                default:
                    throw new NotImplementedException($"QrCode modifier '{modifier}' is not implemented.");
            }
        }

        internal static IEnumerable<Type> ParseAztecBarcode(string modifier, string _)
        {
            switch (modifier)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                case "A":
                case "B":
                case "C":
                    return ParserBuilders;
                default:
                    throw new NotImplementedException($"Aztec modifier '{modifier}' is not implemented.");
            }
        }
    }
}
