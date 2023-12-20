using System.Reflection;
using BarcodeParserBuilder.Barcodes.CODE128;
using BarcodeParserBuilder.Barcodes.CODE39;
using BarcodeParserBuilder.Barcodes.EAN;
using BarcodeParserBuilder.Barcodes.GS1;
using BarcodeParserBuilder.Barcodes.HIBC;
using BarcodeParserBuilder.Barcodes.ISBT128;
using BarcodeParserBuilder.Barcodes.MSI;

namespace BarcodeParserBuilder.Aim;

internal class AimParser : IAimParser
{
    internal static IEnumerable<Type> ParserBuilders { get; set; } = null!;

    //this list is compiled using the Honeywell documentation found at https://sps-support.honeywell.com/s/article/List-of-barcode-symbology-AIM-Identifiers
    internal static Dictionary<string, AimParserProcessor> AimPrefixMap = new()
    {
        { "A", ParseCode39Barcode },
        { "C", ParseCode128Barcode },
        { "d", ParseDataMatrix },
        { "E", ParseEanBarcode },
        { "e", ParseGS1Barcode },
        { "M", ParseMsiBarcode },
        { "J", (modifier, barcode) => { if(modifier != "1") throw new NotImplementedException($"GS1's J information '{modifier}' is not implemented."); return new AimProcessorResult(AimSymbologyIdentifier.ParseString<GS1AimSymbologyIdentifier>(barcode), typeof(GS1BarcodeParserBuilder)); } },
        { "Q", ParseQrCode },
        { "X", ParseCode39Barcode },
        //Z means no barcode
        { "Z", (_, __) => new AimProcessorResult(Enumerable.Empty<Type>(), null) },
        { "z", ParseAztecBarcode }
    };

    static AimParser()
    {
        ParserBuilders ??= CompileParserBuildersList();
    }

    public AimProcessorResult GetParsers(string barcodeString)
    {
        if (!barcodeString.StartsWith("]") || barcodeString.Length <= 3)
            return new AimProcessorResult(ParserBuilders, null);

        var codeIdentifier = barcodeString[1].ToString();
        var modifier = barcodeString[2].ToString();
        if (!AimPrefixMap.ContainsKey(codeIdentifier))
            return new AimProcessorResult(ParserBuilders, AimSymbologyIdentifier.ParseString(barcodeString));

        return AimPrefixMap[codeIdentifier].Invoke(modifier, barcodeString);
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

            //check if the method signature is how we expect it to : public static boolean TryParse(string, AimSymbologyIdentifier, out Barcode)
            if (methodInfo.ReturnType != typeof(bool) ||
                parameters.Count != 3 ||
                parameters.First().ParameterType != typeof(string) ||
                parameters[1].ParameterType != typeof(AimSymbologyIdentifier) ||
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

    internal static AimProcessorResult ParseCode39Barcode(string modifier, string barcode) => modifier switch
    {
        "0" or "1" or "2" or "3" or "4" or "5" or "7" => new AimProcessorResult(AimSymbologyIdentifier.ParseString<Code39SymbologyIdentifier>(barcode), typeof(Code39BarcodeParserBuilder)),
        _ => throw new NotImplementedException($"Code39 information '{modifier}' is not implemented."),
    };

    internal static AimProcessorResult ParseCode128Barcode(string modifier, string barcode) => modifier switch
    {
        "0" => new AimProcessorResult(AimSymbologyIdentifier.ParseString<Code128SymbologyIdentifier>(barcode), typeof(Code128BarcodeParserBuilder)),
        "1" => new AimProcessorResult(AimSymbologyIdentifier.ParseString<Code128SymbologyIdentifier>(barcode), typeof(GS1128BarcodeParserBuilder)),
        "2" or "4" => new AimProcessorResult(
            AimSymbologyIdentifier.ParseString<Code128SymbologyIdentifier>(barcode),
            typeof(GS1128BarcodeParserBuilder), typeof(ISBT128BarcodeParserBuilder), typeof(HibcBarcodeParserBuilder), typeof(MsiBarcodeParserBuilder)),
        _ => throw new NotImplementedException($"Code128 information '{modifier}' is not implemented."),
    };

    internal static AimProcessorResult ParseEanBarcode(string modifier, string barcode) => modifier switch
    {
        "0" or "1" or "2" or "3" or "4" => new AimProcessorResult(new[] { typeof(EanBarcodeParserBuilder) }, AimSymbologyIdentifier.ParseString<EanSymbologyIdentifier>(barcode)),
        _ => throw new NotImplementedException($"EAN information '{modifier}' is not implemented."),
    };

    internal static AimProcessorResult ParseMsiBarcode(string modifier, string barcode) => modifier switch
    {
        "0" or "1" or "2" or "3" => new AimProcessorResult(AimSymbologyIdentifier.ParseString(barcode), typeof(MsiBarcodeParserBuilder)),
        _ => throw new NotImplementedException($"MSI information '{modifier}' is not implemented."),
    };

    internal static AimProcessorResult ParseDataMatrix(string modifier, string barcode) => modifier switch
    {
        "2" => new AimProcessorResult(new[] { typeof(GS1BarcodeParserBuilder) }, AimSymbologyIdentifier.ParseString<GS1AimSymbologyIdentifier>(barcode)),
        "0" or "1" or "3" or "4" or "5" or "6" => new AimProcessorResult(ParserBuilders, AimSymbologyIdentifier.ParseString(barcode)),
        _ => throw new NotImplementedException($"DataMatrix information '{modifier}' is not implemented."),
    };

    internal static AimProcessorResult ParseGS1Barcode(string modifier, string barcode) => modifier switch
    {
        "0" or "1" or "2" or "3" => new AimProcessorResult(new[] { typeof(GS1BarcodeParserBuilder) }, AimSymbologyIdentifier.ParseString<GS1AimSymbologyIdentifier>(barcode)),
        _ => throw new NotImplementedException($"GS1 information '{modifier}' is not implemented."),
    };

    internal static AimProcessorResult ParseQrCode(string modifier, string barcode) => modifier switch
    {
        "3" => new AimProcessorResult(AimSymbologyIdentifier.ParseString<GS1AimSymbologyIdentifier>(barcode), typeof(GS1BarcodeParserBuilder)),
        "0" or "1" or "2" or "4" or "5" or "6" => new AimProcessorResult(ParserBuilders, AimSymbologyIdentifier.ParseString(barcode)),
        _ => throw new NotImplementedException($"QrCode information '{modifier}' is not implemented."),
    };

    internal static AimProcessorResult ParseAztecBarcode(string modifier, string barcode) => modifier switch
    {
        "0" or "1" or "2" or "3" or "4" or "5" or "6" or "7" or "8" or "9" or "A" or "B" or "C" => new AimProcessorResult(ParserBuilders, AimSymbologyIdentifier.ParseString(barcode)),
        _ => throw new NotImplementedException($"Aztec information '{modifier}' is not implemented."),
    };
}
