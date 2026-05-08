using BarcodeParserBuilder.Aim;
using BarcodeParserBuilder.Barcodes.GS1;
using BarcodeParserBuilder.Exceptions.GS1;
using BarcodeParserBuilder.Infrastructure;
using BarcodeParserBuilder.Infrastructure.ProductCodes;
using FluentAssertions;
using Xunit;

namespace BarcodeParserBuilder.UnitTests.Barcodes.GS1;

public class GS1BarcodeParserBuilderTestFixture : BaseBarcodeTestFixture
{
    public static char GroupSeparator => (char)0x1D;
    public static string GS1DateFormat => "yyMMdd";

    [Theory]
    [MemberData(nameof(ValidGs1Barcodes))]
    [MemberData(nameof(ValidGs1ParsingBarcodes))]
    public void CanParseBarcodeString(string barcode, GS1Barcode expectedBarcode)
    {
        //Arrange & Act
        var parsed = GS1BarcodeParserBuilder.TryParse(barcode, expectedBarcode.ReaderInformation, out var result);
        Action parseAction = () => GS1BarcodeParserBuilder.Parse(barcode, expectedBarcode.ReaderInformation);

        //Assert
        parsed.Should().BeTrue();
        parseAction.Should().NotThrow();
        CompareBarcodeObjects(expectedBarcode, result);

        if (expectedBarcode.NetWeightInKg.HasValue)
            result!.NetWeightInKg!.Value.Should().BeApproximately(expectedBarcode.NetWeightInKg.Value, 0.000001d);
        else
            result!.NetWeightInKg.Should().BeNull();

        if (expectedBarcode.NetWeightInPounds.HasValue)
            result.NetWeightInPounds!.Value.Should().BeApproximately(expectedBarcode.NetWeightInPounds.Value, 0.000001d);
        else
            result.NetWeightInPounds.Should().BeNull();

        if (expectedBarcode.Price.HasValue)
            result.Price!.Value.Should().BeApproximately(expectedBarcode.Price.Value, 0.000000000000001d);
        else
            result.Price.Should().BeNull();

        foreach (var field in expectedBarcode.Fields)
        {
            var identifier = field.Identifier;
            result.Fields.Should().ContainSingle(f => f.Identifier == identifier);
            var resultedField = result.Fields.Single(f => f.Identifier == identifier);
            if(field.Value is null)
            {
                resultedField.Value.Should().BeNull($"'{identifier}' is null");
                continue;
            }

            if(field.Value is BarcodeDateTimeRange range)
            {
                var resultedRange = (resultedField.Value as BarcodeDateTimeRange);
                resultedRange.Should().NotBeNull();
                resultedRange.StartDateTime.Should().Be(range.StartDateTime);
                resultedRange.EndDateTime.Should().Be(range.EndDateTime);
            }
            else if(field.Value is double number)
                 ((double?)resultedField.Value).Should().BeApproximately(number, 0.000001d);
            else if(field.Value is BarcodeDateTime dateTime)
                ((resultedField.Value as BarcodeDateTime)!).DateTime.Should().Be(dateTime.DateTime);
            else
                (resultedField.Value).Should().BeEquivalentTo(field.Value);
        }
    }

    [Theory]
    [MemberData(nameof(ValidGs1Barcodes))]
    public void CanBuildBarcodeString(string expectedBarcode, GS1Barcode barcode)
    {
        //Arrange
        string? result = null;

        //Act
        Action parseAction = () => result = GS1BarcodeParserBuilder.Build(barcode);

        //Assert
        parseAction.Should().NotThrow();
        result.Should().Be(expectedBarcode);
    }

    public static TheoryData<string, GS1Barcode> ValidGs1ParsingBarcodes()
    {
        var gs1Barcode = new GS1Barcode(new GS1AimSymbologyIdentifier("e0"))
        {
            ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
            {
                productCode.Type = ProductCodeType.GTIN;
                productCode.Value = "357466145194";
                productCode.Indicator = 0;
            }),
            BatchNumber = null,
            SerialNumber = null,
            ExpirationDate = new TestBarcodeDateTime(new DateTime(2099, 12, 31), "991200", GS1DateFormat),
            ProductionDate = new TestBarcodeDateTime(new DateTime(2002, 05, 04), "020504", GS1DateFormat),
        };
        gs1Barcode.Fields["20"].SetValue("BL");
        gs1Barcode.Fields["240"].SetValue("40600199T");
        gs1Barcode.Fields["30"].SetValue(1);
        gs1Barcode.Fields["710"].SetValue("25862471");
        gs1Barcode.Fields["98"].SetValue("15647");
        gs1Barcode.Fields["99"].SetValue("15489");

        var randomOrderGs1Barcode = new GS1Barcode(new GS1AimSymbologyIdentifier("d2"))
        {
            ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
            {
                productCode.Type = ProductCodeType.GTIN;
                productCode.Value = "357466145194";
                productCode.Indicator = 0;
            }),
            BatchNumber = null,
            SerialNumber = null,
        };

        randomOrderGs1Barcode.Fields["20"].SetValue("BL");
        randomOrderGs1Barcode.Fields["240"].SetValue("40600199T");
        randomOrderGs1Barcode.Fields["30"].SetValue(1);
        randomOrderGs1Barcode.Fields["710"].SetValue("25862471");
        randomOrderGs1Barcode.Fields["98"].SetValue("15647");
        randomOrderGs1Barcode.Fields["99"].SetValue("15489");

        return new TheoryData<string, GS1Barcode>()
        {
            //Random Order #1
            {
                $"]d220BL0103574661451947301{GroupSeparator}9915489{GroupSeparator}9815647{GroupSeparator}24040600199T{GroupSeparator}71025862471",
                randomOrderGs1Barcode
            },

            //Random Order #2 (Original Motilium Package)
            {
                $"{GroupSeparator}010357466145194721118165795226{GroupSeparator}17210331101724847.1",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "357466145194";
                        productCode.Indicator = 0;
                    }),
                    BatchNumber = "1724847.1",
                    SerialNumber = "118165795226",
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2021, 03, 31), "210331", GS1DateFormat),
                    ProductionDate = null
                }
            },

            //GS ending
            {
                $"0134567890123457103456789{GroupSeparator}213456789-012{GroupSeparator}",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("34567890123457", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "456789012345";
                        productCode.Indicator = 3;
                    }),
                    BatchNumber = "3456789",
                    SerialNumber = "3456789-012"
                }
            },

            //QR Code Prefix + BatchNumber
            {
                $"]Q30134567890123457103456789",
                new GS1Barcode(new GS1AimSymbologyIdentifier("Q3"))
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("34567890123457", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "456789012345";
                        productCode.Indicator = 3;
                    }),
                    BatchNumber = "3456789",
                    SerialNumber = null,
                }
            },

            //DataMatrix Prefix + BatchNumber
            {
                $"]d20134567890123457103456789",
                new GS1Barcode(new GS1AimSymbologyIdentifier("d2"))
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("34567890123457", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "456789012345";
                        productCode.Indicator = 3;
                    }),
                    BatchNumber = "3456789",
                    SerialNumber = null,
                }
            },

            //DotCode Prefix + BatchNumber
            {
                $"]J10134567890123457103456789",
                new GS1Barcode(new GS1AimSymbologyIdentifier("J1"))
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("34567890123457", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "456789012345";
                        productCode.Indicator = 3;
                    }),
                    BatchNumber = "3456789",
                    SerialNumber = null,
                }
            },

            //GS1 example 1
            {
                "]d20108430215011539112212221724022021S3736",
                new GS1Barcode(new GS1AimSymbologyIdentifier("d2"))
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("08430215011539", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "843021501153";
                        productCode.Indicator = 0;
                        productCode.Code = "08430215011539";
                    }),
                    SerialNumber = "S3736",
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2024, 02, 20), "240220", GS1DateFormat),
                    ProductionDate = new TestBarcodeDateTime(new DateTime(2022, 12, 22), "221222", GS1DateFormat),
                }
            },

            //GS1 example 2
            {
                $"]e00103574661451947110205041799120020BL24040600199T{GroupSeparator}301{GroupSeparator}71025862471{GroupSeparator}9815647{GroupSeparator}9915489",
                gs1Barcode
            },
        };
    }

    public static TheoryData<string, GS1Barcode> ValidGs1Barcodes()
    {
        var gs1Barcode = new GS1Barcode()
        {
            ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
            {
                productCode.Type = ProductCodeType.GTIN;
                productCode.Value = "357466145194";
                productCode.Indicator = 0;
            }),
            BatchNumber = null,
            SerialNumber = null,
        };
        gs1Barcode.Fields["20"].SetValue("BL");
        gs1Barcode.Fields["240"].SetValue("40600199T");
        gs1Barcode.Fields["30"].SetValue(1);
        gs1Barcode.Fields["710"].SetValue("25862471");
        gs1Barcode.Fields["98"].SetValue("15647");
        gs1Barcode.Fields["99"].SetValue("15489");

        var gs1BarcodeDimension = new GS1Barcode()
        {
            ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
            {
                productCode.Type = ProductCodeType.GTIN;
                productCode.Value = "357466145194";
                productCode.Indicator = 0;
            }),
            BatchNumber = "1724847.1",
            SerialNumber = "118165795226",
            ExpirationDate = new TestBarcodeDateTime(new DateTime(2021, 03, 31), "210331", GS1DateFormat),
            ProductionDate = null,
            NetWeightInPounds = 3.54777d
        };
        gs1BarcodeDimension.Fields["311"].SetValue(1.23456); // Length in metres
        gs1BarcodeDimension.Fields["312"].SetValue(1234.56); // Width in metres
        gs1BarcodeDimension.Fields["313"].SetValue(0.01234); // Deepth in metres
        gs1BarcodeDimension.Fields["314"].SetValue(123456d); // Area in square metres

        var gs1Barcode40x = new GS1Barcode()
        {
            ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
            {
                productCode.Type = ProductCodeType.GTIN;
                productCode.Value = "357466145194";
                productCode.Indicator = 0;
            })
        };
        gs1Barcode40x.Fields["400"].SetValue("PO-20250612-ABC123");       // ORDER NUMBER
        gs1Barcode40x.Fields["401"].SetValue("GINC202506120001SHIPMENT"); // GINC
        gs1Barcode40x.Fields["402"].SetValue("91234567890123456");        // GSIN
        gs1Barcode40x.Fields["403"].SetValue("HUB-NYC-AREA5");            // ROUTE

        var gs1Barcode41x = new GS1Barcode()
        {
            ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
            {
                productCode.Type = ProductCodeType.GTIN;
                productCode.Value = "357466145194";
                productCode.Indicator = 0;
            })
        };
        gs1Barcode41x.Fields["410"].SetValue("10101xxx10101");
        gs1Barcode41x.Fields["412"].SetValue("1212121212lol");
        gs1Barcode41x.Fields["414"].SetValue("34343434asc43");
        gs1Barcode41x.Fields["417"].SetValue("1717171717inc");

        var gs1Barcode41x2 = new GS1Barcode()
        {
            ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
            {
                productCode.Type = ProductCodeType.GTIN;
                productCode.Value = "357466145194";
                productCode.Indicator = 0;
            }),
            BatchNumber = "3456789",
            SerialNumber = "54321"
        };
        gs1Barcode41x2.Fields["410"].SetValue("10101xxx10101");
        gs1Barcode41x2.Fields["412"].SetValue("1212121212lol");
        gs1Barcode41x2.Fields["414"].SetValue("34343434asc43");

        var gs1Barcode25x = new GS1Barcode()
        {
            ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
            {
                productCode.Type = ProductCodeType.GTIN;
                productCode.Value = "357466145194";
                productCode.Indicator = 0;
            })
        };
        gs1Barcode25x.Fields["250"].SetValue("t250");
        gs1Barcode25x.Fields["251"].SetValue("t251");
        gs1Barcode25x.Fields["253"].SetValue("t253");
        gs1Barcode25x.Fields["254"].SetValue("t254");
        gs1Barcode25x.Fields["255"].SetValue("t255");

        var gs1Barcode71x = new GS1Barcode()
        {
            ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
            {
                productCode.Type = ProductCodeType.GTIN;
                productCode.Value = "357466145194";
                productCode.Indicator = 0;
            })
        };
        gs1Barcode71x.Fields["710"].SetValue("PZNG12345");
        gs1Barcode71x.Fields["711"].SetValue("CIPF54321");
        gs1Barcode71x.Fields["712"].SetValue("CNESP0001");
        gs1Barcode71x.Fields["713"].SetValue("DRNBRA1234");
        gs1Barcode71x.Fields["714"].SetValue("AIMPT98765");
        gs1Barcode71x.Fields["715"].SetValue("NDCUS00001");
        gs1Barcode71x.Fields["716"].SetValue("AICITL0001");
        gs1Barcode71x.Fields["717"].SetValue("SRNCR0001");

        var gs1Barcode70x = new GS1Barcode()
        {
            ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
            {
                productCode.Type = ProductCodeType.GTIN;
                productCode.Value = "357466145194";
                productCode.Indicator = 0;
            })
        };
        gs1Barcode70x.Fields["7001"].SetValue(1987415681234);
        gs1Barcode70x.Fields["7002"].SetValue("PZNG12345");
        gs1Barcode70x.Fields["7003"].SetValue(new TestBarcodeDateTime(new DateTime(2025, 6, 15, 14, 30, 0), "202506151430", "yyMMdd"));
        gs1Barcode70x.Fields["7004"].SetValue(1234);
        gs1Barcode70x.Fields["7005"].SetValue("FAO27.3");
        gs1Barcode70x.Fields["7006"].SetValue(new TestBarcodeDateTime(new DateTime(2025, 1, 10), "250110", "yyMMdd"));
        gs1Barcode70x.Fields["7007"].SetValue(new TestBarcodeDateTimeRange(new TestBarcodeDateTime(new DateTime(2025, 1, 1), "250101", "yyMMdd"), new TestBarcodeDateTime(new DateTime(2025, 2, 28), "250228", "yyMMdd")));
        gs1Barcode70x.Fields["7008"].SetValue("COD");
        gs1Barcode70x.Fields["7009"].SetValue("TRAWL1");
        gs1Barcode70x.Fields["7010"].SetValue("02");
        gs1Barcode70x.Fields["7011"].SetValue(new TestBarcodeDateTime(new DateTime(2025, 6, 15), "250615", "yyMMdd"));
        gs1Barcode70x.Fields["7020"].SetValue("REFURB001");
        gs1Barcode70x.Fields["7021"].SetValue("FUNC01");
        gs1Barcode70x.Fields["7022"].SetValue("REV01");
        gs1Barcode70x.Fields["7023"].SetValue("GIAI0000001");
        gs1Barcode70x.Fields["7030"].SetValue("276ABCPROC");
        gs1Barcode70x.Fields["7031"].SetValue("840PROC1");
        gs1Barcode70x.Fields["7032"].SetValue("250FRPROC");
        gs1Barcode70x.Fields["7033"].SetValue("724ESPROC");
        gs1Barcode70x.Fields["7034"].SetValue("380ITPROC");
        gs1Barcode70x.Fields["7035"].SetValue("643RUPROC");
        gs1Barcode70x.Fields["7036"].SetValue("156CNPROC");
        gs1Barcode70x.Fields["7037"].SetValue("392JPPROC");
        gs1Barcode70x.Fields["7038"].SetValue("036AUPROC");
        gs1Barcode70x.Fields["7039"].SetValue("826GBPROC");
        gs1Barcode70x.Fields["7040"].SetValue("1ABC");
        gs1Barcode70x.Fields["7041"].SetValue("FUB1");

        var gs1Barcode72x = new GS1Barcode()
        {
            ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
            {
                productCode.Type = ProductCodeType.GTIN;
                productCode.Value = "357466145194";
                productCode.Indicator = 0;
            })
        };
        gs1Barcode72x.Fields["7230"].SetValue("ISICE1");
        gs1Barcode72x.Fields["7231"].SetValue("ISO001");
        gs1Barcode72x.Fields["7232"].SetValue("EUREF1");
        gs1Barcode72x.Fields["7233"].SetValue("USCER1");
        gs1Barcode72x.Fields["7234"].SetValue("GBSAF1");
        gs1Barcode72x.Fields["7235"].SetValue("DEGER1");
        gs1Barcode72x.Fields["7236"].SetValue("FRQUA1");
        gs1Barcode72x.Fields["7237"].SetValue("CNSTA1");
        gs1Barcode72x.Fields["7238"].SetValue("JPCER1");
        gs1Barcode72x.Fields["7239"].SetValue("AUVAL1");
        gs1Barcode72x.Fields["7240"].SetValue("PROTO001");
        gs1Barcode72x.Fields["7241"].SetValue("01");
        gs1Barcode72x.Fields["7242"].SetValue("VCN001");
        gs1Barcode72x.Fields["7250"].SetValue(new TestBarcodeDateTime(new DateTime(1990, 5, 15), "19900515", "yyyyMMdd"));
        gs1Barcode72x.Fields["7251"].SetValue(new TestBarcodeDateTime(new DateTime(1990, 5, 15, 9, 30, 0), "199005150930", "yyyyMMddHHmm"));
        gs1Barcode72x.Fields["7252"].SetValue(1);
        gs1Barcode72x.Fields["7253"].SetValue("SMITH");
        gs1Barcode72x.Fields["7254"].SetValue("JOHN");
        gs1Barcode72x.Fields["7255"].SetValue("JR");
        gs1Barcode72x.Fields["7256"].SetValue("JOHNSMITHJR");
        gs1Barcode72x.Fields["7257"].SetValue("12MAINST");
        gs1Barcode72x.Fields["7258"].SetValue("1/2");
        gs1Barcode72x.Fields["7259"].SetValue("JONES");

        return new TheoryData<string, GS1Barcode>()
        {
            //ProductCode + Unused AI's
            {
                $"010357466145194720BL24040600199T{GroupSeparator}301{GroupSeparator}71025862471{GroupSeparator}9815647{GroupSeparator}9915489",
                gs1Barcode
            },

            //ProductCode
            {
                $"0103574661451947",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "357466145194";
                        productCode.Indicator = 0;
                    }),
                    BatchNumber = null,
                    SerialNumber = null
                }
            },

            //BatchNumber
            {
                $"0134567890123457103456789",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("34567890123457", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "456789012345";
                        productCode.Indicator = 3;
                    }),
                    BatchNumber = "3456789",
                    SerialNumber = null
                }
            },

            //SerialNumber
            {
                $"0134567890123457103456789{GroupSeparator}213456789-012",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("34567890123457", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "456789012345";
                        productCode.Indicator = 3;
                    }),
                    BatchNumber = "3456789",
                    SerialNumber = "3456789-012"
                }
            },

            //Expiration Date
            {
                $"013456789012345717991200",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("34567890123457", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "456789012345";
                        productCode.Indicator = 3;
                    }),
                    BatchNumber = null,
                    SerialNumber = null,
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2099, 12, 31), "991200", GS1DateFormat)
                }
            },

            //Production Date
            {
                $"0134567890123457103456789{GroupSeparator}11020504213456789-012",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("34567890123457", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "456789012345";
                        productCode.Indicator = 3;
                    }),
                    BatchNumber = "3456789",
                    SerialNumber = "3456789-012",
                    ExpirationDate = null,
                    ProductionDate = new TestBarcodeDateTime(new DateTime(2002,05,04), "020504", GS1DateFormat)
                }
            },

            //Motilium Package (ordered)
            {
                $"0103574661451947101724847.1{GroupSeparator}1721033121118165795226",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "357466145194";
                        productCode.Indicator = 0;
                    }),
                    BatchNumber = "1724847.1",
                    SerialNumber = "118165795226",
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2021, 03, 31), "210331", GS1DateFormat),
                    ProductionDate = null
                }
            },

            //UPC-A/EAN12 NDC product in GTIN-14 with indicator 0
            {
                $"0100367457153032101724847.1{GroupSeparator}1721033121118165795226",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("00367457153032", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Schema = GtinProductScheme.NationalDrugCode;
                        productCode.Value = "6745715303";
                        productCode.Indicator = 0;
                    }),
                    BatchNumber = "1724847.1",
                    SerialNumber = "118165795226",
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2021, 03, 31), "210331", GS1DateFormat),
                    ProductionDate = null
                }
            },

            //UPC-A/EAN12 NDC product in GTIN-14 with indicator 1
            {
                $"0110304094903115101724847.1{GroupSeparator}1721033121118165795226",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("10304094903115", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Schema = GtinProductScheme.NationalDrugCode;
                        productCode.Value = "0409490311";
                        productCode.Indicator = 1;
                    }),
                    BatchNumber = "1724847.1",
                    SerialNumber = "118165795226",
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2021, 03, 31), "210331", GS1DateFormat),
                    ProductionDate = null
                }
            },

            //NetWeight in Kg
            {
                $"0103574661451947101724847.1{GroupSeparator}1721033121118165795226{GroupSeparator}3105354777",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "357466145194";
                        productCode.Indicator = 0;
                    }),
                    BatchNumber = "1724847.1",
                    SerialNumber = "118165795226",
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2021, 03, 31), "210331", GS1DateFormat),
                    ProductionDate = null,
                    NetWeightInKg = 3.54777d
                }
            },

            //NetWeight in Pounds
            {
                $"0103574661451947101724847.1{GroupSeparator}1721033121118165795226{GroupSeparator}3205354777",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "357466145194";
                        productCode.Indicator = 0;
                    }),
                    BatchNumber = "1724847.1",
                    SerialNumber = "118165795226",
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2021, 03, 31), "210331", GS1DateFormat),
                    ProductionDate = null,
                    NetWeightInPounds = 3.54777d
                }
            },

            //Price
            {
                $"0103574661451947101724847.1{GroupSeparator}1721033121118165795226{GroupSeparator}3929123456789012345",
                new GS1Barcode()
                {
                    ProductCode = TestProductCode.CreateProductCode<GtinProductCode>("03574661451947", (productCode) =>
                    {
                        productCode.Type = ProductCodeType.GTIN;
                        productCode.Value = "357466145194";
                        productCode.Indicator = 0;
                    }),
                    BatchNumber = "1724847.1",
                    SerialNumber = "118165795226",
                    ExpirationDate = new TestBarcodeDateTime(new DateTime(2021, 03, 31), "210331", GS1DateFormat),
                    ProductionDate = null,
                    Price = 123456.789012345d
                }
            },

            //Check multiple dimensions
            {
               $"0103574661451947101724847.1{GroupSeparator}1721033121118165795226{GroupSeparator}31151234563122123456313500123431401234563205354777",
                gs1BarcodeDimension
            },
            //Check prefix 40 AIs
            {
                $"0103574661451947400PO-20250612-ABC123{GroupSeparator}401GINC202506120001SHIPMENT{GroupSeparator}40291234567890123456403HUB-NYC-AREA5",
                gs1Barcode40x
            },
            //Check prefix 41 AIs
            {
                $"010357466145194741010101xxx101014121212121212lol41434343434asc434171717171717inc",
                gs1Barcode41x
            },
            {
                $"0103574661451947103456789{GroupSeparator}2154321{GroupSeparator}41010101xxx101014121212121212lol41434343434asc43",
                gs1Barcode41x2
            },
            //Check prefix 25 AIs
            {
                $"0103574661451947250t250{GroupSeparator}251t251{GroupSeparator}253t253{GroupSeparator}254t254{GroupSeparator}255t255",
                gs1Barcode25x
            },

            //test case for all 70xx AI's
            {
                $"0103574661451947700119874156812347002PZNG12345{GroupSeparator}7003202506151430700412347005FAO27.3{GroupSeparator}70062501107007250101250228{GroupSeparator}7008COD{GroupSeparator}" +
                $"7009TRAWL1{GroupSeparator}701002{GroupSeparator}7011250615{GroupSeparator}7020REFURB001{GroupSeparator}7021FUNC01{GroupSeparator}7022REV01{GroupSeparator}7023GIAI0000001{GroupSeparator}" +
                $"7030276ABCPROC{GroupSeparator}7031840PROC1{GroupSeparator}7032250FRPROC{GroupSeparator}7033724ESPROC{GroupSeparator}7034380ITPROC{GroupSeparator}7035643RUPROC{GroupSeparator}"+
                $"7036156CNPROC{GroupSeparator}7037392JPPROC{GroupSeparator}7038036AUPROC{GroupSeparator}7039826GBPROC{GroupSeparator}70401ABC7041FUB1",
                gs1Barcode70x
            },

            //Random 71x range test
            {
                $"0103574661451947710PZNG12345{GroupSeparator}711CIPF54321{GroupSeparator}712CNESP0001{GroupSeparator}713DRNBRA1234{GroupSeparator}714AIMPT98765{GroupSeparator}715NDCUS00001{GroupSeparator}716AICITL0001{GroupSeparator}717SRNCR0001",
                gs1Barcode71x
            },

            //test case for all 72xx AIs
            {
                $"0103574661451947" +
                $"7230ISICE1{GroupSeparator}7231ISO001{GroupSeparator}7232EUREF1{GroupSeparator}7233USCER1{GroupSeparator}7234GBSAF1{GroupSeparator}7235DEGER1{GroupSeparator}" +
                $"7236FRQUA1{GroupSeparator}7237CNSTA1{GroupSeparator}7238JPCER1{GroupSeparator}7239AUVAL1{GroupSeparator}7240PROTO001{GroupSeparator}724101" +
                $"7242VCN001{GroupSeparator}725019900515" +
                $"7251199005150930725217253SMITH{GroupSeparator}7254JOHN{GroupSeparator}7255JR{GroupSeparator}7256JOHNSMITHJR{GroupSeparator}725712MAINST{GroupSeparator}72581/27259JONES",
                gs1Barcode72x
            },
        };
    }

    [Theory]
    [MemberData(nameof(InValidGs1Barcodes))]
    public void InvalidBarcodeStringThrowsException(string barcode, string expectedMessage)
    {
        //Arrange & Act
        var parsed = GS1BarcodeParserBuilder.TryParse(barcode, null, out var result);
        Action parseAction = () => GS1BarcodeParserBuilder.Parse(barcode, null);

        //Assert
        parsed.Should().BeFalse();
        result.Should().BeNull();
        parseAction.Should()
            .Throw<GS1ParseException>()
            .WithMessage(expectedMessage);
    }

    public static TheoryData<string, string> InValidGs1Barcodes() => new()
    {
        //ProductCode Too Short
        {
            $"01911972534034{GroupSeparator}103456789",
            $"Failed to parse GS1 Barcode :{Environment.NewLine}01 : Invalid value Length 12. Expected 14 Bytes."
        },

        //Invalid ProductCode
        {
            $"019119725340342717991200213456789-012{GroupSeparator}103456789",
            $"Failed to parse GS1 Barcode :{Environment.NewLine}01 : Invalid GTIN/EAN CheckDigit '7', Expected '8'."
        },

        //Missing AI
        {
            $"0191197253403428ABG3456789-012{GroupSeparator}103456789",
            $"Failed to parse GS1 Barcode :{Environment.NewLine}Invalid character detected in AI 'AB'."
        },

        //Random Character
        {
            $"X019119725340342817991200213456789-012{GroupSeparator}103456789",
            $"Failed to parse GS1 Barcode :{Environment.NewLine}Invalid character detected in AI 'X0'."
        },

        //Invalid Production Date
        {
            $"019119725340342817991200213456789-012{GroupSeparator}103456789{GroupSeparator}110BOGUS",
            $"Failed to parse GS1 Barcode :{Environment.NewLine}11 : Invalid GS1 Date value '0BOGUS'."
        },

        //Invalid Expiration Date
        {
            $"0191197253403428170BOGUS213456789-012{GroupSeparator}103456789{GroupSeparator}",
            $"Failed to parse GS1 Barcode :{Environment.NewLine}17 : Invalid GS1 Date value '0BOGUS'."
        },

        //Invalid Batch String
        {
            $"0191197253403428213456789-012{GroupSeparator}1034#|56789{GroupSeparator}",
            $"Failed to parse GS1 Barcode :{Environment.NewLine}10 : Invalid GS1 string value '34#|56789'."
        },

        //Batch too long
        {
            $"0191197253403428213456789-012{GroupSeparator}10001189998819991197253{GroupSeparator}",
            $"Failed to parse GS1 Barcode :{Environment.NewLine}10 : Invalid value Length 21. Expected Max 20 Bytes."
        },

        //Invalid SerialNumber
        {
            $"01911972534034282134^µ56789{GroupSeparator}",
            $"Failed to parse GS1 Barcode :{Environment.NewLine}21 : Invalid GS1 string value '34^µ56789'."
        },

        //SerialNumber too Long
        {
            $"01911972534034282134567890ABCDE+)97+-ER{GroupSeparator}",
            $"Failed to parse GS1 Barcode :{Environment.NewLine}21 : Invalid value Length 21. Expected Max 20 Bytes."
        },

        //Random Fields Contains Invalid string character
        {
            $"019119725340342899#$^248BFGD^{GroupSeparator}",
            $"Failed to parse GS1 Barcode :{Environment.NewLine}99 : Invalid GS1 string value '#$^248BFGD^'."
        },

        //invalid UPC/EAN
        {
            $"300450549108",
            $"Failed to parse GS1 Barcode :{Environment.NewLine}30 : Invalid value Length 10. Expected Max 8 Bytes."
        },
    };
}
