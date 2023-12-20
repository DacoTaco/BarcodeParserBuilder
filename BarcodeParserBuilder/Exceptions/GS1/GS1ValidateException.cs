namespace BarcodeParserBuilder.Exceptions.GS1;

public class GS1ValidateException(string message, Exception? e = null) : ValidateException(message, e)
{
}
