using BarcodeParserBuilder.Exceptions;

namespace BarcodeParserBuilder.Barcodes.MSI
{
    public class MsiBarcode : Barcode
    {
        public MsiBarcode() : this(null) { }
        public MsiBarcode(AimSymbologyIdentifier? identifier) : base(identifier) { }
        public override BarcodeType BarcodeType => BarcodeType.MSI;
        protected override FieldCollection BarcodeFields { get; } = new FieldCollection()
        {
            new BarcodeField<ProductCode>(BarcodeType.MSI, nameof(ProductCode), 3, null),
            new BarcodeField<AimSymbologyIdentifier?>(BarcodeType.MSI, nameof(ReaderInformation), 3),
        };

        public override AimSymbologyIdentifier? ReaderInformation
        {
            get => (AimSymbologyIdentifier?)BarcodeFields[nameof(ReaderInformation)].Value;
            protected set => BarcodeFields[nameof(ReaderInformation)].SetValue(value);
        }

        public override ProductCode? ProductCode
        {
            get => (ProductCode?)BarcodeFields[nameof(ProductCode)].Value;
            set => BarcodeFields[nameof(ProductCode)].SetValue(value);
        }
        public override BarcodeDateTime? ExpirationDate { get => throw new UnusedFieldException(nameof(ExpirationDate)); set => throw new UnusedFieldException(nameof(ExpirationDate)); }
        public override BarcodeDateTime? ProductionDate { get => throw new UnusedFieldException(nameof(ProductionDate)); set => throw new UnusedFieldException(nameof(ProductionDate)); }
        public override string? BatchNumber { get => throw new UnusedFieldException(nameof(BatchNumber)); set => throw new UnusedFieldException(nameof(BatchNumber)); }
        public override string? SerialNumber { get => throw new UnusedFieldException(nameof(SerialNumber)); set => throw new UnusedFieldException(nameof(SerialNumber)); }
    }
}
