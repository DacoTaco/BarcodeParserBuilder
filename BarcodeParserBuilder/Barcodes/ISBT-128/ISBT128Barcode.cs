using BarcodeParserBuilder.Infrastructure;

namespace BarcodeParserBuilder.Barcodes.ISBT128
{
    public class ISBT128Barcode : Barcode
    {
        public override BarcodeType BarcodeType => BarcodeType.ISBT128;

        public override ProductCode? ProductCode { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override BarcodeDateTime? ExpirationDate { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override BarcodeDateTime? ProductionDate { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override string? BatchNumber { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override string? SerialNumber { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        protected override FieldCollection BarcodeFields => throw new System.NotImplementedException();
    }
}

