using BarcodeParserBuilder.Infrastructure;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BarcodeParserBuilder.Barcodes.HIBC
{
    internal class HibcBarcodeSegmentFormat
    {
        public static ReadOnlyDictionary<int, string> SegmentFormats { get; } =
            new ReadOnlyDictionary<int, string>(
                new Dictionary<int, string>()
                {
                    [0] = BarcodeDateTime.HIBCMonthShortYear,
                    [1] = BarcodeDateTime.HIBCMonthShortYear,
                    [2] = BarcodeDateTime.HIBCMonthDayShortYear,
                    [3] = BarcodeDateTime.HIBCShortYearMonthDay,
                    [4] = BarcodeDateTime.HIBCShortYearMonthDayHour,
                    [5] = BarcodeDateTime.HIBCShortYearJulianDay,
                    [6] = BarcodeDateTime.HIBCShortYearJulianDayHour,
                    [7] = "",
                    [8] = "00",
                    [9] = "00000",
                    [10] = BarcodeDateTime.HIBCYearMonthDay,
                });

        public static int GetHibcDateTimeFormatIdentifierByFormat(string format) => SegmentFormats.First(x => x.Value?.ToUpper() == format.ToUpper()).Key;
        public static string GetHibcDateTimeFormatByIdentifier(int id) => SegmentFormats.Single(x => x.Key == id).Value;
    }
}
