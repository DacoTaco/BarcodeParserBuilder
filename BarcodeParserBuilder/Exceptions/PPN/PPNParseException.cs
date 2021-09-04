using System;

namespace BarcodeParserBuilder.Exceptions.PPN
{
    public class PPNParseException : ParseException
    {
        public PPNParseException() : this(null) { }
        public PPNParseException(string message, Exception e = null) : base(message, e) { }
    }
}
