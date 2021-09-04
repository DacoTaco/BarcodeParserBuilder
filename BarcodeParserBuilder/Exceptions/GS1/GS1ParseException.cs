using System;
using System.Collections.Generic;
using System.Text;

namespace BarcodeParserBuilder.Exceptions.GS1
{
    public class GS1ParseException : ParseException
    {
        public GS1ParseException() : this(null) { }
        public GS1ParseException(string message, Exception e = null) : base(message, e) { }
    }
}
