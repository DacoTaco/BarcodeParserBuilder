using System;
using System.Collections.Generic;
using System.Linq;

namespace BarcodeParserBuilder.Infrastructure
{
    //source : https://www.datafeedwatch.com/academy/gtin
    //UPC-E - 7 characters long
    //GTIN-8/UPC-A - used mostly for EAN-8 or upc-a barcodes
    //ISBN-10 - not officially a gtin, but lets accept it
    //GTIN-12 - used mostly for UPC barcodes
    //GTIN-13 - used mostly for ISBN, JAN, and EAN-13 barcodes
    //GTIN-14 - this is for wholesale or multipack products
    public enum GtinProductScheme
    {
        Unknown,
        ManufacturerAndProduct,
        Reserved,
        NationalDrugCode,
        ReservedCoupons,
        Coupons
    }

    public class GtinProductCode : ProductCode
    {
        internal readonly static Dictionary<int, GtinProductScheme> EanProductSystems = new Dictionary<int, GtinProductScheme>()
        {
            //0-1 & 6-9 -> Manufacturer & Product
            { 0, GtinProductScheme.ManufacturerAndProduct },
            { 1, GtinProductScheme.ManufacturerAndProduct },
            { 6, GtinProductScheme.ManufacturerAndProduct },
            { 7, GtinProductScheme.ManufacturerAndProduct },
            { 8, GtinProductScheme.ManufacturerAndProduct },
            { 9, GtinProductScheme.ManufacturerAndProduct },

            { 2, GtinProductScheme.Reserved },
            { 3, GtinProductScheme.NationalDrugCode },
            { 4, GtinProductScheme.ReservedCoupons },
            { 5, GtinProductScheme.Coupons },
        };

        internal GtinProductCode() : base("") { }
        internal GtinProductCode(string value) : base(value)
        {
            //all digits and correct size?
            if (!value.All(char.IsDigit))
                throw new ArgumentException($"Invalid GTIN/EAN value '{value}'.");

            if (value.Length != 7 && value.Length != 8 && value.Length != 10 && value.Length != 12 && value.Length != 13 && value.Length != 14)
                throw new ArgumentException($"Invalid GTIN/EAN Length of {value.Length}.");

            Type = (value.Length == 14) ? ProductCodeType.GTIN : ProductCodeType.EAN;
            var checkDigit = int.Parse(value.Last().ToString());
            //pad untill we have at least 13 characters.
            //the beauty of GTIN/EAN is thats its backwards & fowards compatible with EAN8/EAN13/UPC etc etc.
            string productCode = value.Remove(value.Length - 1).PadLeft(13, '0');

            //The checkDigit is calculated as following : 
            //every digit is added together. numbers in even positions are multiplied by 3 before being added.
            //the last digit is the remainder of the next 10 fold number minus the added numbers.
            //this is why if you were to add all numbers (including the check digit) together you should always end up with a 10 fold.
            //for more info, see : https://www.gs1.org/services/how-calculate-check-digit-manually
            //we do it slightly different though by cutting off the check digit from the string and calculating it.
            //this way we can debug and throw the exception showing the difference in check digits.

            var calculatedCheckDigit = productCode.Select((character, index) => index % 2 == 0 ? char.GetNumericValue(character) * 3 : char.GetNumericValue(character)).Sum();
            calculatedCheckDigit %= 10;
            if (calculatedCheckDigit > 0)
                calculatedCheckDigit = 10 - calculatedCheckDigit;

            if (calculatedCheckDigit != checkDigit)
                throw new ArgumentException($"Invalid GTIN/EAN CheckDigit '{checkDigit}', Expected '{calculatedCheckDigit}'.");

            productCode = value[..^1];
            switch (value.Length)
            {
                case 7: //UPC-E without a leading 0 to make it gtin compatible
                case 8: //Ean8 or UPC-E
                case 14:
                    if (productCode.StartsWith("0") && !productCode.StartsWith("02"))
                    {
                        productCode = productCode[1..];
                        goto case 13;
                    }
                    Value = productCode;
                    break;
                case 10: //original ISBN, not converted to GTIN-13
                    productCode = $"000{productCode}";
                    goto case 13;
                case 13: //Ean13 or Gtin        
                    if (productCode.StartsWith("0") && !productCode.StartsWith("02"))
                    {
                        productCode = productCode[1..];
                        goto case 12;
                    }
                    Value = productCode;
                    break;
                case 12: //Ean12 or UPC-A
                    var productSchema = int.Parse(productCode.First().ToString());
                    productCode = productCode[1..];
                    Schema = EanProductSystems[productSchema];
                    switch(Schema)
                    {
                        case GtinProductScheme.ManufacturerAndProduct:
                            CompanyIdentifier = productCode[0..4];
                            Value = productCode[5..];
                            break;
                        case GtinProductScheme.Reserved:
                            Value = productCode[..5];
                            break;
                        case GtinProductScheme.NationalDrugCode:
                        case GtinProductScheme.Coupons:
                        case GtinProductScheme.ReservedCoupons:
                        default:
                            Value = productCode;
                            break;
                    }
                    break;
            }
        }

        public override ProductCodeType Type { get; internal set; }

        //GTIN Specific values
        /// <summary>
        /// GTIN Product code Schema
        /// </summary>
        public GtinProductScheme Schema { get; internal set; } = GtinProductScheme.Unknown;

        /// <summary>
        /// Gtin Product code value
        /// </summary>
        public string Value { get; internal set; } = null!;

        public string? CompanyIdentifier { get; internal set; }
    }
}
