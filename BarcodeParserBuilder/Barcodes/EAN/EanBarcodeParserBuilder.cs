using BarcodeParserBuilder.Exceptions.EAN;
using BarcodeParserBuilder.Infrastructure;
using System;
using System.Linq;

namespace BarcodeParserBuilder.Barcodes.EAN
{
    public class EanBarcodeParserBuilder : BaseBarcodeParserBuilder<EanBarcode>
    {
        protected EanBarcodeParserBuilder() { }

        public static string? Build(EanBarcode? barcode)
        {
            var parserBuider = new EanBarcodeParserBuilder();
            return parserBuider.BuildString(barcode);       
        }

        public static bool TryParse(string? barcode, out EanBarcode? eanBarcode)
        {
            try
            {
                eanBarcode = Parse(barcode);
                return true;
            }
            catch
            {
                eanBarcode = null;
            }
            return false;
        }

        public static EanBarcode? Parse(string? barcode)
        {
            var parserBuider = new EanBarcodeParserBuilder();
            return parserBuider.ParseString(barcode);
        }

        protected override string? BuildString(EanBarcode? barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode?.ProductCode?.Code))
                return null;

            var barcodeString = (barcode.ProductCode.Code.Length != 7 && barcode.ProductCode.Code.Length < 11)
                ? $"{barcode.ProductSystem?.Value.ToString()}{barcode.CompanyPrefix}{barcode.ProductCode.Code}"
                : $"{barcode.ProductCode.Code}";

            if (barcodeString.Length < 12 && barcodeString.Length > 8)
                barcodeString = barcodeString.PadLeft(11, '0');

            if (barcodeString.Length < 8 || barcodeString.Length < 13)
            {
                var checkDigit = EanCheckDigitCalculator.CalculateCheckDigit(barcodeString);
                barcodeString = $"{barcodeString}{checkDigit}";
            }
            
            if (barcodeString.Length != 8 && barcodeString.Length != 12 && barcodeString.Length != 13)
                throw new ArgumentException("Invalid Ean barcode to be generated.");

            return barcodeString;
        }

        protected override EanBarcode? ParseString(string? inputBarcode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(inputBarcode))
                    return null;

                if (!inputBarcode.All(char.IsDigit))
                    throw new ArgumentException($"Invalid Ean value '{inputBarcode}'.");

                var checkCharacter = int.Parse(inputBarcode.Last().ToString());
                var barcodeString = inputBarcode[..^1];
                var expectedCheckCharacter = EanCheckDigitCalculator.CalculateCheckDigit(barcodeString);
                if (!int.TryParse(checkCharacter.ToString(), out var checkDigit) || checkCharacter != expectedCheckCharacter)
                    throw new ArgumentException($"Invalid Ean Checkdigit '{checkCharacter}', Expected '{expectedCheckCharacter}'.");

                var barcode = new EanBarcode();
                ProductCode? productCode = null;
                switch (inputBarcode.Length)
                {  
                    case 13: //Ean13 or Gtin
                        if (barcodeString.StartsWith("0") && !barcodeString.StartsWith("02"))
                        {
                            barcodeString = barcodeString[1..];
                            goto case 12;
                        }
                        productCode = ProductCode.ParseGtin(inputBarcode);
                        break;
                    case 8: //Ean8 or UPC-E
                        productCode = ProductCode.ParseEan(barcodeString);
                        break;
                    case 12: //Ean12 or UPC-A
                        barcode.Fields[nameof(EanBarcode.ProductSystem)].Parse(barcodeString.First().ToString());
                        barcodeString = barcodeString[1..];
                        var productSystem = barcode.ProductSystem!;
                        switch (productSystem.Scheme)
                        {                 
                            case EanProductSystemScheme.Reserved:
                                productCode = ProductCode.ParseEan(barcodeString[..5]);
                                break;
                            case EanProductSystemScheme.ManufacturerAndProduct:
                                barcode.CompanyPrefix = barcodeString[0..4];
                                productCode = ProductCode.ParseEan(barcodeString[5..]);
                                break;
                            case EanProductSystemScheme.Coupons:
                            case EanProductSystemScheme.ReservedCoupons:
                                productCode = ProductCode.ParseEan(barcodeString);
                                break;
                            case EanProductSystemScheme.NationalDrugCode:
                                productCode = ProductCode.ParseNdc(barcodeString);
                                break;
                        };
                        break;
                    default:
                        throw new Exception($"Invalid value Length {inputBarcode.Length}. Expected 8, 12 or 13 Bytes.");
                }

                barcode.ProductCode = productCode;
                return barcode;
            }
            catch (Exception e)
            {
                throw new EanParseException($"Failed to parse Ean Barcode :{Environment.NewLine}{e.Message}", e);
            }
        }
    }
}
