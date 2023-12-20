using BarcodeParserBuilder.Exceptions.CODE39;

namespace BarcodeParserBuilder.Barcodes.CODE39;

internal class Code39BarcodeParserBuilder : BaseBarcodeParserBuilder<Code39Barcode>
{
    protected Code39BarcodeParserBuilder() { }

    public static string? Build(Code39Barcode? barcode)
    {
        var parserBuider = new Code39BarcodeParserBuilder();
        return parserBuider.BuildString(barcode);
    }

    public static bool TryParse(string? barcode, AimSymbologyIdentifier? symbologyIdentifier, out Code39Barcode? code39Barcode)
    {
        try
        {
            code39Barcode = Parse(barcode, symbologyIdentifier);
            return true;
        }
        catch
        {
            code39Barcode = null;
        }
        return false;
    }

    public static Code39Barcode? Parse(string? barcode, AimSymbologyIdentifier? symbologyIdentifier)
    {
        var parserBuider = new Code39BarcodeParserBuilder();
        return parserBuider.ParseString(barcode, symbologyIdentifier);
    }

    protected override string? BuildString(Code39Barcode? barcode)
    {
        if (barcode?.ProductCode?.Code == null)
            return string.Empty;

        var barcodeStr = $"{barcode.Fields[nameof(barcode.ProductCode)].Build()}";
        if (barcode.ReaderInformation is Code39SymbologyIdentifier symbologyIdentifier)
            barcodeStr = $"{barcodeStr}{Code39Checksum.GetBarcodeCheckCharacter(barcodeStr, symbologyIdentifier)}";

        // Reading is validated now in the context of obtained identifier information 
        // Same reading may give different validation results depending on the AIM identifier
        if (!Validate(barcodeStr, (Code39SymbologyIdentifier)barcode.ReaderInformation!))
            throw new Code39ParseException("Code content does not match reader information");

        if (!string.IsNullOrWhiteSpace(barcode.ReaderInformation?.SymbologyIdentifier))
            barcodeStr = $"]{barcode.ReaderInformation!.SymbologyIdentifier}{barcodeStr}";

        return barcodeStr;
    }

    protected override Code39Barcode? ParseString(string? inputBarcode, AimSymbologyIdentifier? symbologyIdentifier)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(inputBarcode))
                return null;

            if (symbologyIdentifier is not Code39SymbologyIdentifier code39identifier)
                throw new Code39ParseException("Invalid Code39 SymbologyIdentifier");

            inputBarcode = code39identifier.StripSymbologyIdentifier(inputBarcode!);

            // Reading is validated now in the context of obtained identifier information 
            // Same reading may give different validation results depending on the AIM identifier
            if (!Validate(inputBarcode, code39identifier))
                throw new Code39ParseException("Code content does not match reader information");

            // When the check character is transmitted, we strip it from the entire reading, 
            // because Code39 does not have any specific structure and check character applies to entire reading
            string strippedInput = Code39Checksum.ValidateAndStripChecksum(inputBarcode, code39identifier);

            // Although Code39 does not specify any structure whether the reading is ProductCode or SerialNumber
            // or something else, we initialize the ProductCode, because it is most aligned with the current implementation
            return new Code39Barcode(code39identifier)
            {
                ProductCode = new Code39ProductCode(strippedInput)
            };
        }
        catch (Exception e)
        {
            throw new Code39ParseException($"Failed to parse Code39 Barcode :{Environment.NewLine}{e.Message}", e);
        }
    }

    /// <summary>
    /// The validation depends from the reader symbology identifier and describes how the reading should be interpreted
    /// </summary>
    /// <param name="value">reading</param>
    /// <param name="information">AimSymbologyIdentifier for the Code39</param>
    /// <returns></returns>
    internal bool Validate(string? value, Code39SymbologyIdentifier information)
    {
        return information.SymbologyIdentifier switch
        {
            Code39SymbologyIdentifier.FullASCIIOlnyModChecksumValue or
            Code39SymbologyIdentifier.FullASCIINoChecksumValue or
            Code39SymbologyIdentifier.FullASCIIMod43ChecksumTransmittedValue or
            Code39SymbologyIdentifier.FullASCIIMod43ChecksumStrippedValue => ValidateFullASCII(value),
            _ => ValidateCode39String(value),
        };
    }

    /// <summary>
    /// Code39 can contain full ASCII set of symbols
    /// </summary>
    /// <param name="value">reading</param>
    /// <returns>whether the reading is subset of full ASCII</returns>
    /// <exception cref="ArgumentNullException"></exception>
    internal bool ValidateFullASCII(string? value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        return System.Text.RegularExpressions.Regex.IsMatch(value!, "^[\x0-\x7F]+$");
    }

    /// <summary>
    /// Regular Code39 can contain limited number of ASCII symbols A-Z, 0-9 and characters -,.,$,+,%,/
    /// </summary>
    /// <param name="value">reading</param>
    /// <returns>whether reading is valid set of Code39 default charset</returns>
    /// <exception cref="ArgumentNullException"></exception>
    internal bool ValidateCode39String(string? value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        return System.Text.RegularExpressions.Regex.IsMatch(value!, @"^[A-Z0-9\s\-\.\$\+\%\/]+$");
    }

    /// <summary>
    /// There is no strict rules, how long the Code39 reading can be. But most readers are not able to read more than 55 symbols
    /// </summary>
    /// <param name="value">read string</param>
    /// <returns>whether the reading is within the limit</returns>
    /// <exception cref="ArgumentNullException"></exception>
    internal bool ValidateCode39ContentLength(string? value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        return value.Length > 1 && value.Length < 56;
    }
}
