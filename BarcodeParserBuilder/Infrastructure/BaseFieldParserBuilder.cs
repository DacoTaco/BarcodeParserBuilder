using BarcodeParserBuilder.Abstraction;
using BarcodeParserBuilder.Exceptions;

namespace BarcodeParserBuilder.Infrastructure
{
    internal abstract class BaseFieldParserBuilder<T> : IFieldParserBuilder
    {
        public object? Parse(object? obj, int? minimumLength, int? maximumLength) => ValidateAndReTypeObject(obj, minimumLength, maximumLength);
        public object? Parse(string? value, int? minimumLength, int? maximumLength) => ValidateAndParseString(value, minimumLength, maximumLength);
        public string? Build(object? obj) => ValidateAndBuildString(obj);

        private T ValidateAndParseString(string? value, int? minimumLength, int? maximumLength)
        {
            var valueLength = value?.Length ?? 0;
            if (maximumLength.HasValue && valueLength > 0 && valueLength > maximumLength)
                throw new ValidateException($"Invalid string value '{value}' : Too large ({valueLength}/{maximumLength.Value}).");

            if (minimumLength.HasValue && valueLength > 0 && valueLength < minimumLength)
                throw new ValidateException($"Invalid string value '{value}' : Too small ({valueLength}/{minimumLength.Value}).");

            if (!Validate(value))
                throw new ValidateException($"Failed to validate object (value rejected).");

#pragma warning disable CS8603 // Possible null reference return.
            if (string.IsNullOrWhiteSpace(value))
                return default;

            return Parse(value);
#pragma warning restore CS8603 // Possible null reference return.
        }

        private T ValidateAndReTypeObject(object? obj, int? minimumLength = null, int? maximumLength = null)
        {
#pragma warning disable CS8603 // Possible null reference return.
            if (obj == null)
                return default;
#pragma warning restore CS8603 // Possible null reference return.

            var objType = obj.GetType();
            var resultedType = typeof(T);

            if (!resultedType.IsAssignableFrom(objType) || (T)obj == null)
                throw new ValidateException($"Failed to validate object : received {objType.Name} but expected {resultedType.Name}");

            if(!ValidateObject((T)obj) || 
                ((minimumLength.HasValue || maximumLength.HasValue) && !ValidateObjectLength((T)obj, null, null)))
                throw new ValidateException($"Failed to validate object (value rejected).");

            return (T)obj;
        }
        private string? ValidateAndBuildString(object? obj)
        {
            if (obj == null)
                return null;

            var input = ValidateAndReTypeObject(obj);
            return Build(input);
        }

        protected abstract bool Validate(string? value);
        protected virtual bool ValidateObject(T obj) => true;
        protected virtual bool ValidateObjectLength(T obj, int? minimumLength, int? maximumLength) => true;
        protected abstract T Parse(string? value);
        protected abstract string? Build(T obj);
    }
}
