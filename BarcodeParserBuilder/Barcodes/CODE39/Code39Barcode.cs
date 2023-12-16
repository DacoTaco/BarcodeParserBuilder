using BarcodeParserBuilder.Exceptions;

namespace BarcodeParserBuilder.Barcodes.CODE39;

public class Code39Barcode : Barcode
{
    public Code39Barcode() : base() { }
    public Code39Barcode(Code39SymbologyIdentifier symbologyIdentifier) : base(symbologyIdentifier) { }

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


    public static string StripCheckCharacter(string inputString, Code39SymbologyIdentifier symbologyIdentifier)
    {
        if (String.IsNullOrEmpty(inputString) || inputString!.Length < 2)
            return inputString;

        switch (symbologyIdentifier.SymbologyIdentifier)
        {
            case Code39SymbologyIdentifier.NoFullASCIIMod43ChecksumTransmittedValue:
            case Code39SymbologyIdentifier.FullASCIIMod43ChecksumTransmittedValue:
                return inputString[0..^1].ToString();
            default:
                return inputString;
        }
    }
}
