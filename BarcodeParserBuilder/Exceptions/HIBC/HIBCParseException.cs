using System;

namespace BarcodeParserBuilder.Exceptions.HIBC
{
    public class HIBCParseException : ParseException
    {
        public HIBCParseException(string message, Exception? e = null) : base(message, e) { }
    }
}
