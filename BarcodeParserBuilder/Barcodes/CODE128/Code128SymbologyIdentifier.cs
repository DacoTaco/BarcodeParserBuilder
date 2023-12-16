using System;
using System.Collections.Generic;
using System.Text;
using BarcodeParserBuilder.Infrastructure;

namespace BarcodeParserBuilder.Barcodes.CODE128
{
    public class Code128SymbologyIdentifier : AimSymbologyIdentifier
    {
        // From the ISO/IEC 15424:2008(E) standard:
        // 0 Standard data packet.No FNC1 in first or second symbol character position after start character
        // 1 GS1-128 data packet - FNC1 in first symbol character position after start character
        // 2 FNC1 in second symbol character position after start character
        // 4 Concatenation according to International Society for Blood Transfusion specifications has been
        //    performed; concatenated data follows

        public const string StandardNoFNC1Value = "C0";
        public const string GS1128FNC1InFirstSymbolValue = "C1";
        public const string FNC1InSecondSymbolValue = "C2";
        public const string ISBTConcatenatedValue = "C4";
        
        public Code128SymbologyIdentifier() { }

        public Code128SymbologyIdentifier(string symbologyIdentifier) : base(symbologyIdentifier) { }

        public static Code128SymbologyIdentifier Invalid => new("");
        
        // C0 - Standard data packet.No FNC1 in first or second symbol character position after start character
        public static Code128SymbologyIdentifier StandardNoFNC1 => new(StandardNoFNC1Value);

        // C1 - GS1-128 data packet - FNC1 in first symbol character position after start character
        public static Code128SymbologyIdentifier GS1128FNC1InFirstSymbol => new(GS1128FNC1InFirstSymbolValue);

        // C2 - FNC1 in second symbol character position after start character
        public static Code128SymbologyIdentifier FNC1InSecondSymbol => new(FNC1InSecondSymbolValue);

        // C4 - Concatenation according to International Society for Blood Transfusion specifications has been performed; concatenated data follows
        public static Code128SymbologyIdentifier ISBTConcatenated => new(ISBTConcatenatedValue);

    }
}

