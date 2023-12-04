using System;
using System.Collections.Generic;
using System.Text;
using BarcodeParserBuilder.Infrastructure;

namespace BarcodeParserBuilder.Barcodes.CODE39
{
    internal class Code39Productcode : ProductCode
    {
        public Code39Productcode(string productCode) : base(productCode) { }

        public override ProductCodeType Type { get => ProductCodeType.CODE39; internal set { } }
    }
}
