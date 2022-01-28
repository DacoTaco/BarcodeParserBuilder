using BarcodeParserBuilder.Abstraction;
using BarcodeParserBuilder.Infrastructure;
using System;
using System.Collections.ObjectModel;

namespace BarcodeParserBuilder.Barcodes
{
    public abstract class Barcode
    {
        public Barcode() { }

        protected abstract FieldCollection BarcodeFields { get; }

        // Public Barcode Properties 
        /// <summary>
        /// Read only list of the barcode's internal fields
        /// </summary>
        public ReadOnlyFieldCollection Fields
        {
            get
            {
                if (_fields == null)
                    _fields = new ReadOnlyFieldCollection(BarcodeFields);

                return _fields;
            }
        }
        private ReadOnlyFieldCollection _fields = null;
        public abstract BarcodeType BarcodeType { get; }
        public abstract ProductCode ProductCode { get; set; }
        public abstract BarcodeDateTime ExpirationDate { get; set; }
        public abstract BarcodeDateTime ProductionDate { get; set; }
        public abstract string BatchNumber { get; set; }
        public abstract string SerialNumber { get; set; }
    }

    public class FieldCollection : KeyedCollection<string, IBarcodeField>
    {
        protected override string GetKeyForItem(IBarcodeField item) => item.Identifier;
    }

    public class ReadOnlyFieldCollection : ReadOnlyCollection<IBarcodeField>
    {
        readonly KeyedCollection<string, IBarcodeField> innerCollection;

        public ReadOnlyFieldCollection(KeyedCollection<string, IBarcodeField> innerCollection) : base(innerCollection)
        {
            this.innerCollection = innerCollection ?? throw new ArgumentException("innerCollection should not be null");
        }

        public IBarcodeField this[string identifier] => innerCollection[identifier];

        internal bool Contains(string identifier) => innerCollection.Contains(identifier);
    }
}
