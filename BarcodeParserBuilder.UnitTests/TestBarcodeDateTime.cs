using System;
using BarcodeParserBuilder.Infrastructure;

namespace BarcodeParserBuilder.UnitTests
{
    public class TestBarcodeDateTime : BarcodeDateTime
    {
        public TestBarcodeDateTime(DateTime date, string value, string format) : base(date, value, format) { }
    }
}
