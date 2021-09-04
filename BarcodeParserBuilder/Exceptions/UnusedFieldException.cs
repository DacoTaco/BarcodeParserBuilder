using System;

namespace BarcodeParserBuilder.Exceptions
{
    public class UnusedFieldException : Exception
    {
        public UnusedFieldException(string fieldName, Exception e = null) : base($"{fieldName} is an unused field.", e) { }
    }
}
