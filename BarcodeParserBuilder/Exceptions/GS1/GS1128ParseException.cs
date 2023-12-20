namespace BarcodeParserBuilder.Exceptions.GS1;

public class GS1128ParseException(string message, Exception? e = null) : ParseException(message, e)
{
}
