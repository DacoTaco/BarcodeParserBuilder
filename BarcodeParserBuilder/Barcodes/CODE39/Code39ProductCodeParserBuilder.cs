﻿using BarcodeParserBuilder.Exceptions.CODE39;

namespace BarcodeParserBuilder.Barcodes.CODE39;

internal class Code39ProductCodeParserBuilder : BaseFieldParserBuilder<ProductCode?>
{
    protected override ProductCode? Parse(string? value) => ProductCode.ParseGtin(value);
    protected override string? Build(ProductCode? obj) => string.IsNullOrWhiteSpace(obj?.Code) ? null : obj!.Code;

    protected override bool Validate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return true;

        if (!value.Any(c => c > 0xFF))
            throw new Code39ValidateException($"Invalid Code128 value '{value}'.");

        return true;
    }

    protected override bool ValidateObject(ProductCode? obj)
    {
        if (obj == null)
            return true;

        if (obj.Type != ProductCodeType.CODE39)
            throw new Code39ValidateException($"Invalid ProductCode type '{obj.Type}'.");

        return true;
    }
}
