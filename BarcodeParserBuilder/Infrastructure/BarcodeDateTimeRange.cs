using System;

namespace BarcodeParserBuilder.Infrastructure;

public class BarcodeDateTimeRange
{
    protected BarcodeDateTimeRange(BarcodeDateTime start, BarcodeDateTime end)
    {
        Start = start;
        End = end;
    }

    public BarcodeDateTime Start { get; }
    public BarcodeDateTime End { get; }

    public DateTime StartDateTime => Start.DateTime;
    public DateTime EndDateTime => End.DateTime;

    internal const char DateTimeFormatSeparator = '|';

    public static BarcodeDateTimeRange? Gs1Range(string? value) => Gs1Range(value, null, null);

    public static BarcodeDateTimeRange? Gs1Range(string? value, string? startFormat, string? endFormat)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        startFormat ??= BarcodeDateTime.GS1DateShortFormat;
        endFormat ??= BarcodeDateTime.GS1DateShortFormat;
        if (!BarcodeDateTime.IsGS1Format(startFormat))
            throw new ArgumentException($"Invalid datetime format '{startFormat}' for GS1 date(/time).");
        if (!BarcodeDateTime.IsGS1Format(endFormat))
            throw new ArgumentException($"Invalid datetime format '{endFormat}' for GS1 date(/time).");

        if (value!.Length != startFormat.Length + endFormat.Length)
            throw new ArgumentException($"Invalid datetime value '{value}' for GS1 date range formats.");

        var startDate = BarcodeDateTime.Gs1Date(value[..startFormat.Length], startFormat) ?? throw new ArgumentException($"Invalid datetime value '{value[..startFormat.Length]}' for GS1 date formats.");
        var endDate = BarcodeDateTime.Gs1Date(value[startFormat.Length..], endFormat) ?? throw new ArgumentException($"Invalid datetime value '{value[startFormat.Length..]}' for GS1 date formats.");
        return new BarcodeDateTimeRange(startDate, endDate);
    }

    public static BarcodeDateTimeRange FromDateTimes(DateTime start, DateTime end) => new (BarcodeDateTime.Gs1Date(start), BarcodeDateTime.Gs1Date(end));
}
