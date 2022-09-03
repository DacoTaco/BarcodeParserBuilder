using System;
using System.Linq;

namespace BarcodeParserBuilder.Infrastructure
{
    public class HibcProductCode : ProductCode
    {
        internal HibcProductCode() : base("") { }
        internal HibcProductCode(string code) : base(code) 
        {
            if (code.Length < 2 || code.Length > 18 || code.Any(c => !char.IsLetterOrDigit(c)) || code.Where(c => char.IsLetter(c)).Any(c => !char.IsUpper(c)))
                throw new ArgumentException($"Invalid HIBC value '{code}'.");
        }

        public override ProductCodeType Type { get => ProductCodeType.HIBC; internal set { } }
    }
}
