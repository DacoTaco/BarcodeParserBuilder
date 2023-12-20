namespace BarcodeParserBuilder.Exceptions.HIBC;

public class HIBCParseException(string message, Exception? e = null) : ParseException(message, e)
{
}
