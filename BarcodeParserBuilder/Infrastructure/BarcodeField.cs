using BarcodeParserBuilder.Abstraction;
using BarcodeParserBuilder.Exceptions;
using System;
using System.IO;

namespace BarcodeParserBuilder.Infrastructure
{
    internal class BarcodeField<T> : IBarcodeField
    {
        public BarcodeField(BarcodeType barcodeType, string identifier, int length) : this(barcodeType, identifier, length, length) { }
        public BarcodeField(BarcodeType barcodeType, string identifier, int minLength, int? maxLength)
        {
            if (minLength < 0 || 
                (maxLength.HasValue && maxLength.Value < 0) ||
                (maxLength.HasValue && maxLength.Value < MinLength))
                throw new ArgumentException($"Invalid field size '({MinLength}{(MaxLength.HasValue?$"-{MaxLength.Value}":null)})' for '{identifier}'.");

            Identifier = identifier;
            MinLength = minLength;
            MaxLength = maxLength;
            FieldParserBuilder = FieldParserBuilderFactory.CreateFieldParserBuilder(barcodeType, typeof(T));
        }

        public string Identifier { get; }
        public int MinLength { get; }
        public int? MaxLength { get; }
        public bool FixedLength => MinLength == (MaxLength ?? -1);
        public object? Value { get; private set; }
        private IFieldParserBuilder FieldParserBuilder { get; set; }

        private bool ValidateLength(string? value)
        {
            if(!FixedLength && string.IsNullOrWhiteSpace(value))
                return true;

            var valueLength = (value?.Length ?? 0);
            if (!FixedLength && MaxLength.HasValue && valueLength > MaxLength)
                return false;

            if (FixedLength && valueLength != MinLength)
                return false;

            return true;
        }

        public virtual void Parse(StringReader codeStream)
        {
            string value = codeStream.ReadToEnd();

            Parse(value);
        }

        public void Parse(string? value)
        {
            if (!ValidateLength(value))
                throw new ValidateException($"Invalid value Length {value?.Length ?? 0}. Expected {(FixedLength? null : "Max ")}{MaxLength} Bytes.");

            Value = FieldParserBuilder.Parse(value, MinLength, MaxLength);
        }

        public string? Build() => FieldParserBuilder.Build(Value);

        public void SetValue(object? obj)
        {
            Value = FieldParserBuilder.Parse(obj, MinLength, MaxLength);
        }
    }
}
