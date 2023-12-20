namespace BarcodeParserBuilder.Exceptions.PPN;

public class PPNParseException(string message, Exception? e = null) : ParseException(message, e)
{
}
