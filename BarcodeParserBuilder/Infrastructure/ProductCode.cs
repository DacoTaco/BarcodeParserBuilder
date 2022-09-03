using System;

namespace BarcodeParserBuilder.Infrastructure
{
    public enum ProductCodeType
    {
        Unknown = 0,
        GTIN,
        EAN,
        PPN,
        MSI,
        HIBC
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
        public static ProductCode? ParseGtin(string? code) => string.IsNullOrWhiteSpace(code) ? null : new GtinProductCode(code);
        [Obsolete("Ean is the same as Gtin, Use ParseGtin instead.")]
        public static ProductCode? ParseEan(string? code) => null;
        public static ProductCode? ParseHibc(string? code) => string.IsNullOrWhiteSpace(code) ? null : new HibcProductCode(code);
        public static ProductCode? ParseMsi(string? code) => string.IsNullOrWhiteSpace(code) ? null : new MsiProductCode(code);
        public static ProductCode? ParsePpn(string? code) => string.IsNullOrWhiteSpace(code) ? null : new PpnProductCode(code);
    }
}
