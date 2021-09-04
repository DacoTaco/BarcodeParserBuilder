using System;

namespace BarcodeParserBuilder.Exceptions.GS1
{
    public class GS1ValidateException : ValidateException
    {
        public GS1ValidateException() : this(null) { }
        public GS1ValidateException(string message, Exception e = null) : base(message, e) { }
    }
}
