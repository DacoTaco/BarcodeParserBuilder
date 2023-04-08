using BarcodeParserBuilder.Exceptions.GS1;
using BarcodeParserBuilder.Infrastructure;
using System;
using System.IO;
using System.Linq;

namespace BarcodeParserBuilder.Barcodes.GS1
{
    public class GS1Barcode : Barcode
    {
        internal const char GroupSeparator = (char)0x1D;

        public GS1Barcode() : base() { }

        public override BarcodeType BarcodeType => BarcodeType.GS1;
        protected override FieldCollection BarcodeFields { get; } = new FieldCollection()
        {
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
            new FixedLengthGS1Field<double?>("320", 7),

            new GS1Field("10", 20),
            new GS1Field("21", 20),
            new GS1Field("22", 20),
            new GS1Field("235", 28),
            new GS1Field("240", 30),
            new GS1Field("241", 30),
            new GS1Field("242", 6),
            new GS1Field("243", 20),
            new GS1Field("25"),
            new GS1Field<int?>("30", 8),
            new GS1Field("311"),
            new GS1Field("312"),
            new GS1Field("313"),
            new GS1Field("314"),
            new GS1Field("315"),
            new GS1Field("316"),
            new GS1Field("321"),
            new GS1Field("322"),
            new GS1Field("323"),
            new GS1Field("324"),
            new GS1Field("325"),
            new GS1Field("326"),
            new GS1Field("327"),
            new GS1Field("328"),
            new GS1Field("329"),
            new GS1Field("33"),
            new GS1Field("34"),
            new GS1Field("35"),
            new GS1Field("36"),
            new GS1Field("37"),
            new GS1Field("390"),
            new GS1Field("391"),
            new GS1Field<double?>("392", 1, 16),
            new GS1Field("393"),
            new GS1Field("394"),
            new GS1Field("395"),
            new GS1Field("40"),
            new GS1Field("41"),
            new GS1Field("42"),
            new GS1Field("43"),
            new GS1Field("70"),
            new GS1Field("71"),
            new GS1Field("72"),
            new GS1Field("80"),
            new GS1Field("81"),
            new GS1Field("82"),
            new GS1Field("90", 90),
            new GS1Field("91", 90),
            new GS1Field("92", 90),
            new GS1Field("93", 90),
            new GS1Field("94", 90),
            new GS1Field("95", 90),
            new GS1Field("96", 90),
            new GS1Field("97", 90),
            new GS1Field("98", 90),
            new GS1Field("99", 90),
        };

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
        public GS1Field(string identifier, int? maxLength = null) : base(BarcodeType.GS1, identifier, 1, maxLength ?? 90){ }
        public GS1Field(string identifier, int minLength, int maxLength) : base(BarcodeType.GS1, identifier, minLength, maxLength) { }
        public override void Parse(StringReader codeStream)
        {
            if (MinLength <= 0)
                throw new GS1ParseException($"{Identifier} : Invalid Field size.");

            var value = "";
            while ( codeStream.Peek() > -1 && codeStream.Peek() != GS1Barcode.GroupSeparator )
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
            catch(Exception e)
            {
                throw new GS1ParseException($"{Identifier} : {e.Message}", e);
            }
        }
    }

    internal class FixedLengthGS1Field<T> : GS1Field<T>
    {
        public FixedLengthGS1Field(string identifier, int length) : base(identifier, length, length) { }
    }

    internal class GS1Field : GS1Field<string?>
    {
        public GS1Field(string identifier, int? maxLength = null) : base(identifier, maxLength) { }
    }

    internal class FixedLengthGS1Field : FixedLengthGS1Field<string?>
    {
        public FixedLengthGS1Field(string identifier, int length) : base(identifier, length) { }
    }
}
