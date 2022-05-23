using BarcodeParserBuilder.Exceptions;
using BarcodeParserBuilder.Infrastructure;

namespace BarcodeParserBuilder.Barcodes.EAN
{
    public class EanBarcode : Barcode
    {
        public EanBarcode() : base() { }

        protected override FieldCollection BarcodeFields { get; } = new FieldCollection()
        {
            new BarcodeField<EanProductSystem>(BarcodeType.EAN, nameof(ProductSystem), 1),
            new BarcodeField<ProductCode>(BarcodeType.EAN, nameof(ProductCode), 6, 10),
            new BarcodeField<string?>(BarcodeType.EAN, nameof(CompanyPrefix), 0, 6)
        };

        public override BarcodeType BarcodeType => BarcodeType.EAN;

        public override ProductCode? ProductCode 
        {
            get => (ProductCode?)BarcodeFields[nameof(ProductCode)].Value;
            set => BarcodeFields[nameof(ProductCode)].SetValue(value);
        }

        public EanProductSystem? ProductSystem
        {
            get => (EanProductSystem?)BarcodeFields[nameof(ProductSystem)].Value;
            set => BarcodeFields[nameof(ProductSystem)].SetValue(value);
        }

        public string? CompanyPrefix
        {
            get => (string?)BarcodeFields[nameof(CompanyPrefix)].Value;
            set => BarcodeFields[nameof(CompanyPrefix)].SetValue(value);
        }

        public override BarcodeDateTime? ExpirationDate { get => throw new UnusedFieldException(nameof(ExpirationDate)); set => throw new UnusedFieldException(nameof(ExpirationDate)); }
        public override BarcodeDateTime? ProductionDate { get => throw new UnusedFieldException(nameof(ProductionDate)); set => throw new UnusedFieldException(nameof(ProductionDate)); }
        public override string? BatchNumber { get => throw new UnusedFieldException(nameof(BatchNumber)); set => throw new UnusedFieldException(nameof(BatchNumber)); }
        public override string? SerialNumber { get => throw new UnusedFieldException(nameof(SerialNumber)); set => throw new UnusedFieldException(nameof(SerialNumber)); }
    }
}
