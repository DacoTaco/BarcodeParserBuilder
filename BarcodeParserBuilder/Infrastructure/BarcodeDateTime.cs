using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace BarcodeParserBuilder.Infrastructure;

public class BarcodeDateTime
{
    protected BarcodeDateTime(DateTime date, string stringValue, string formatString)
    {
        DateTime = date;
        StringValue = stringValue;
        FormatString = formatString;
    }

    public DateTime DateTime { get; }
    public string StringValue { get; }
    internal string FormatString { get; }

    //the following regex is quite a monstrocity, i know.
    //basically it can be split up in 2 parts. 
    //first part is that every letter gets checked if it is not preceeded nor followed by the same letter & exists in the limited count. 
    //this is repeated for each letter.
    //the second part checks that the string ONLY exists of the specific date letters.
    internal static string DateFormatRegex = @"^(?=[^M]*M{0,2}[^M]*$)" +
                                            @"(?=[^m]*m{0,2}[^m]*$)" +
                                            @"(?=[^y]*y{0,4}[^y]*$)" +
                                            @"(?=[^d]*d{0,2}[^d]*$)" +
                                            @"(?=[^H]*H{0,2}[^H]*$)" +
                                            @"(?=[^J]*J{0,3}[^J]*$)" +
                                            @"[MmydHJJJ]*$";

    internal const char OptionalTimeSeparator = '/';
    internal static string GS1DateShortFormat => "yyMMdd";
    internal static string GS1DateLongFormat => "yyyyMMdd";
    internal static string GS1DateTimeShortFormat => "yyMMddHHmm";
    internal static string GS1DateOptionalTimeFormat => $"yyMMdd{OptionalTimeSeparator}HHmm";
    internal static string GS1DateTimeLongFormat => "yyyyMMddHHmm";
    internal static string PPNFormat => "yyyyMMdd";
    internal static string HIBCShortYearMonthDayHour => "yyMMddHH";
    internal static string HIBCYearMonthDay => "yyyyMMdd";
    internal static string HIBCMonthShortYear => "MMyy";
    internal static string HIBCMonthDayShortYear => "MMddyy";
    internal static string HIBCShortYearMonthDay => "yyMMdd";
    internal static string HIBCShortYearJulianDay => "yyJJJ";
    internal static string HIBCShortYearJulianDayHour => "yyJJJHH";

    public static BarcodeDateTime? Gs1Date(string? value) => ParseGs1DateString(value, null);
    public static BarcodeDateTime? Gs1Date(string? value, string format) => ParseGs1DateString(value, format);
    public static BarcodeDateTime Gs1Date(DateTime date)
    {
        var format = date.Minute > 0 || date.Hour > 0
            ? GS1DateTimeShortFormat
            : GS1DateShortFormat;

        return BuildDateString(date, format);
    }

    internal static bool IsGS1Format(string format) => format == GS1DateShortFormat || format == GS1DateLongFormat || format == GS1DateOptionalTimeFormat || format == GS1DateTimeShortFormat || format == GS1DateTimeLongFormat;

    internal static BarcodeDateTime? ParseGs1DateString(string? value, string? format)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        //if it is the optional time format, we will let it autodetect the format
        if(format == GS1DateOptionalTimeFormat)
            format = null;

        format ??= value!.Length switch
        {
            6 => GS1DateShortFormat,
            8 => GS1DateLongFormat,
            10 => GS1DateTimeShortFormat,
            12 => GS1DateTimeLongFormat,
            _ => throw new ArgumentException($"Invalid datetime value '{value}' for GS1 date formats.")
        };

        if(!IsGS1Format(format))
            throw new ArgumentException($"Invalid datetime format '{format}' for GS1 date(/time).");

        ParseDateString(value, format, out var year, out var month, out var day, out var hour, out var minutes);
        if (year == null || month == null)
            return null;

        if (!day.HasValue || day.Value == 0)
            day = DateTime.DaysInMonth(year.Value, month.Value);

        return new BarcodeDateTime(new DateTime(year.Value, month.Value, day.Value, hour ?? 0, minutes ?? 0, 0), value!, GS1DateShortFormat);
    }

    public static BarcodeDateTime PpnDate(DateTime date) => BuildDateString(date, PPNFormat);
    public static BarcodeDateTime? PpnDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        ParseDateString(value!, PPNFormat, out var year, out var month, out var day, out var _, out var _);
        if (year == null || month == null)
            return null;

        if (!day.HasValue || day.Value == 0)
            day = DateTime.DaysInMonth(year.Value, month.Value);

        return new BarcodeDateTime(new DateTime(year.Value, month.Value, day.Value), value!, PPNFormat);
    }
    public static BarcodeDateTime? HibcDate(string? value, string format)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        ValidateHibcFormat(format);
        ParseDateString(value!, format, out var year, out var month, out var day, out var hour, out var _);

        if (year == null)
            return null;

        month ??= 1;

        //if we have a Julian format we need to just add the days to Jan 1st
        if (format.Any(c => c == 'J' || c == 'j'))
        {
            var date = new DateTime(year.Value, 1, 1, hour ?? 0, 0, 0);
            if (day > 1)
                date = date.AddDays(day.Value - 1);
            return new BarcodeDateTime(date, value!, format);
        }

        return new BarcodeDateTime(new DateTime(year.Value, month.Value, day ?? 1, hour ?? 0, 0, 0), value!, format);
    }
    public static BarcodeDateTime? HibcDate(DateTime date) => HibcDate(date, date.Hour > 0 ? HIBCShortYearMonthDayHour : HIBCYearMonthDay);
    public static BarcodeDateTime? HibcDate(DateTime date, string format)
    {
        ValidateHibcFormat(format);
        return BuildDateString(date, format);
    }

    private static void ValidateHibcFormat(string? format)
    {
        if (!string.Equals(format, HIBCMonthShortYear) &&
            !string.Equals(format, HIBCShortYearMonthDayHour) &&
            !string.Equals(format, HIBCYearMonthDay) &&
            !string.Equals(format, HIBCMonthDayShortYear) &&
            !string.Equals(format, HIBCShortYearMonthDay) &&
            !string.Equals(format, HIBCShortYearJulianDay) &&
            !string.Equals(format, HIBCShortYearJulianDayHour))
            throw new ArgumentException($"Invalid Hibc date format '{(string.IsNullOrWhiteSpace(format) ? "(null)" : format)}'.");
    }
    private static void ParseDateString(string? input, string? format, out int? year, out int? month, out int? day, out int? hour, out int? minutes)
    {
        year = null;
        month = null;
        day = null;
        hour = null;
        minutes = null;
        if (string.IsNullOrWhiteSpace(input))
            return;

        if (string.IsNullOrWhiteSpace(format) || !Regex.IsMatch(format, DateFormatRegex))
            throw new ArgumentException($"Invalid format '{(string.IsNullOrWhiteSpace(format) ? "(null)" : format)}' given.");

        if (input!.Length != format!.Length || input.Any(c => !char.IsDigit(c)))
            throw new ArgumentException($"Invalid datetime value '{input}' for format '{format}'.");

        foreach (var match in Regex.Matches(format, @"([a-zA-Z])\1*").Cast<Match?>())
        {
            if (string.IsNullOrWhiteSpace(match?.Value))
                continue;

            int number = int.Parse(input[..match!.Value.Length]);
            input = input[match.Value.Length..];
            switch (match.Value[0])
            {
                case 'M':
                    month = number;
                    break;
                case 'm':
                    minutes = number;
                    break;
                case 'Y':
                case 'y':
                    year = number;
                    break;
                case 'J':
                case 'j':
                case 'D':
                case 'd':
                    day = number;
                    break;
                case 'H':
                case 'h':
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

    internal static BarcodeDateTime BuildDateString(DateTime input, string format)
    {
        if (string.IsNullOrWhiteSpace(format) || !Regex.IsMatch(format, DateFormatRegex))
            throw new ArgumentException($"Invalid format '{(string.IsNullOrWhiteSpace(format) ? "(null)" : format)}' given.");

        string value = string.Empty;
        foreach (var match in Regex.Matches(format, @"([a-zA-Z])\1*", RegexOptions.IgnoreCase).Cast<Match?>())
        {
            if (string.IsNullOrWhiteSpace(match?.Value))
                continue;

            switch (match!.Value[0])
            {
                case 'M':
                    value += input.Month.ToString("00");
                    break;
                case 'm':
                    value += input.Minute.ToString("00");
                    break;
                case 'Y':
                case 'y':
                    var year = input.Year.ToString("0000");
                    if (match.Value.Length < 4)
                        year = year.Substring(4 - match.Value.Length, match.Value.Length);
                    value += year;
                    break;
                case 'J':
                case 'j':
                    value += input.DayOfYear.ToString("000");
                    break;
                case 'D':
                case 'd':
                        value += input.Day.ToString("00");
                    break;
                case 'H':
                case 'h':
                    value += input.Hour.ToString("00");
                    break;
                default:
                    throw new ArgumentException($"Unknown date format '{match}'.");
            }
        }

        return new BarcodeDateTime(input, value, format);
    }
}
