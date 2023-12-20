namespace BarcodeParserBuilder.Exceptions;

public abstract class BuildException(string message, Exception? e = null) : Exception(message, e)
{
}
