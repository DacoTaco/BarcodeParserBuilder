namespace BarcodeParserBuilder.Exceptions.CODE128;

public class Code128ParseException(string message, Exception? e = null) : ParseException(message, e)
{
}
