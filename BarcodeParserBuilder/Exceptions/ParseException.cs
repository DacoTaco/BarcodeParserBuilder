namespace BarcodeParserBuilder.Exceptions;

public abstract class ParseException(string message, Exception? e = null) : Exception(message, e)
{
}
