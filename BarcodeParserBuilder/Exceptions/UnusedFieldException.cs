namespace BarcodeParserBuilder.Exceptions;

public class UnusedFieldException(string fieldName, Exception? e = null) : Exception($"{fieldName} is an unused field.", e)
{
}
