using BarcodeParserBuilder.Exceptions;

namespace BarcodeParserBuilder.Infrastructure
{
    public interface IFieldParserBuilder
    {
        object Parse(object obj);
        object Parse(string value, int? maxLength);
        string Build(object obj);
    }

    internal abstract class BaseFieldParserBuilder<T> : IFieldParserBuilder
    {
        public object Parse(object obj) => ValidateAndParseObject(obj);
        public object Parse(string value, int? maxLength) => ValidateAndParseString(value, maxLength);
        public string Build(object obj) => ValidateAndBuildString(obj);

        private T ValidateAndParseString(string value, int? maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
                return default;

            if (maxLength.HasValue && value.Length > maxLength)
                throw new ValidateException($"Invalid string value '{value}' : Too large ({value.Length}/{maxLength.Value})");

            if (!Validate(value))
                throw new ValidateException($"Valid to validate object (value rejected).");

            return Parse(value);
        }

        private T ValidateAndParseObject(object obj)
        {
            if (obj == null)
                return default;

            var objType = obj.GetType();
            var resultedType = typeof(T);

            if (!resultedType.IsAssignableFrom(objType) || (T)obj == null)
                throw new ValidateException($"Failed to validate object : received {objType.Name} but expected {resultedType.Name}");

            if(!ValidateObject((T)obj))
                throw new ValidateException($"Failed to validate object (value rejected).");

            return (T)obj;
        }

        private string ValidateAndBuildString(object obj)
        {
            if (obj == null)
                return null;

            var input = ValidateAndParseObject(obj);
            return Build(input);
        }

        protected abstract bool Validate(string value);
        protected virtual bool ValidateObject(T obj) => true;
        protected abstract T Parse(string value);
        protected abstract string Build(T obj);
    }
}
