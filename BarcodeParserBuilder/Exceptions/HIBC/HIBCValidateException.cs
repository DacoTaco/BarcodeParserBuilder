namespace BarcodeParserBuilder.Exceptions.HIBC;

public class HIBCValidateException(string message, Exception? e = null) : ValidateException(message, e)
{
}
