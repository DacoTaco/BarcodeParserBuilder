using System;

namespace BarcodeParserBuilder.Exceptions.EAN
{
    public class EanValidateException : ValidateException
    {
        public EanValidateException(string message, Exception? e = null) : base(message, e) { }
    }
}
