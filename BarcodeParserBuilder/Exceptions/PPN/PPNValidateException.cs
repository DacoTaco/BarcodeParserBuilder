using System;

namespace BarcodeParserBuilder.Exceptions.PPN
{
    public class PPNValidateException : ValidateException
    {
        public PPNValidateException() : this(null) { }
        public PPNValidateException(string message, Exception e = null) : base(message, e) { }
    }
}
