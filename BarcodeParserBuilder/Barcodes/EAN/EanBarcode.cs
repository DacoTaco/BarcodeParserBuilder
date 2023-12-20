using BarcodeParserBuilder.Exceptions;

namespace BarcodeParserBuilder.Barcodes.EAN;

public class EanBarcode(EanSymbologyIdentifier? symbologyIdentifier) : Barcode(symbologyIdentifier)
{
    public EanBarcode() : this(null) { }

    protected override FieldCollection BarcodeFields { get; } =
    [
        new BarcodeField<ProductCode>(BarcodeType.EAN, nameof(ProductCode), 6, 10),
        new BarcodeField<AimSymbologyIdentifier?>(BarcodeType.EAN, nameof(ReaderInformation), 3)
    ];

    public override BarcodeType BarcodeType => BarcodeType.EAN;

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

    public string? CompanyPrefix
    {
        get
        {
            if (BarcodeFields[nameof(ProductCode)].Value is GtinProductCode productCode)
                return productCode.CompanyIdentifier;

            return null;
        }
    }

    public override BarcodeDateTime? ExpirationDate { get => throw new UnusedFieldException(nameof(ExpirationDate)); set => throw new UnusedFieldException(nameof(ExpirationDate)); }
    public override BarcodeDateTime? ProductionDate { get => throw new UnusedFieldException(nameof(ProductionDate)); set => throw new UnusedFieldException(nameof(ProductionDate)); }
    public override string? BatchNumber { get => throw new UnusedFieldException(nameof(BatchNumber)); set => throw new UnusedFieldException(nameof(BatchNumber)); }
    public override string? SerialNumber { get => throw new UnusedFieldException(nameof(SerialNumber)); set => throw new UnusedFieldException(nameof(SerialNumber)); }
}
