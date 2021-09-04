using BarcodeParserBuilder.Exceptions;
using BarcodeParserBuilder.Infrastructure;
using System;

namespace BarcodeParserBuilder.MSI
{
    public class MsiBarcode : Barcode
    {
        public MsiBarcode() : base() { }
        public override BarcodeType BarcodeType => BarcodeType.MSI;
        protected override FieldCollection BarcodeFields { get; } = new FieldCollection()
        {
            new BarcodeField<ProductCode>(BarcodeType.MSI, nameof(ProductCode), 3, null)
        };


        public override ProductCode ProductCode 
        {
            get => (ProductCode)BarcodeFields[nameof(ProductCode)].Value;
            set => BarcodeFields[nameof(ProductCode)].SetValue(value);
        }
        public override BarcodeDateTime ExpirationDate { get => throw new UnusedFieldException(nameof(ExpirationDate)); set => throw new UnusedFieldException(nameof(ExpirationDate)); }
        public override BarcodeDateTime ProductionDate { get => throw new UnusedFieldException(nameof(ProductionDate)); set => throw new UnusedFieldException(nameof(ProductionDate)); }
        public override string BatchNumber { get => throw new UnusedFieldException(nameof(BatchNumber)); set => throw new UnusedFieldException(nameof(BatchNumber)); }
        public override string SerialNumber { get => throw new UnusedFieldException(nameof(SerialNumber)); set => throw new UnusedFieldException(nameof(SerialNumber)); }
    }
}
