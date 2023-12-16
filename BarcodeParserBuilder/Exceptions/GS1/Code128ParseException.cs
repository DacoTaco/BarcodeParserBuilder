using System;
using System.Collections.Generic;
using System.Text;

namespace BarcodeParserBuilder.Exceptions.GS1
{
    public class Code128ParseException : ParseException
    {
        public Code128ParseException(string message, Exception? e = null) : base(message, e) { }
    }
}
