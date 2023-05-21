namespace BarcodeParserBuilder.Exceptions.PPN
{
    public class PPNParseException : ParseException
    {
        public PPNParseException(string message, Exception? e = null) : base(message, e) { }
    }
}
