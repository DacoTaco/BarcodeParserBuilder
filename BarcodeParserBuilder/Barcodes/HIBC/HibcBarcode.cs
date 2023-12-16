using BarcodeParserBuilder.Exceptions.HIBC;

namespace BarcodeParserBuilder.Barcodes.HIBC
{
    public class HibcBarcode : Barcode
    {
        public HibcBarcode() : this(true) { }
        public HibcBarcode(bool is2DBarcode) : base()
        {
            Is2DBarcode = is2DBarcode;
        }
        public bool Is2DBarcode { get; internal set; }
        public override BarcodeType BarcodeType => BarcodeType.HIBC;
        protected override FieldCollection BarcodeFields { get; } = new FieldCollection()
        {
            new HibcField<ProductCode?>(nameof(ProductCode), 18),
            new FixedLengthHibcField(nameof(LabelerIdentificationCode), 4),
            new FixedLengthHibcField<int?>(nameof(UnitOfMeasure), 1),
            new HibcField<BarcodeDateTime?>("14D", 8),
            new HibcField<BarcodeDateTime?>("16D", 8),
            new HibcField("S", 18),
            new HibcField("B", 18),
            new HibcField<int?>("Q", 5),
        };

        public override ProductCode? ProductCode
        {
            get => (ProductCode?)BarcodeFields[nameof(ProductCode)].Value;
            set => BarcodeFields[nameof(ProductCode)].SetValue(value);
        }
        public override BarcodeDateTime? ExpirationDate
        {
            get => (BarcodeDateTime?)BarcodeFields["14D"].Value;
            set => BarcodeFields["14D"].SetValue(value);
        }
        public override BarcodeDateTime? ProductionDate
        {
            get => (BarcodeDateTime?)BarcodeFields["16D"].Value;
            set => BarcodeFields["16D"].SetValue(value);
        }
        public override string? BatchNumber
        {
            get => (string?)BarcodeFields["B"].Value;
            set => BarcodeFields["B"].SetValue(value);
        }
        public override string? SerialNumber
        {
            get => (string?)BarcodeFields["S"].Value;
            set => BarcodeFields["S"].SetValue(value);
        }

        //HIBC Specific's
        public string? LabelerIdentificationCode
        {
            get => (string?)BarcodeFields[nameof(LabelerIdentificationCode)].Value;
            set => BarcodeFields[nameof(LabelerIdentificationCode)].SetValue(value);
        }

        public int UnitOfMeasure
        {
            get => (int)(BarcodeFields[nameof(UnitOfMeasure)].Value ?? 0);
            set
            {
                if (value > 9 || value < 0)
                    throw new HIBCParseException($"Invalid Unit of measure '{value}'.");

                BarcodeFields[nameof(UnitOfMeasure)].SetValue(value);
            }
        }

        public int Quantity
        {
            get => (int)(BarcodeFields["Q"].Value ?? 0);
            set => BarcodeFields["Q"].SetValue(value);
        }
    }

    internal class HibcField<T> : BarcodeField<T>
    {
        public HibcField(string identifier, int? maxLength = null) : base(BarcodeType.HIBC, identifier, 1, maxLength ?? 90) { }
        public HibcField(string identifier, int minLength, int maxLength) : base(BarcodeType.HIBC, identifier, minLength, maxLength) { }
    }

    internal class FixedLengthHibcField<T> : HibcField<T>
    {
        public FixedLengthHibcField(string identifier, int length) : base(identifier, length, length) { }
    }

    internal class HibcField : HibcField<string?>
    {
        public HibcField(string identifier, int? maxLength = null) : base(identifier, maxLength) { }
    }

    internal class FixedLengthHibcField : FixedLengthHibcField<string?>
    {
        public FixedLengthHibcField(string identifier, int length) : base(identifier, length) { }
    }
}
