namespace BarcodeParserBuilder.Infrastructure
{
    public enum BarcodeType
    {
        Unknown = 0,
        GS1,
        //GS1-128
        GS1128,
        EAN,
        PPN,
        MSI,
        HIBC,
        ISBT128
    }
}
