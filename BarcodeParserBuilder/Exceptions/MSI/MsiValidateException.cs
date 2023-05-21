namespace BarcodeParserBuilder.Exceptions.MSI
{
    public class MsiValidateException : ValidateException
    {
        public MsiValidateException(string message, Exception? e = null) : base(message, e) { }
    }
}
