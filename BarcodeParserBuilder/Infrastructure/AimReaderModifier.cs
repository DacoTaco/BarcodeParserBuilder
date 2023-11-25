using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BarcodeParserBuilder.Infrastructure
{
    public abstract class AimReaderModifier : IComparable
    {
        public string Value { get; private set; }

        protected AimReaderModifier(string value) { Value = value; }

        public override string ToString() => Value;

        public static IEnumerable<T> GetAll<T>() where T : AimReaderModifier =>
            typeof(T).GetFields(BindingFlags.Public |
                                BindingFlags.Static |
                                BindingFlags.DeclaredOnly)
                .Select(f => f.GetValue(null))
                .Cast<T>();

        public static IEnumerable<string> GetAllValues<T>() where T : AimReaderModifier =>
            typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(f => f.GetValue(f).ToString());
                

        public override bool Equals(object obj)
        {
            return obj is AimReaderModifier aimModifier && aimModifier.Value == this.Value;
        }

        public int CompareTo(object other) => Value.CompareTo(((AimReaderModifier)other).Value);

        //public AimReaderModifier(string readerModifierValue) { Value = readerModifierValue ?? throw new ArgumentNullException(nameof(readerModifierValue)); }

        //public string Value { get; private set; }

        //public override string ToString()
        //{
        //    return Value;
        //}

        //public override bool Equals(object obj)
        //{
        //    return obj is AimReaderModifier aimModifier && aimModifier.Value == this.Value;
        //}

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
