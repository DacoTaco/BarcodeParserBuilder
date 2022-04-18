using System;

namespace BarcodeParserBuilder.Exceptions.GS1
{
    public class GS1ParseException : ParseException
    {
        public GS1ParseException(string message, Exception? e = null) : base(message, e) { }
    }
}
