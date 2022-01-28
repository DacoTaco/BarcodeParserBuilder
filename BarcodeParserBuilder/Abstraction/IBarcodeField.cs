using System.IO;

namespace BarcodeParserBuilder.Abstraction
{
    public interface IBarcodeField
    {
        string Identifier { get; }
        object Value { get; }
        bool FixedLength { get; }
        void Parse(string value);
        void Parse(StringReader codeStream);
        string Build();
        void SetValue(object obj);
    }
}
