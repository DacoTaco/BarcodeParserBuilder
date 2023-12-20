namespace BarcodeParserBuilder.Exceptions.MSI;

public class MsiValidateException(string message, Exception? e = null) : ValidateException(message, e)
{
}
