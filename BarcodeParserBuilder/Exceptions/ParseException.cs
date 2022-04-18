using System;

namespace BarcodeParserBuilder.Exceptions
{
    public abstract class ParseException : Exception
    {
        public ParseException(string message, Exception? e = null) : base(message, e) { }
    }
}
