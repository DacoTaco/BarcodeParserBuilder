using System;

namespace BarcodeParserBuilder.Exceptions
{
    public abstract class ParseException : Exception
    {
        public ParseException() : this(null) { }
        public ParseException(string message, Exception e = null) : base(message, e) { }
    }
}
