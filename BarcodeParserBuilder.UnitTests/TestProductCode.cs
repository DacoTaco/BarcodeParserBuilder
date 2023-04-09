using System;
using BarcodeParserBuilder.Infrastructure;

namespace BarcodeParserBuilder.UnitTests
{
    public class TestProductCode
    {
        public static T CreateProductCode<T>(string value, Action<T> setProperties = null) where T : ProductCode
        {
            var productCode = (T)Activator.CreateInstance(typeof(T), true);
            productCode.Code = value;
            setProperties?.Invoke(productCode);

            return productCode;
        }
    }
}
