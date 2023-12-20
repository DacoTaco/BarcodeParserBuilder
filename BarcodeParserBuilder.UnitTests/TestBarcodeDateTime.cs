using BarcodeParserBuilder.Infrastructure;

namespace BarcodeParserBuilder.UnitTests;

public class TestBarcodeDateTime(DateTime date, string value, string format) : BarcodeDateTime(date, value, format)
{
}
