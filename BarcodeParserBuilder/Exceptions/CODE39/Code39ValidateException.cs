namespace BarcodeParserBuilder.Exceptions.CODE39;

internal class Code39ValidateException(string message, Exception? e = null) : ValidateException(message, e)
{
}
