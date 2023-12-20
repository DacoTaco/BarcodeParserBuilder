namespace BarcodeParserBuilder.Exceptions.EAN;

public class EanValidateException(string message, Exception? e = null) : ValidateException(message, e)
{
}
