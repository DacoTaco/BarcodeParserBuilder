namespace BarcodeParserBuilder.Exceptions
{
    public abstract class BuildException : Exception
    {
        public BuildException(string message, Exception? e = null) : base(message, e) { }
    }
}
