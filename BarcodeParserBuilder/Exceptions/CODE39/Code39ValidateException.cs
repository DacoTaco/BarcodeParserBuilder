using System;
using System.Collections.Generic;
using System.Text;

namespace BarcodeParserBuilder.Exceptions.CODE39
{
    internal class Code39ValidateException : ValidateException
    {
        public Code39ValidateException(string message, Exception? e = null) : base(message, e) { }
    }
}
