using BarcodeParserBuilder.Exceptions.GS1;
using BarcodeParserBuilder.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BarcodeParserBuilder.Barcodes.GS1
{
    public class GS1BarcodeParserBuilder : BaseGS1BarcodeParserBuilder<GS1Barcode> 
    {
        protected GS1BarcodeParserBuilder() { }

        //Overwrite the internal orderNumber to be a high number. this should make it last when ordered
        internal new static int ParsingOrderNumber => 0xFF;

        public static bool TryParse(string barcode, out GS1Barcode? gs1Barcode)
        {
            try
            {
                gs1Barcode = Parse(barcode);
                return true;
            }
            catch
            {
                gs1Barcode = null;
            }
            return false;
        }

        public static GS1Barcode? Parse(string? barcode)
        {
            var parserBuider = new GS1BarcodeParserBuilder();
            return parserBuider.ParseString(barcode);
        }

        public static string? Build(GS1Barcode? barcode)
        {
            if (barcode == null)
                return null;

            var parserBuider = new GS1BarcodeParserBuilder();
            return parserBuider.BuildString(barcode);
        }
    }

    public abstract class BaseGS1BarcodeParserBuilder<T> : BaseBarcodeParserBuilder<T>
        where T : GS1Barcode
    {
        protected override string? BuildString(T? barcode)
        {
            if (barcode == null)
                return "";

            var barcodeString = "";

            foreach (var field in barcode.Fields.OrderBy(f => f.Identifier))
            {
                var value = field.Build();

                if (string.IsNullOrWhiteSpace(value))
                    continue;

                barcodeString += $"{field.Identifier}{value}{(field.FixedLength? "" : GS1Barcode.GroupSeparator.ToString())}";
            }

            if (string.IsNullOrWhiteSpace(barcodeString))
                return null;

            //remove last <GS>, it can never be the last character.
            if (barcodeString.Last() == GS1Barcode.GroupSeparator)
                barcodeString = barcodeString.Remove(barcodeString.Length-1); 

            return barcodeString;
        }

        protected override T? ParseString(string? barcodeString)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(barcodeString))
                    return default;

                barcodeString = AimParser.StripBarcodePrefix(barcodeString);
                if (barcodeString.FirstOrDefault() == GS1Barcode.GroupSeparator)
                    barcodeString = barcodeString[1..];

                var barcode = Activator.CreateInstance<T>();
                var codeStream = new StringReader(barcodeString);
                var applicationIdentifier = "";
                while (codeStream.Peek() > -1)
                {
                    var buffer = new char[string.IsNullOrWhiteSpace(applicationIdentifier) ? 2 : 1];
                    codeStream.Read(buffer, 0, string.IsNullOrWhiteSpace(applicationIdentifier) ? 2 : 1);
                    applicationIdentifier += new string(buffer);

                    if (!applicationIdentifier.All(char.IsDigit) || applicationIdentifier.Any(x => x == '\0') || string.IsNullOrWhiteSpace(applicationIdentifier))
                        throw new GS1ParseException($"Invalid character detected in AI '{applicationIdentifier}'.");

                    if (applicationIdentifier.Length >= 4 || barcode.Fields.Contains(applicationIdentifier))
                    {
                        if (codeStream.Peek() < 0)
                            throw new GS1ParseException("Garbage barcode detected.");

                        if (!barcode.Fields.Contains(applicationIdentifier))
                            throw new GS1ParseException($"Unknown GS1 AI '{applicationIdentifier}'.");

                        barcode.Fields[applicationIdentifier].Parse(codeStream);
                        applicationIdentifier = "";

                        //skip all group separators
                        while (codeStream.Peek() == GS1Barcode.GroupSeparator)
                            codeStream.Read();
                    }
                }
                return barcode;
            }
            catch (Exception e)
            {
                throw new GS1ParseException($"Failed to parse GS1 Barcode :{Environment.NewLine}{e.Message}", e);
            }
        }
    }
}
