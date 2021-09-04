using BarcodeParserBuilder.Exceptions.GS1;
using BarcodeParserBuilder.Infrastructure;
using System.Text.RegularExpressions;

namespace BarcodeParserBuilder.GS1
{
    internal class GS1StringParserBuilder : BaseFieldParserBuilder<string>
    {
        protected override string Build(string obj) => string.IsNullOrWhiteSpace(obj) ? null : obj;
        protected override string Parse(string value) => value;

        protected override bool Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            //validate string value using regex
            var regex = new Regex(@"^[!""%&'()*+,-./:;<=>?_a-zA-Z0-9]*$");

            if (!regex.IsMatch(value))
                throw new GS1ValidateException($"Invalid GS1 string value '{value}'.");

            return true;
        }
    }
}
