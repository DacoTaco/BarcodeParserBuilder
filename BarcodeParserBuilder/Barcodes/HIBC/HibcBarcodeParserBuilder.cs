﻿using System.Text.RegularExpressions;
using BarcodeParserBuilder.Exceptions.HIBC;

namespace BarcodeParserBuilder.Barcodes.HIBC;

public class HibcBarcodeParserBuilder : BaseBarcodeParserBuilder<HibcBarcode>
{
    protected HibcBarcodeParserBuilder() { }

    public static string? Build(HibcBarcode? barcode)
    {
        if (barcode == null)
            return null;

        var parserBuider = new HibcBarcodeParserBuilder();
        return parserBuider.BuildString(barcode);
    }

    public static bool TryParse(string? barcode, AimSymbologyIdentifier? symbologyIdentifier, out HibcBarcode? hibcBarcode)
    {
        try
        {
            hibcBarcode = Parse(barcode, symbologyIdentifier);
            return true;
        }
        catch
        {
            hibcBarcode = null;
        }
        return false;
    }

    public static HibcBarcode? Parse(string? barcode, AimSymbologyIdentifier? symbologyIdentifier)
    {
        var parserBuider = new HibcBarcodeParserBuilder();
        return parserBuider.ParseString(barcode, symbologyIdentifier);
    }

    public static IList<string> BuildList(HibcBarcode? barcode) => new HibcBarcodeParserBuilder().BuildBarcodes(barcode);

    protected override string? BuildString(HibcBarcode? barcode)
    {
        var list = BuildBarcodes(barcode);
        var barcodeString = (list.Count <= 0) ? "" : list.Select(s => s).Aggregate((i, s) => i + s);
        var identifier = string.IsNullOrEmpty(barcode?.ReaderInformation?.SymbologyIdentifier)
            ? string.Empty
            : $"]{barcode!.ReaderInformation!.SymbologyIdentifier}";

        return string.IsNullOrWhiteSpace(barcodeString) ? null : $"{identifier}{barcodeString}";
    }

    protected override IList<string> BuildBarcodes(HibcBarcode? barcode)
    {
        var list = new List<string>();
        if (string.IsNullOrWhiteSpace(barcode?.ProductCode?.Code) || string.IsNullOrWhiteSpace(barcode!.LabelerIdentificationCode))
            return list;

        var segments = BuildSegments(barcode);
        char? linkCharacter = null;
        for (var index = 0; index < segments.Count; index++)
        {
            var segment = segments[index];
            var ending = "";

            if (index == 0 || !barcode.Is2DBarcode)
                segment = $"+{segment}";
            else
                segment = $"/{segment}";

            //1D Barcode - Add Link & CheckCharacter
            if (!barcode.Is2DBarcode)
            {
                ending = linkCharacter.HasValue ? linkCharacter.ToString() : "";
                var checkCharacter = HibcCheckCharacterCalculator.CalculateSegmentCheckCharacter($"{segment}{ending}");
                ending += checkCharacter;
                if (index == 0)
                    linkCharacter = checkCharacter;
            }
            list.Add($"{segment}{ending}");
        }

        //2D Barcode - Add CheckCharacter if we have any data
        if (barcode.Is2DBarcode && list.Count != 0)
        {
            var barcodeString = list.Select(s => s).Aggregate((i, s) => i + s);
            list[^1] += HibcCheckCharacterCalculator.CalculateSegmentCheckCharacter(barcodeString).ToString();
        }

        return list;
    }

    protected static IList<string> BuildSegments(HibcBarcode? barcode)
    {
        var segments = new List<string>();
        if (string.IsNullOrWhiteSpace(barcode?.ProductCode?.Code) || string.IsNullOrWhiteSpace(barcode!.LabelerIdentificationCode))
            return segments;

        var hasBatchNumber = !string.IsNullOrWhiteSpace(barcode!.BatchNumber);
        var addBatchNumber = hasBatchNumber;
        var hasSerialNumber = !string.IsNullOrWhiteSpace(barcode.SerialNumber);
        var hasExpirationDate = barcode.ExpirationDate != null && !string.IsNullOrWhiteSpace(barcode.ExpirationDate.StringValue);
        var hasQuantity = barcode.Quantity > 0;
        var quantityFormatNumber = barcode.Quantity < 100 ? 8 : 9;
        var expirationFormatNumber = HibcBarcodeSegmentFormat.GetHibcDateTimeFormatIdentifierByFormat(barcode.ExpirationDate?.FormatString ?? string.Empty);

        segments.Add($"{barcode.LabelerIdentificationCode}{barcode.ProductCode!.Code}{barcode.UnitOfMeasure}");

        if (hasQuantity && (addBatchNumber || hasSerialNumber))
        {
            hasQuantity = false;
            var format = HibcBarcodeSegmentFormat.SegmentFormats[quantityFormatNumber];
            var prefix = $"$${(addBatchNumber ? "" : "+")}";
            //prefix + quatity + (possible)7, which is the indicator that a batch or serial is following the date
            var segment = $"{prefix}{quantityFormatNumber}{barcode.Quantity.ToString(format)}{(quantityFormatNumber > 7 ? "7" : string.Empty)}";

            if (addBatchNumber)
            {
                segment += barcode.BatchNumber;
                addBatchNumber = false;
            }
            else
            {
                segment += barcode.SerialNumber;
                hasSerialNumber = false;
            }
            segments.Add(segment);
        }

        if (hasExpirationDate && expirationFormatNumber <= 7 && (addBatchNumber || hasSerialNumber))
        {
            hasExpirationDate = false;
            var prefix = $"$${(addBatchNumber ? "" : "+")}";

            prefix += (expirationFormatNumber < 2 || expirationFormatNumber > 6) ? "" : expirationFormatNumber.ToString();
            var date = BarcodeDateTime.HibcDate(barcode.ExpirationDate!.DateTime, HibcBarcodeSegmentFormat.SegmentFormats[expirationFormatNumber]);
            //prefix + date + (possible)7, which is the indicator that a batch or serial is following the date
            var segment = $"{prefix}{date?.StringValue}{(expirationFormatNumber > 7 ? "7" : string.Empty)}";

            if (addBatchNumber)
            {
                segment += barcode.BatchNumber;
                addBatchNumber = false;
            }
            else
            {
                segment += barcode.SerialNumber;
                hasSerialNumber = false;
            }
            segments.Add(segment);
        }
        else if (addBatchNumber || hasSerialNumber)
        {
            var segment = $"$${(addBatchNumber ? "" : "+")}7";
            if (addBatchNumber)
            {
                segment += barcode.BatchNumber;
                addBatchNumber = false;
            }
            else
            {
                segment += barcode.SerialNumber;
                hasSerialNumber = false;
            }
            segments.Add(segment);
        }

        //if we have no batch number, but we have additional data to write, we will need to add a empty batch number segment
        var hasAdditionalFields = barcode.ProductionDate != null || hasSerialNumber || hasExpirationDate || hasQuantity;
        if (addBatchNumber || (!hasBatchNumber && hasAdditionalFields))
            segments.Add($"${barcode.BatchNumber ?? string.Empty}");

        if (hasQuantity)
            segments.Add($"Q{barcode.Quantity}");

        if (hasExpirationDate)
            segments.Add($"14D{BarcodeDateTime.HibcDate(barcode.ExpirationDate!.DateTime, BarcodeDateTime.HIBCYearMonthDay)!.StringValue}");

        if (!string.IsNullOrWhiteSpace(barcode.ProductionDate?.StringValue))
            segments.Add($"16D{BarcodeDateTime.HibcDate(barcode.ProductionDate!.DateTime, BarcodeDateTime.HIBCYearMonthDay)!.StringValue}");

        if (hasSerialNumber)
            segments.Add($"S{barcode.SerialNumber}");

        return segments;
    }

    protected override HibcBarcode? ParseString(string? barcodeString, AimSymbologyIdentifier? symbologyIdentifier)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(barcodeString))
                return null;

            barcodeString = symbologyIdentifier?.StripSymbologyIdentifier(barcodeString!) ?? barcodeString!;
            if (!Regex.IsMatch(barcodeString, HibcCheckCharacterCalculator.AllowedCharacterRegex))
                throw new HIBCParseException("Invalid HIBC Character detected.");

            if (!(barcodeString.First() == '+'))
                throw new HIBCParseException("Invalid barcode : no '+' DI.");

            //separate all hibc segments. all segments either start with :
            // - a +[LabelOrSegmentID] (1D barcodes concat together) 
            // - a +[LabelID] or /[SegmentID] ( 2D barcodes concat and separated by / )
            var matches = Regex.Matches(barcodeString, @"[+/]{1}[A-Za-z0-9$]{1,3}[A-Za-z0-9+]{0,1}");
            var segments = new List<string>();
            var is2DBarcode = false;

            if (matches.Count == 0)
                throw new HIBCParseException("Invalid beginning or pattern.");

            for (int i = 1; i <= matches.Count; i++)
            {
                var segmentStart = matches[i - 1].Index;
                var segmentLength = (i == matches.Count ? barcodeString.Length : matches[i].Index) - segmentStart;
                var segment = barcodeString.Substring(segmentStart, segmentLength);
                is2DBarcode = is2DBarcode || (segment.First() == '/');
                segments.Add(segment);
            }

            // prefix (+ or / ) + identifier -> minimum 2 characters
            if (segments.Any(s => s.Length < 2))
                throw new HIBCParseException("Barcode contains segments that are too small.");

            //if its a 2D barcode, we will verify the check character & remove it before parsing it
            if (is2DBarcode)
            {
                if (!HibcCheckCharacterCalculator.ValidateSegment(barcodeString))
                    throw new HIBCParseException("Invalid HIBC Barcode.");

                var segment = segments[^1];
                segments[^1] = segment.Remove(segment.Length - 1, 1);
            }

            var barcode = new HibcBarcode(symbologyIdentifier, is2DBarcode);
            char? linkCharacter = null;
            foreach (var segment in segments)
            {
                var isPrimarySegment = barcode.ProductCode == null && (segment.First() == '+') && char.IsLetter(segment[1]);
                var segmentData = segment;

                if (!barcode.Is2DBarcode)
                {
                    HibcCheckCharacterCalculator.ValidateSegment(segmentData, linkCharacter);

                    //remove or save link character, depending if we are parsing the primary or secondary barcode
                    if (linkCharacter.HasValue)
                        segmentData = segmentData.Remove(segmentData.Length - 1, 1);
                    else if (!linkCharacter.HasValue && isPrimarySegment)
                        linkCharacter = segmentData.Last();

                    segmentData = segmentData.Remove(segmentData.Length - 1, 1);
                }

                //remove the DI (+ or /)
                segmentData = segmentData.Remove(0, 1);

                if (isPrimarySegment)
                {
                    barcode.LabelerIdentificationCode = segmentData[..4];
                    barcode.ProductCode = ProductCode.ParseHibc(segmentData[4..^1]);
                    barcode.UnitOfMeasure = int.Parse(segmentData.Last().ToString());
                    continue;
                }

                //Parse Secondary segmentData
                switch (segmentData.First())
                {
                    default:
                        if (!char.IsDigit(segmentData.First()))
                            throw new HIBCParseException($"Invalid Segment starts with '{segmentData.First()}'.");

                        if (segmentData.StartsWith("16D", StringComparison.Ordinal))
                            barcode.ProductionDate = BarcodeDateTime.HibcDate(segmentData[3..], BarcodeDateTime.HIBCYearMonthDay);
                        else if (segmentData.StartsWith("14D", StringComparison.Ordinal))
                        {
                            if (!string.IsNullOrWhiteSpace(barcode.ExpirationDate?.StringValue))
                                throw new HIBCParseException($"ExpirationDate already parsed before '{segmentData}'.");

                            barcode.ExpirationDate = BarcodeDateTime.HibcDate(segmentData[3..], BarcodeDateTime.HIBCYearMonthDay);
                        }
                        //Old segment standard, no prefix, just YYJJJ[LotNumber]
                        //we fix it by adding a $$5 to it and parsing it as such.
                        else
                        {
                            segmentData = $"$$5{segmentData}";
                            goto case '$';
                        }

                        break;
                    case 'Q': // Quantity /Q as of Spec 2.6 version
                        if (barcode.UnitOfMeasure != 9)
                            throw new HIBCParseException($"Using Quantity /Q requires UnitOfMeasure of 9!");
                        segmentData = segmentData[1..];
                        barcode.Quantity = int.Parse(segmentData);
                        break;                        
                    case 'S': //Supplimentary Serial Number
                        if (!string.IsNullOrWhiteSpace(barcode.SerialNumber))
                            throw new HIBCParseException($"Serial already parsed before '{segmentData}'.");

                        barcode.SerialNumber = segmentData[1..];
                        break;
                    case '$':
                        //empty line
                        if(segmentData.Length == 1)
                            continue;
                        
                        var isMultiDataLine = segmentData.StartsWith("$$", StringComparison.Ordinal);
                        segmentData = segmentData[(isMultiDataLine ? 2 : 1)..];
                        var isSerialLine = segmentData.First() == '+';
                        if (isSerialLine)
                            segmentData = segmentData[1..];

                        //$$2 - $$6 -> dated batch or serial line
                        //$$7 -> only batch/serial
                        //$$8 - $$9 -> quantity + (optional) date + (optional) batch/serial lines
                        //in hibc specs 2.6 $$8 & $$9 have been depricated, and are replaced with /Q, but in <= 2.5 they are allowed...
                        //flippin' hibc specs man
                        if (isMultiDataLine && int.TryParse(segmentData.First().ToString(), out var formatNumber))
                        {
                            var format = HibcBarcodeSegmentFormat.SegmentFormats[formatNumber];
                            switch (formatNumber)
                            {
                                //quantity ( + optional date ) ( + optional batch/serial )
                                case 8:
                                case 9:
                                    segmentData = segmentData[1..];
                                    barcode.Quantity = int.Parse(segmentData[..format.Length]);
                                    segmentData = segmentData[format.Length..];

                                    //neither date nor batch follow the quantity
                                    if (segmentData.Length < 1)
                                        break;

                                    if (int.TryParse(segmentData.First().ToString(), out var newFormat))
                                    {
                                        formatNumber = int.Parse(segmentData.First().ToString());
                                        format = HibcBarcodeSegmentFormat.SegmentFormats[formatNumber];
                                    }
                                    goto default;
                                //formats none - 6
                                default:
                                    //just batch/serial
                                    if (formatNumber == 7)
                                    {
                                        segmentData = segmentData[1..];
                                        break;
                                    }

                                    if (formatNumber > 7)
                                        break;

                                    if (formatNumber > 1)
                                        segmentData = segmentData[1..];

                                    if (!string.IsNullOrWhiteSpace(barcode.ExpirationDate?.StringValue))
                                        throw new HIBCParseException($"ExpirationDate already parsed before '{segmentData}'.");

                                    barcode.ExpirationDate = BarcodeDateTime.HibcDate(segmentData[..format.Length], format);
                                    segmentData = segmentData[format.Length..];
                                    break;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(segmentData))
                        {
                            if (isSerialLine)
                            {
                                if (!string.IsNullOrWhiteSpace(barcode.SerialNumber))
                                    throw new HIBCParseException($"Serial already parsed before '{segmentData}'.");

                                barcode.SerialNumber = segmentData;
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(barcode.BatchNumber))
                                    throw new HIBCParseException($"BatchNumber already parsed before '{segmentData}'.");

                                barcode.BatchNumber = segmentData;
                            }
                        }
                        break;
                }
            }

            return barcode;
        }
        catch (Exception e)
        {
            throw new HIBCParseException($"Failed to parse HIBC Barcode :{Environment.NewLine}{e.Message}", e);
        }
    }
}
