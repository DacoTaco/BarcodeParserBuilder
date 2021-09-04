using System;

namespace BarcodeParserBuilder.Exceptions.MSI
{
    public class MsiParseException : ParseException
    {
        public MsiParseException() : this(null) { }
        public MsiParseException(string message, Exception e = null) : base(message, e) { }
    }
}
