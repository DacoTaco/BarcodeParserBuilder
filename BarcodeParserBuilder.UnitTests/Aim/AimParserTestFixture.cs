using BarcodeParserBuilder.Abstraction;
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
        private readonly IAimParser _aimParser = new AimParser();

        [Theory]
        [MemberData(nameof(ValidAimPrefixTestCases))]
        public void PrefixReturnsCorrectParserBuilders(string prefix, IEnumerable<Type> expectedParserBuilders)
        {
            //Arrange
            var barcode = $"{prefix}49654";

            //Act
            var result = _aimParser.GetParsers(barcode);

            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(expectedParserBuilders.Count());

            if (expectedParserBuilders.Any())
                result.Should().Contain(expectedParserBuilders);
        }

        public static TheoryData<string, IEnumerable<Type>> ValidAimPrefixTestCases()
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

            return new TheoryData<string, IEnumerable<Type>>()
            {
                //Code39
                { "]A0", code39Parsers },
                { "]A1", code39Parsers },
                { "]A2", code39Parsers },
                { "]A3", code39Parsers },
                { "]A4", code39Parsers },
                { "]A5", code39Parsers },
                { "]A7", code39Parsers },

                //Code128
                { "]C1", new[] {typeof(GS1128BarcodeParserBuilder)} },
                { "]C0", code128Parsers },
                { "]C2", gs1Parsers },
                { "]C4", gs1Parsers },

                //DataMatrix
                { "]d2", new[] {typeof(GS1BarcodeParserBuilder)} },
                { "]d0", AimParser.ParserBuilders },
                { "]d1", AimParser.ParserBuilders },
                { "]d3", AimParser.ParserBuilders },
                { "]d4", AimParser.ParserBuilders },
                { "]d5", AimParser.ParserBuilders },
                { "]d6", AimParser.ParserBuilders },

                //EAN
                { "]E0", new[] {typeof(EanBarcodeParserBuilder)} },
                { "]E1", new[] {typeof(EanBarcodeParserBuilder)} },
                { "]E2", new[] {typeof(EanBarcodeParserBuilder)} },
                { "]E3", new[] {typeof(EanBarcodeParserBuilder)} },
                { "]E4", new[] {typeof(EanBarcodeParserBuilder)} },

                //GS1
                { "]e0", new[] {typeof(GS1BarcodeParserBuilder) } },
                { "]e1", new[] {typeof(GS1BarcodeParserBuilder) } },
                { "]e2", new[] {typeof(GS1BarcodeParserBuilder) } },
                { "]e3", new[] {typeof(GS1BarcodeParserBuilder) } },

                //MSI
                { "]M0", new[] {typeof(MsiBarcodeParserBuilder)} },
                { "]M1", new[] {typeof(MsiBarcodeParserBuilder) } },
                { "]M2", new[] {typeof(MsiBarcodeParserBuilder) } },
                { "]M3", new[] {typeof(MsiBarcodeParserBuilder) } },

                //??
                { "]J1", new[] {typeof(GS1BarcodeParserBuilder) } },

                //QR Code
                { "]Q3", new[] {typeof(GS1BarcodeParserBuilder)} },
                { "]Q0", AimParser.ParserBuilders },
                { "]Q1", AimParser.ParserBuilders },
                { "]Q2", AimParser.ParserBuilders },
                { "]Q4", AimParser.ParserBuilders },
                { "]Q5", AimParser.ParserBuilders },
                { "]Q6", AimParser.ParserBuilders },

                //No Barcode
                { "]Z", Enumerable.Empty<Type>() },

                //Aztec
                { "]z0", AimParser.ParserBuilders },
                { "]z1", AimParser.ParserBuilders },
                { "]z2", AimParser.ParserBuilders },
                { "]z3", AimParser.ParserBuilders },
                { "]z4", AimParser.ParserBuilders },
                { "]z5", AimParser.ParserBuilders },
                { "]z6", AimParser.ParserBuilders },
                { "]z7", AimParser.ParserBuilders },
                { "]z8", AimParser.ParserBuilders },
                { "]z9", AimParser.ParserBuilders },
                { "]zA", AimParser.ParserBuilders },
                { "]zB", AimParser.ParserBuilders },
                { "]zC", AimParser.ParserBuilders },
            };
        }
    }
}
