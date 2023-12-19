namespace BarcodeParserBuilder.Abstraction
{
    internal interface IAimParser
    {
        public AimProcessorResult GetParsers(string barcodeString);
    }
}
