namespace BarcodeParserBuilder.Exceptions.CODE39;

public class Code39ParseException(string message, Exception? e = null) : ParseException(message, e)
{
}
