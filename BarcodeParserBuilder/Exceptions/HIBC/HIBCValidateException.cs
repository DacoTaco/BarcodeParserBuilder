using System;

namespace BarcodeParserBuilder.Exceptions.HIBC
{
    public class HIBCValidateException : ValidateException
    {
        public HIBCValidateException() : this(null) { }
        public HIBCValidateException(string message, Exception e = null) : base(message, e) { }
    }
}
