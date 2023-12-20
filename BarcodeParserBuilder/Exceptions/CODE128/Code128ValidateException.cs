namespace BarcodeParserBuilder.Exceptions.CODE128;

public class Code128ValidateException(string message, Exception? e = null) : ValidateException(message, e)
{
}
