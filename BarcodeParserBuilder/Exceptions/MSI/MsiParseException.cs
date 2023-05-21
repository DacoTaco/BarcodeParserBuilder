namespace BarcodeParserBuilder.Exceptions.MSI
{
    public class MsiParseException : ParseException
    {
        public MsiParseException(string message, Exception? e = null) : base(message, e) { }
    }
}
