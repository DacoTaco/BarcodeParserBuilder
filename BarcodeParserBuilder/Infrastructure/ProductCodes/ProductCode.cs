namespace BarcodeParserBuilder.Infrastructure.ProductCodes
{
    public enum ProductCodeType
    {
        Unknown = 0,
        GTIN,
        EAN,
        PPN,
        MSI,
        HIBC,
        CODE39,
        CODE128
    }

    public abstract class ProductCode
    {
        protected ProductCode(string code)
        {
            Code = code;
        }

        /// <summary>
        /// The Raw Product Code value
        /// </summary>
        public string Code { get; internal set; }

        /// <summary>
        /// Product code type
        /// </summary>
        public abstract ProductCodeType Type { get; internal set; }

        //Parse functions
        public static GtinProductCode? ParseGtin(string? code) => string.IsNullOrWhiteSpace(code) ? null : new GtinProductCode(code!);
        [Obsolete("Ean is the same as Gtin, Use ParseGtin instead.")]
        public static ProductCode? ParseEan(string? code) => ParseGtin(code);
        public static HibcProductCode? ParseHibc(string? code) => string.IsNullOrWhiteSpace(code) ? null : new HibcProductCode(code!);
        public static MsiProductCode? ParseMsi(string? code) => string.IsNullOrWhiteSpace(code) ? null : new MsiProductCode(code!);
        public static PpnProductCode? ParsePpn(string? code) => string.IsNullOrWhiteSpace(code) ? null : new PpnProductCode(code!);
        public static Code39ProductCode? ParseCode39(string? value) => string.IsNullOrEmpty(value) ? null : new Code39ProductCode(value!);
        public static Code128ProductCode? ParseCode128(string? value) => string.IsNullOrEmpty(value) ? null : new Code128ProductCode(value!);
    }
}
