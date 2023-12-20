namespace BarcodeParserBuilder.Exceptions.MSI;

public class MsiParseException(string message, Exception? e = null) : ParseException(message, e)
{
}
