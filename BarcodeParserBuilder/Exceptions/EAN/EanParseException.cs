namespace BarcodeParserBuilder.Exceptions.EAN;

public class EanParseException(string message, Exception? e = null) : ParseException(message, e)
{
}
