namespace BarcodeParserBuilder.Exceptions.PPN;

public class PPNValidateException(string message, Exception? e = null) : ValidateException(message, e)
{
}
