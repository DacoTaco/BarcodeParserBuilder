using BarcodeParserBuilder.Exceptions.GS1;

namespace BarcodeParserBuilder.Barcodes.GS1;

public class GS1Barcode(AimSymbologyIdentifier? symbologyIdentifier) : Barcode(symbologyIdentifier)
{
    internal const char GroupSeparator = (char)0x1D;

    public GS1Barcode() : this(null) { }

    public override BarcodeType BarcodeType => BarcodeType.GS1;
    protected override FieldCollection BarcodeFields { get; } =
    [
        new FixedLengthGS1Field("00", 18),
        new FixedLengthGS1Field<ProductCode?>("01", 14),
        new FixedLengthGS1Field<ProductCode?>("02", 14),
        new FixedLengthGS1Field<BarcodeDateTime?>("11", 6),
        new FixedLengthGS1Field<BarcodeDateTime?>("12", 6),
        new FixedLengthGS1Field<BarcodeDateTime?>("13", 6),
        new FixedLengthGS1Field<BarcodeDateTime?>("15", 6),
        new FixedLengthGS1Field<BarcodeDateTime?>("16", 6),
        new FixedLengthGS1Field<BarcodeDateTime?>("17", 6),
        new FixedLengthGS1Field("20", 2),
        new FixedLengthGS1Field<double?>("310", 7),
        new FixedLengthGS1Field<double?>("311", 7),
        new FixedLengthGS1Field<double?>("312", 7),
        new FixedLengthGS1Field<double?>("313", 7),
        new FixedLengthGS1Field<double?>("314", 7),
        new FixedLengthGS1Field<double?>("315", 7),
        new FixedLengthGS1Field<double?>("316", 7),
        new FixedLengthGS1Field<double?>("320", 7),
        new FixedLengthGS1Field<double?>("321", 7),
        new FixedLengthGS1Field<double?>("322", 7),
        new FixedLengthGS1Field<double?>("323", 7),
        new FixedLengthGS1Field<double?>("324", 7),
        new FixedLengthGS1Field<double?>("325", 7),
        new FixedLengthGS1Field<double?>("326", 7),
        new FixedLengthGS1Field<double?>("327", 7),
        new FixedLengthGS1Field<double?>("328", 7),
        new FixedLengthGS1Field<double?>("329", 7),
        new FixedLengthGS1Field<double?>("330", 7),
        new FixedLengthGS1Field<double?>("331", 7),
        new FixedLengthGS1Field<double?>("332", 7),
        new FixedLengthGS1Field<double?>("333", 7),
        new FixedLengthGS1Field<double?>("334", 7),
        new FixedLengthGS1Field<double?>("335", 7),
        new FixedLengthGS1Field<double?>("336", 7),
        new FixedLengthGS1Field<double?>("337", 7),
        new FixedLengthGS1Field<double?>("340", 7),
        new FixedLengthGS1Field<double?>("341", 7),
        new FixedLengthGS1Field<double?>("342", 7),
        new FixedLengthGS1Field<double?>("343", 7),
        new FixedLengthGS1Field<double?>("344", 7),
        new FixedLengthGS1Field<double?>("345", 7),
        new FixedLengthGS1Field<double?>("346", 7),
        new FixedLengthGS1Field<double?>("347", 7),
        new FixedLengthGS1Field<double?>("348", 7),
        new FixedLengthGS1Field<double?>("349", 7),
        new FixedLengthGS1Field<double?>("350", 7),
        new FixedLengthGS1Field<double?>("351", 7),
        new FixedLengthGS1Field<double?>("352", 7),
        new FixedLengthGS1Field<double?>("353", 7),
        new FixedLengthGS1Field<double?>("354", 7),
        new FixedLengthGS1Field<double?>("355", 7),
        new FixedLengthGS1Field<double?>("356", 7),
        new FixedLengthGS1Field<double?>("357", 7),
        new FixedLengthGS1Field<double?>("360", 7),
        new FixedLengthGS1Field<double?>("361", 7),
        new FixedLengthGS1Field<double?>("362", 7),
        new FixedLengthGS1Field<double?>("363", 7),
        new FixedLengthGS1Field<double?>("364", 7),
        new FixedLengthGS1Field<double?>("365", 7),
        new FixedLengthGS1Field<double?>("366", 7),
        new FixedLengthGS1Field<double?>("367", 7),
        new FixedLengthGS1Field<double?>("368", 7),
        new FixedLengthGS1Field<double?>("369", 7),
        new FixedLengthGS1Field("410", 13),
        new FixedLengthGS1Field("411", 13),
        new FixedLengthGS1Field("412", 13),
        new FixedLengthGS1Field("413", 13),
        new FixedLengthGS1Field("414", 13),
        new FixedLengthGS1Field("415", 13),
        new FixedLengthGS1Field("416", 13),
        new FixedLengthGS1Field("417", 13),

        new GS1Field("10", 20),
        new GS1Field("21", 20),
        new GS1Field("22", 20),
        new GS1Field("235", 28),
        new GS1Field("240", 30),
        new GS1Field("241", 30),
        new GS1Field("242", 6),
        new GS1Field("243", 20),
        new GS1Field<int?>("30", 8),
        new GS1Field("37"),
        new GS1Field("390"),
        new GS1Field("391"),
        new GS1Field<double?>("392", 1, 16),
        new GS1Field("393"),
        new GS1Field("394"),
        new GS1Field("395"),
        new GS1Field("40"),
        new GS1Field("42"),
        new GS1Field("43"),
        new GS1Field("70"),
        new GS1Field("71"),
        new GS1Field("72"),
        new GS1Field("80"),
        new GS1Field("81"),
        new GS1Field("82"),
        new GS1Field("90", 30),
        new GS1Field("91", 90),
        new GS1Field("92", 90),
        new GS1Field("93", 90),
        new GS1Field("94", 90),
        new GS1Field("95", 90),
        new GS1Field("96", 90),
        new GS1Field("97", 90),
        new GS1Field("98", 90),
        new GS1Field("99", 90),
        new GS1Field("250", 30),
        new GS1Field("251", 30),
        new GS1Field("253", 30),
        new GS1Field("254", 20),
        new GS1Field("255", 25),
    ];

    public override AimSymbologyIdentifier? ReaderInformation { get; protected set; }

    public override ProductCode? ProductCode
    {
        get => (ProductCode?)BarcodeFields["01"].Value;
        set => BarcodeFields["01"].SetValue(value);
    }
    public override BarcodeDateTime? ProductionDate
    {
        get => (BarcodeDateTime?)BarcodeFields["11"].Value;
        set => BarcodeFields["11"].SetValue(value);
    }
    public override BarcodeDateTime? ExpirationDate
    {
        get => (BarcodeDateTime?)BarcodeFields["17"].Value;
        set => BarcodeFields["17"].SetValue(value);
    }

    public override string? BatchNumber
    {
        get => string.IsNullOrWhiteSpace((string?)BarcodeFields["10"].Value) ? null : (string?)BarcodeFields["10"].Value;
        set => BarcodeFields["10"].SetValue(value);
    }
    public override string? SerialNumber
    {
        get => string.IsNullOrWhiteSpace((string?)BarcodeFields["21"].Value) ? null : (string?)BarcodeFields["21"].Value;
        set => BarcodeFields["21"].SetValue(value);
    }

    public double? NetWeightInKg
    {
        get => (double?)BarcodeFields["310"].Value;
        set => BarcodeFields["310"].SetValue(value);
    }

    public double? NetWeightInPounds
    {
        get => (double?)BarcodeFields["320"].Value;
        set => BarcodeFields["320"].SetValue(value);
    }

    public double? Price
    {
        get => (double?)BarcodeFields["392"].Value;
        set => BarcodeFields["392"].SetValue(value);
    }
}

internal class GS1Field<T> : BarcodeField<T>
{
    public GS1Field(string identifier, int? maxLength = null) : base(BarcodeType.GS1, identifier, 1, maxLength ?? 90) { }
    public GS1Field(string identifier, int minLength, int maxLength) : base(BarcodeType.GS1, identifier, minLength, maxLength) { }
    public override void Parse(StringReader codeStream)
    {
        if (MinLength <= 0)
            throw new GS1ParseException($"{Identifier} : Invalid Field size.");

        var value = "";
        while (codeStream.Peek() > -1 && codeStream.Peek() != GS1Barcode.GroupSeparator)
        {
            value += (char)codeStream.Read();

            if (FixedLength && value.Length == MinLength)
                break;
        }

        if (value.Any(c => c == GS1Barcode.GroupSeparator))
            throw new GS1ParseException($"{Identifier} : Invalid GS1 value : value contains a group separator");

        try
        {
            Parse(value);
        }
        catch (Exception e)
        {
            throw new GS1ParseException($"{Identifier} : {e.Message}", e);
        }
    }
}

internal class FixedLengthGS1Field<T>(string identifier, int length) : GS1Field<T>(identifier, length, length) { }

internal class GS1Field(string identifier, int? maxLength = null) : GS1Field<string?>(identifier, maxLength) { }

internal class FixedLengthGS1Field(string identifier, int length) : FixedLengthGS1Field<string?>(identifier, length) { }
