using System;
using System.Collections.Generic;
using System.Text;

namespace BarcodeParserBuilder.Exceptions.GS1
{
    public class Code128ValidateException : ValidateException
    {
        public Code128ValidateException(string message, Exception? e = null) : base(message, e) { }
    }
}
