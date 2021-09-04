using System;

namespace BarcodeParserBuilder.Exceptions
{
    public abstract class BuildException : Exception
    {
        public BuildException() : this(null) { }
        public BuildException(string message, Exception e = null) : base(message, e) { }
    }
}
