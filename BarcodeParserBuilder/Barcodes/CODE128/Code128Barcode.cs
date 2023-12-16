using BarcodeParserBuilder.Exceptions;

namespace BarcodeParserBuilder.Barcodes.CODE128
{
    public class Code128Barcode : Barcode
    {
        public Code128Barcode() : base() { }

        public Code128Barcode(Code128SymbologyIdentifier symbologyIdentifier) : base(symbologyIdentifier) { }

        public override ProductCode? ProductCode
        {
            get => (ProductCode?)BarcodeFields[nameof(ProductCode)].Value;
            set => BarcodeFields[nameof(ProductCode)].SetValue(value);
        }

        protected override FieldCollection BarcodeFields { get; } = new()
        {
            new BarcodeField<ProductCode>(BarcodeType.CODE128, nameof(ProductCode), 2, 48)
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
