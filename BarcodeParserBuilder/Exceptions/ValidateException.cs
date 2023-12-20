namespace BarcodeParserBuilder.Exceptions;

public class ValidateException(string message, Exception? e = null) : Exception(message, e)
{
}
