using System;
using System.Collections.Generic;
using System.Text;
using BarcodeParserBuilder.Barcodes.EAN;
using BarcodeParserBuilder.Exceptions.CODE39;
using BarcodeParserBuilder.Infrastructure;

namespace BarcodeParserBuilder.Barcodes.CODE39
{
    internal class Code39BarcodeParserBuilder : BaseBarcodeParserBuilder<Code39Barcode>
    {
        

        protected Code39BarcodeParserBuilder() { }

        public static string? Build(Code39Barcode? barcode)
        {
            var parserBuider = new Code39BarcodeParserBuilder();
            return parserBuider.BuildString(barcode);
        }

        public static bool TryParse(string? barcode, out Code39Barcode? code39Barcode)
        {
            try
            {
                code39Barcode = Parse(barcode);
                return true;
            }
            catch
            {
                code39Barcode = null;
            }
            return false;
        }
        

        public static Code39Barcode? Parse(string? barcode)
        {
            var parserBuider = new Code39BarcodeParserBuilder();
            return parserBuider.ParseString(barcode);
        }

        protected override string? BuildString(Code39Barcode? barcode) => barcode?.SerialNumber;

        protected override Code39Barcode? ParseString(string? inputBarcode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(inputBarcode))
                    return null;
                var aimIdentifier = AimParser.GetAimIdentifierCode(inputBarcode!);
                var aimModifier = AimParser.GetAimIdentifierModifier(inputBarcode!);
                
                inputBarcode = AimParser.StripBarcodePrefix(inputBarcode!);
                Code39ReaderModifier modifier = new Code39ReaderModifier($"{aimIdentifier}{aimModifier}");

                if (!Code39StringParserBuilder.Validate(inputBarcode, modifier))
                {
                    throw new Code39ParseException("Code content does not match reader modifier");
                }

                string strippedInput = Code39Barcode.StripCheckCharacter(inputBarcode, modifier);

                return new Code39Barcode(modifier)
                {
                    ProductCode = new Code39Productcode(strippedInput)
                };
            }
            catch (Exception e)
            {
                throw new Code39ParseException($"Failed to parse Code39 Barcode :{Environment.NewLine}{e.Message}", e);
            }
        }
    }
}
