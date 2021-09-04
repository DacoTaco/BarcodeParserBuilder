using System;

namespace BarcodeParserBuilder.Exceptions.EAN
{
    public class EanParseException : ParseException
    {
        public EanParseException() : this(null) { }
        public EanParseException(string message, Exception e = null) : base(message, e) { }
    }
}
