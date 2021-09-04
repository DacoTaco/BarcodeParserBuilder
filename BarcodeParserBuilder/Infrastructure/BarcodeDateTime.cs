using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace BarcodeParserBuilder.Infrastructure
{
    public class BarcodeDateTime
    {
        protected BarcodeDateTime(DateTime date, string stringValue, string formatString)
        {
            DateTime = date;
            StringValue = stringValue;
            FormatString = formatString;
        }

        public DateTime DateTime { get; private set; }
        public string StringValue { get; private set; }
        public string FormatString  { get; private set; }

        //the following regex is quite a monstrocity, i know.
        //basically it can be split up in 2 parts. 
        //first part is that every letter gets checked if it is not preceeded nor followed by the same letter & exists in the limited count. 
        //this is repeated for each letter.
        //the second part checks that the string ONLY exists of the specific date letters.
        internal static string DateFormatRegex = @"^(?=[^M]*M{0,2}[^M]*$)" +
                                                @"(?=[^Y]*Y{0,4}[^Y]*$)" +
                                                @"(?=[^D]*D{0,2}[^D]*$)" +
                                                @"(?=[^H]*H{0,2}[^H]*$)" +
                                                @"(?=[^J]*J{0,3}[^J]*$)" +
                                                @"[MYDHJJJ]*$";
        internal static string GS1Format => "yyMMdd";
        internal static string PPNFormat => "yyyyMMdd";
        internal static string HIBCShortYearMonthDayHour => "yyMMddHH";
        internal static string HIBCYearMonthDay => "yyyyMMdd";
        internal static string HIBCMonthShortYear => "MMyy";
        internal static string HIBCMonthDayShortYear => "MMddyy";
        internal static string HIBCShortYearMonthDay => "yyMMdd";
        internal static string HIBCShortYearJulianDay => "yyJJJ";
        internal static string HIBCShortYearJulianDayHour => "yyJJJHH";

        public static BarcodeDateTime Gs1Date(DateTime date) => BuildDateString(date, GS1Format);
        public static BarcodeDateTime Gs1Date(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            ParseDateString(value, GS1Format, out var year, out var month, out var day, out var _);
            if (day.Value == 0)
                day = DateTime.DaysInMonth(year.Value, month.Value);

            return new BarcodeDateTime(new DateTime(year.Value, month.Value, day.Value), value, GS1Format);
        }

        public static BarcodeDateTime PpnDate(DateTime date) => BuildDateString(date, PPNFormat);
        public static BarcodeDateTime PpnDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            ParseDateString(value, PPNFormat, out var year, out var month, out var day, out var _);
            if (day.Value == 0)
                day = DateTime.DaysInMonth(year.Value, month.Value);

            return new BarcodeDateTime(new DateTime(year.Value, month.Value, day.Value), value, PPNFormat);
        }

        public static BarcodeDateTime HibcDate(string value, string format)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            ValidateHibcFormat(format);
            ParseDateString(value, format, out var year, out var month, out var day, out var hour);

            //if we have a Julian format we need to just add the days to Jan 1st
            if (format.Any(c => c == 'J'))
            {
                var date = new DateTime(year.Value, 1, 1, hour ?? 0, 0, 0);
                if (day > 1)
                    date = date.AddDays(day.Value - 1);
                return new BarcodeDateTime(date, value, format);
            }

            return new BarcodeDateTime(new DateTime(year.Value, month.Value, day ?? 1, hour ?? 0, 0, 0), value, format);
        }
        public static BarcodeDateTime HibcDate(DateTime date) => HibcDate(date, date.Hour > 0 ? HIBCShortYearMonthDayHour : HIBCYearMonthDay);
        public static BarcodeDateTime HibcDate(DateTime date, string format)
        {
            ValidateHibcFormat(format);
            return BuildDateString(date, format);
        }

        private static void ValidateHibcFormat(string format)
        {
            if( !string.Equals(format, HIBCMonthShortYear, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(format, HIBCShortYearMonthDayHour, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(format, HIBCYearMonthDay, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(format, HIBCMonthDayShortYear, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(format, HIBCShortYearMonthDay, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(format, HIBCShortYearJulianDay, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(format, HIBCShortYearJulianDayHour, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"Invalid Hibc date format '{(string.IsNullOrWhiteSpace(format) ? "(null)" : format)}'.");
        }
        private static void ParseDateString(string input, string format, out int? year, out int? month, out int? day, out int? hour)
        {
            year = null;
            month = null;
            day = null;
            hour = null;

            if (string.IsNullOrWhiteSpace(input))
                return;

            if (string.IsNullOrWhiteSpace(format) || !Regex.IsMatch(format, DateFormatRegex, RegexOptions.IgnoreCase))
                throw new ArgumentException($"Invalid format '{(string.IsNullOrWhiteSpace(format) ? "(null)" : format)}' given.");

            if (input.Length != format.Length || input.Any(c => !char.IsDigit(c)))
                throw new ArgumentException($"Invalid datetime value '{input}' for format '{format}'.");

            foreach (Match match in Regex.Matches(format, @"([a-zA-Z])\1*", RegexOptions.IgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(match?.Value))
                    continue;

                int number = int.Parse(input.Substring(0, match.Value.Length));
                input = input.Remove(0, match.Value.Length);
                switch (match.Value.ToUpper().First())
                {
                    case 'M':
                        month = number;
                        break;
                    case 'Y':
                        year = number;
                        break;
                    case 'J':
                    case 'D':
                        day = number;
                        break;
                    case 'H':
                        hour = number;
                        break;
                    default:
                        throw new ArgumentException($"Unknown date format '{match}'.");
                }
            }

            if (year.HasValue && year < 1000)
                year += 2000;

            return;
        }
        private static BarcodeDateTime BuildDateString(DateTime input, string format)
        {
            if (string.IsNullOrWhiteSpace(format) || !Regex.IsMatch(format, DateFormatRegex, RegexOptions.IgnoreCase))
                throw new ArgumentException($"Invalid format '{(string.IsNullOrWhiteSpace(format) ? "(null)" : format)}' given.");

            string value = null;
            foreach (Match match in Regex.Matches(format, @"([a-zA-Z])\1*", RegexOptions.IgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(match?.Value))
                    continue;

                switch (match.Value.ToUpper().First())
                {
                    case 'M':
                        value += input.Month.ToString("00");
                        break;
                    case 'Y':
                        var year = input.Year.ToString("0000");
                        if (match.Value.Length < 4)
                            year = year.Substring(4 - match.Value.Length, match.Value.Length);
                        value += year;
                        break;
                    case 'J':
                        value += input.DayOfYear.ToString("000");
                        break;
                    case 'D':
                        value += input.Day.ToString("00");
                        break;
                    case 'H':
                        value += input.Hour.ToString("00");
                        break;
                    default:
                        throw new ArgumentException($"Unknown date format '{match}'.");
                }
            }

            if (string.IsNullOrWhiteSpace(value))
                return null;

            return new BarcodeDateTime(input, value, format);
        }
    }
}
