using System;

namespace BarcodeParserBuilder.Exceptions
{
    public class ValidateException : Exception
    {
        public ValidateException(string message, Exception? e = null) : base(message, e) { }
    }
}
