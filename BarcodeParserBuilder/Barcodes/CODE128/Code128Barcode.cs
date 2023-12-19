using BarcodeParserBuilder.Exceptions;

namespace BarcodeParserBuilder.Barcodes.CODE128
{
    public class Code128Barcode : Barcode
    {
        public Code128Barcode() : this(null) { }

        public Code128Barcode(Code128SymbologyIdentifier? symbologyIdentifier) : base(symbologyIdentifier) { }

        public override ProductCode? ProductCode
        {
            get => (ProductCode?)BarcodeFields[nameof(ProductCode)].Value;
            set => BarcodeFields[nameof(ProductCode)].SetValue(value);
        }

        public override AimSymbologyIdentifier? ReaderInformation
        {
            get => (AimSymbologyIdentifier?)BarcodeFields[nameof(ReaderInformation)].Value;
            protected set => BarcodeFields[nameof(ReaderInformation)].SetValue(value);
        }

        protected override FieldCollection BarcodeFields { get; } = new()
        {
            new BarcodeField<ProductCode>(BarcodeType.CODE128, nameof(ProductCode), 2, 48),
            new BarcodeField<AimSymbologyIdentifier?>(BarcodeType.CODE128, nameof(ReaderInformation), 3),
        };

        public override BarcodeType BarcodeType => BarcodeType.CODE128;

        public override BarcodeDateTime? ExpirationDate
        {
            get => throw new UnusedFieldException(nameof(ExpirationDate));
            set => throw new UnusedFieldException(nameof(ExpirationDate));
        }

        public override BarcodeDateTime? ProductionDate
        {
            get => throw new UnusedFieldException(nameof(ProductionDate));
            set => throw new UnusedFieldException(nameof(ProductionDate));
        }

        public override string? BatchNumber
        {
            get => throw new UnusedFieldException(nameof(BatchNumber));
            set => throw new UnusedFieldException(nameof(BatchNumber));
        }

        public override string? SerialNumber
        {
            get => throw new UnusedFieldException(nameof(SerialNumber));
            set => throw new UnusedFieldException(nameof(SerialNumber));
        }
    }
}
