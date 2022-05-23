using System;
using System.Collections.Generic;

namespace BarcodeParserBuilder.Barcodes.EAN
{
    public enum EanProductSystemScheme
    {
        ManufacturerAndProduct,
        Reserved,
        NationalDrugCode,
        ReservedCoupons,
        Coupons
    }

    public class EanProductSystem
    {
        private static Dictionary<int, EanProductSystemScheme> _eanProductSystems = new Dictionary<int, EanProductSystemScheme>
        {
            //0-1 & 6-9 -> Manufacturer & Product
            { 0, EanProductSystemScheme.ManufacturerAndProduct },
            { 1, EanProductSystemScheme.ManufacturerAndProduct },
            { 6, EanProductSystemScheme.ManufacturerAndProduct },
            { 7, EanProductSystemScheme.ManufacturerAndProduct },
            { 8, EanProductSystemScheme.ManufacturerAndProduct },
            { 9, EanProductSystemScheme.ManufacturerAndProduct },

            { 2, EanProductSystemScheme.Reserved },
            { 3, EanProductSystemScheme.NationalDrugCode },
            { 4, EanProductSystemScheme.ReservedCoupons },
            { 5, EanProductSystemScheme.Coupons },
        };

        public EanProductSystemScheme Scheme { get; private set; }
        public int Value { get; private set; }

        internal EanProductSystem(EanProductSystemScheme scheme, int value)
        {
            Scheme = scheme;
            Value = value;
        }

        public static EanProductSystem Create(int numberSystem)
        {
            if (!_eanProductSystems.ContainsKey(numberSystem))
                throw new ArgumentException($"invalid Ean Product system '{numberSystem}'");

            return new EanProductSystem(_eanProductSystems[numberSystem], numberSystem);
        }
    }
}
