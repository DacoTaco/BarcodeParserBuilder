using BarcodeParserBuilder.Aim;
using BarcodeParserBuilder.Barcodes.CODE128;
using BarcodeParserBuilder.Barcodes.CODE39;
using BarcodeParserBuilder.Barcodes.EAN;
using BarcodeParserBuilder.Barcodes.GS1;
using BarcodeParserBuilder.Barcodes.HIBC;
using BarcodeParserBuilder.Barcodes.ISBT128;
using BarcodeParserBuilder.Barcodes.MSI;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Aim
{
    public class AimParserTestFixture
    {
        private readonly AimParser _aimParser = new();

        [Theory]
        [MemberData(nameof(ValidAimPrefixTestCases))]
        public void PrefixReturnsCorrectParserBuilders(string prefix, IEnumerable<Type> expectedParserBuilders, AimSymbologyIdentifier expectedIdentifier)
        {
            //Arrange
            var barcode = $"{prefix}49654";

            //Act
            var result = _aimParser.GetParsers(barcode);

            //Assert
            result.Should().NotBeNull();
            if (expectedIdentifier != null)
                result.SymbologyIdentifier.Should().BeOfType(expectedIdentifier.GetType());
            result.SymbologyIdentifier.Should().Be(expectedIdentifier);
            result.ParserBuilders.Should().HaveCount(expectedParserBuilders.Count());

            if (expectedParserBuilders.Any())
                result.ParserBuilders.Should().Contain(expectedParserBuilders);
        }

        public static TheoryData<string, IEnumerable<Type>, AimSymbologyIdentifier> ValidAimPrefixTestCases()
        {
            var code39Parsers = new[]
            {
                typeof(Code39BarcodeParserBuilder)
            };

            var code128Parsers = new[]
            {
                typeof(Code128BarcodeParserBuilder)
            };

            var gs1Parsers = new[]
            {
                typeof(GS1128BarcodeParserBuilder),
                typeof(ISBT128BarcodeParserBuilder),
                typeof(HibcBarcodeParserBuilder),
                typeof(MsiBarcodeParserBuilder),
            };

            return new TheoryData<string, IEnumerable<Type>, AimSymbologyIdentifier>()
            {
                //Code39
                { "]A0", code39Parsers, new Code39SymbologyIdentifier("A0") },
                { "]A1", code39Parsers, new Code39SymbologyIdentifier("A1") },
                { "]A2", code39Parsers, new Code39SymbologyIdentifier("A2") },
                { "]A3", code39Parsers, new Code39SymbologyIdentifier("A3") },
                { "]A4", code39Parsers, new Code39SymbologyIdentifier("A4") },
                { "]A5", code39Parsers, new Code39SymbologyIdentifier("A5") },
                { "]A7", code39Parsers, new Code39SymbologyIdentifier("A7") },

                //Code128
                { "]C1", new[] {typeof(GS1128BarcodeParserBuilder)}, new Code128SymbologyIdentifier("C1") },
                { "]C0", code128Parsers, new Code128SymbologyIdentifier("C0") },
                { "]C2", gs1Parsers, new Code128SymbologyIdentifier("C2") },
                { "]C4", gs1Parsers, new Code128SymbologyIdentifier("C4") },

                //DataMatrix
                { "]d2", new[] {typeof(GS1BarcodeParserBuilder)}, new GS1AimSymbologyIdentifier("d2") },
                { "]d0", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]d0") },
                { "]d1", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]d1") },
                { "]d3", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]d3") },
                { "]d4", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]d4") },
                { "]d5", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]d5") },
                { "]d6", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]d6") },

                //EAN
                { "]E0", new[] {typeof(EanBarcodeParserBuilder)}, new EanSymbologyIdentifier("E0") },
                { "]E1", new[] {typeof(EanBarcodeParserBuilder)}, new EanSymbologyIdentifier("E1") },
                { "]E2", new[] {typeof(EanBarcodeParserBuilder)}, new EanSymbologyIdentifier("E2") },
                { "]E3", new[] {typeof(EanBarcodeParserBuilder)}, new EanSymbologyIdentifier("E3") },
                { "]E4", new[] {typeof(EanBarcodeParserBuilder)}, new EanSymbologyIdentifier("E4") },

                //GS1
                { "]e0", new[] {typeof(GS1BarcodeParserBuilder) }, new GS1AimSymbologyIdentifier("e0") },
                { "]e1", new[] {typeof(GS1BarcodeParserBuilder) }, new GS1AimSymbologyIdentifier("e1") },
                { "]e2", new[] {typeof(GS1BarcodeParserBuilder) }, new GS1AimSymbologyIdentifier("e2") },
                { "]e3", new[] {typeof(GS1BarcodeParserBuilder) }, new GS1AimSymbologyIdentifier("e3") },

                //MSI
                { "]M0", new[] {typeof(MsiBarcodeParserBuilder) }, AimSymbologyIdentifier.ParseString("]M0") },
                { "]M1", new[] {typeof(MsiBarcodeParserBuilder) }, AimSymbologyIdentifier.ParseString("]M1") },
                { "]M2", new[] {typeof(MsiBarcodeParserBuilder) }, AimSymbologyIdentifier.ParseString("]M2") },
                { "]M3", new[] {typeof(MsiBarcodeParserBuilder) }, AimSymbologyIdentifier.ParseString("]M3") },

                //??
                { "]J1", new[] {typeof(GS1BarcodeParserBuilder) }, new GS1AimSymbologyIdentifier("J1") },

                //QR Code
                { "]Q3", new[] {typeof(GS1BarcodeParserBuilder)}, new GS1AimSymbologyIdentifier("Q3") },
                { "]Q0", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]Q0") },
                { "]Q1", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]Q1") },
                { "]Q2", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]Q2") },
                { "]Q4", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]Q4") },
                { "]Q5", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]Q5") },
                { "]Q6", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]Q6") },

                //No Barcode
                { "]Z", Enumerable.Empty<Type>(), null },

                //Aztec
                { "]z0", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]z0") },
                { "]z1", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]z1") },
                { "]z2", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]z2") },
                { "]z3", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]z3") },
                { "]z4", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]z4") },
                { "]z5", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]z5") },
                { "]z6", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]z6") },
                { "]z7", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]z7") },
                { "]z8", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]z8") },
                { "]z9", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]z9") },
                { "]zA", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]zA") },
                { "]zB", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]zB") },
                { "]zC", AimParser.ParserBuilders, AimSymbologyIdentifier.ParseString("]zC") },
            };
        }
    }
}
