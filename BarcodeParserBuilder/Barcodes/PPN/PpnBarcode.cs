using BarcodeParserBuilder.Exceptions.PPN;
using BarcodeParserBuilder.Barcodes.GS1;
using BarcodeParserBuilder.Infrastructure;
using System;
using System.IO;
using System.Linq;

namespace BarcodeParserBuilder.Barcodes.PPN
{
    //Specs : 
    // https://www.ifaffm.de/mandanten/1/documents/04_ifa_coding_system/IFA_Spec_PPN_Code_Handelspackung_EN.pdf
    // https://ec.europa.eu/health/sites/health/files/md_topics-interest/docs/md_ifa_udi-hri_aidc_formats_en.pdf
    // https://bemvo.be/wp-content/uploads/2018/09/2D-matrix-and-Product-codes-reading.pdf

    public class PpnBarcode : Barcode
    {
        internal static char GroupSeparator => (char)0x1D;
        internal static char RecordSeparator => (char)0x1E;
        internal static char EndOfTransmission => (char)0x04;
        internal static string Prefix => $"[)>{RecordSeparator}06{GroupSeparator}";
        internal static string Suffix => $"{RecordSeparator}{EndOfTransmission}";

        public PpnBarcode() : base() { }
        public override BarcodeType BarcodeType => BarcodeType.PPN;
        public override ProductCode? ProductCode
        {
            get => (ProductCode?)(BarcodeFields["9N"].Value ?? BarcodeFields["8P"].Value);
            set 
            {
                if(value == null)
                {
                    BarcodeFields["9N"].SetValue(null);
                    BarcodeFields["8P"].SetValue(null);
                    return;
                }

                if (value.Type == ProductCodeType.GTIN)
                {
                    BarcodeFields["8P"].SetValue(value);
                    BarcodeFields["9N"].SetValue(null);
                }                    
                else
                {
                    BarcodeFields["8P"].SetValue(null);
                    BarcodeFields["9N"].SetValue(value);
                }
            }
        }
        public override BarcodeDateTime? ExpirationDate 
        {
            get => (BarcodeDateTime?)BarcodeFields["D"].Value;
            set => BarcodeFields["D"].SetValue(value);
        }
        public override BarcodeDateTime? ProductionDate 
        {
            get => (BarcodeDateTime?)BarcodeFields["16D"].Value;
            set => BarcodeFields["16D"].SetValue(value);
        }
        public override string? BatchNumber 
        {
            get => string.IsNullOrWhiteSpace((string?)BarcodeFields["1T"].Value) ? null : (string?)BarcodeFields["1T"].Value;
            set => BarcodeFields["1T"].SetValue(value);
        }
        public override string? SerialNumber
        {
            get => string.IsNullOrWhiteSpace((string?)BarcodeFields["S"].Value) ? null : (string?)BarcodeFields["S"].Value;
            set => BarcodeFields["S"].SetValue(value);
        }

        protected override FieldCollection BarcodeFields { get; } = new FieldCollection()
        {
            new PpnField<ProductCode?>("9N", 4, 22),
            new FixedLengthGS1Field<ProductCode?>("8P", 14),

            new PpnField("1T", 20),
            new PpnField("S", 20),
            new PpnField("Q", 8), 
            new PpnField("27Q", 20),
            new FixedLengthPpnField<BarcodeDateTime?>("D", 6),
            new FixedLengthPpnField<BarcodeDateTime?>("16D", 8)
        };

        internal class PpnField<T> : BarcodeField<T>
        {
            public PpnField(string identifier, int? maxLength = null) : base(BarcodeType.PPN, identifier, 1, maxLength ?? 90) { }
            public PpnField(string identifier, int minLength, int maxLength) : base(BarcodeType.PPN, identifier, minLength, maxLength) { }
            protected PpnField(string identifier, int fixedLength) : base(BarcodeType.PPN, identifier, fixedLength, fixedLength) { }
            public override void Parse(StringReader codeStream)
            {
                if (MinLength <= 0)
                    throw new PPNParseException($"{Identifier} : Invalid Field size.");

                var value = "";
                while (codeStream.Peek() > -1 && codeStream.Peek() != GroupSeparator)
                {
                    value += (char)codeStream.Read();

                    if (FixedLength && value.Length == MinLength)
                        break;
                }

                if (value.Any(c => c == GroupSeparator))
                    throw new PPNParseException($"{Identifier} : Invalid PPN value : value contains a group separator");

                try
                {
                    Parse(value);
                }
                catch (Exception e)
                {
                    throw new PPNParseException($"{Identifier} : {e.Message}", e);
                }
            }
        }

        internal class FixedLengthPpnField<T> : PpnField<T>
        {
            public FixedLengthPpnField(string identifier, int length) : base(identifier, length) { }
        }

        internal class PpnField : PpnField<string?>
        {
            public PpnField(string identifier, int? maxLength = null) : base(identifier, maxLength) { }
        }

        internal class FixedLengthPpnField : FixedLengthPpnField<string?>
        {
            public FixedLengthPpnField(string identifier, int length) : base(identifier, length) { }
        }
    }
}
