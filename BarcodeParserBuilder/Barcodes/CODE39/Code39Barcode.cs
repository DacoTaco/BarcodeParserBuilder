using BarcodeParserBuilder.Exceptions;

namespace BarcodeParserBuilder.Barcodes.CODE39;

public class Code39Barcode : Barcode
{
    public Code39Barcode() : this(null) { }
    public Code39Barcode(Code39SymbologyIdentifier? symbologyIdentifier) : base(symbologyIdentifier) { }

    public override ProductCode? ProductCode
    {
        get => (ProductCode?)BarcodeFields[nameof(ProductCode)].Value;
        set => BarcodeFields[nameof(ProductCode)].SetValue(value);
    }

    protected override FieldCollection BarcodeFields { get; } = new()
    {
        new BarcodeField<ProductCode>(BarcodeType.CODE39, nameof(ProductCode), 2, 55)
    };

    public override BarcodeType BarcodeType => BarcodeType.CODE39;


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
