using System;
using System.Collections.Generic;
using System.Text;

namespace BarcodeParserBuilder.Exceptions.CODE128
{
    public class Code128ValidateException : ValidateException
    {
        public Code128ValidateException(string message, Exception? e = null) : base(message, e) { }
    }
}
