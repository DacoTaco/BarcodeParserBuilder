namespace BarcodeParserBuilder.Exceptions.CODE128
{
    public class Code128ParseException : ParseException
    {
        public Code128ParseException(string message, Exception? e = null) : base(message, e) { }
    }
}
