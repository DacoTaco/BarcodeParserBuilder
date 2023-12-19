namespace BarcodeParserBuilder.Aim
{
    internal record AimProcessorResult(IEnumerable<Type> ParserBuilders, AimSymbologyIdentifier? SymbologyIdentifier)
    {
        internal AimProcessorResult(AimSymbologyIdentifier? identifier, params Type[] parsers) : this(parsers, identifier) { }
    }

    internal delegate AimProcessorResult AimParserProcessor(string modifier, string barcodeString);
}
