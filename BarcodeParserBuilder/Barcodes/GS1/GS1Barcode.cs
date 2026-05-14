using BarcodeParserBuilder.Exceptions.GS1;

namespace BarcodeParserBuilder.Barcodes.GS1;

public class GS1Barcode(AimSymbologyIdentifier? symbologyIdentifier) : Barcode(symbologyIdentifier)
{
    internal const char GroupSeparator = (char)0x1D;

    public GS1Barcode() : this(null) { }

    public override BarcodeType BarcodeType => BarcodeType.GS1;
    protected override FieldCollection BarcodeFields { get; } =
    [
        // GS1 Application Identifiers starting with digit 0
        new FixedLengthGS1Field("00", 18),                      // Identification of a logistic unit (SSCC
        new FixedLengthGS1Field<ProductCode?>("01", 14),        // Identification of a trade item (GTIN
        new FixedLengthGS1Field<ProductCode?>("02", 14),        // Identification of trade items contained in a logistic unit

        // GS1 Application Identifiers starting with digit 1
        new GS1Field("10", 20),                                 // Batch or lot number
        new DateTimeGS1Field("11"),     // Production date
        new DateTimeGS1Field("12"),     // Due date for amount on payment slip
        new DateTimeGS1Field("13"),     // Packaging date
        new DateTimeGS1Field("15"),     // Best before date
        new DateTimeGS1Field("16"),     // Sell by date
        new DateTimeGS1Field("17"),     // Expiration date

        // GS1 Application Identifiers starting with digit 2
        new FixedLengthGS1Field("20", 2),                       // Internal product variant
        new GS1Field("21", 20),                                 // Serial number
        new GS1Field("22", 20),                                 // Consumer product variant
        new GS1Field("235", 28),                                // Third Party Controlled, Serialised Extension of Global Trade Item Number (GTIN) (TPX)
        new GS1Field("240", 30),                                // Additional product identification assigned by the manufacturer
        new GS1Field("241", 30),                                // Customer part number
        new GS1Field("242", 6),                                 // Made-to-Order variation number
        new GS1Field("243", 20),                                // Packaging component number
        new GS1Field("250", 30),                                // Secondary serial number
        new GS1Field("251", 30),                                // Reference to source entity
        new GS1Field("253", 30),                                // Global Document Type Identifier (GDTI)
        new GS1Field("254", 20),                                // Global Location Number (GLN) extension component
        new GS1Field("255", 25),                                // Global Coupon Number (GCN)

        // GS1 Application Identifiers starting with digit 3
        new GS1Field<int?>("30", 8),                           // Variable count of items
        // Trade measures: AIs (31nn, 32nn, 35nn, 36nn)
        // Logistic measures: AIs (33nn, 34nn, 35nn, 36nn)
        new FixedLengthGS1Field<double?>("310", 7),
        new FixedLengthGS1Field<double?>("311", 7),
        new FixedLengthGS1Field<double?>("312", 7),
        new FixedLengthGS1Field<double?>("313", 7),
        new FixedLengthGS1Field<double?>("314", 7),
        new FixedLengthGS1Field<double?>("315", 7),
        new FixedLengthGS1Field<double?>("316", 7),
        new FixedLengthGS1Field<double?>("320", 7),
        new FixedLengthGS1Field<double?>("321", 7),
        new FixedLengthGS1Field<double?>("322", 7),
        new FixedLengthGS1Field<double?>("323", 7),
        new FixedLengthGS1Field<double?>("324", 7),
        new FixedLengthGS1Field<double?>("325", 7),
        new FixedLengthGS1Field<double?>("326", 7),
        new FixedLengthGS1Field<double?>("327", 7),
        new FixedLengthGS1Field<double?>("328", 7),
        new FixedLengthGS1Field<double?>("329", 7),
        new FixedLengthGS1Field<double?>("330", 7),
        new FixedLengthGS1Field<double?>("331", 7),
        new FixedLengthGS1Field<double?>("332", 7),
        new FixedLengthGS1Field<double?>("333", 7),
        new FixedLengthGS1Field<double?>("334", 7),
        new FixedLengthGS1Field<double?>("335", 7),
        new FixedLengthGS1Field<double?>("336", 7),
        new FixedLengthGS1Field<double?>("337", 7),
        new FixedLengthGS1Field<double?>("340", 7),
        new FixedLengthGS1Field<double?>("341", 7),
        new FixedLengthGS1Field<double?>("342", 7),
        new FixedLengthGS1Field<double?>("343", 7),
        new FixedLengthGS1Field<double?>("344", 7),
        new FixedLengthGS1Field<double?>("345", 7),
        new FixedLengthGS1Field<double?>("346", 7),
        new FixedLengthGS1Field<double?>("347", 7),
        new FixedLengthGS1Field<double?>("348", 7),
        new FixedLengthGS1Field<double?>("349", 7),
        new FixedLengthGS1Field<double?>("350", 7),
        new FixedLengthGS1Field<double?>("351", 7),
        new FixedLengthGS1Field<double?>("352", 7),
        new FixedLengthGS1Field<double?>("353", 7),
        new FixedLengthGS1Field<double?>("354", 7),
        new FixedLengthGS1Field<double?>("355", 7),
        new FixedLengthGS1Field<double?>("356", 7),
        new FixedLengthGS1Field<double?>("357", 7),
        new FixedLengthGS1Field<double?>("360", 7),
        new FixedLengthGS1Field<double?>("361", 7),
        new FixedLengthGS1Field<double?>("362", 7),
        new FixedLengthGS1Field<double?>("363", 7),
        new FixedLengthGS1Field<double?>("364", 7),
        new FixedLengthGS1Field<double?>("365", 7),
        new FixedLengthGS1Field<double?>("366", 7),
        new FixedLengthGS1Field<double?>("367", 7),
        new FixedLengthGS1Field<double?>("368", 7),
        new FixedLengthGS1Field<double?>("369", 7),
        new GS1Field("37"),                                     // Count of trade items or trade item pieces contained in a logistic unit
        new GS1Field("390"),                                    // Amount payable or coupon value - Single monetary area: AI (390n)
        new GS1Field("391"),                                    // Amount payable and ISO currency code: AI (391n)
        new GS1Field<double?>("392", 1, 16),                    // Amount payable for a variable measure trade item – Single monetary area: AI (392n)
        new GS1Field("393"),                                    // Amount payable for a variable measure trade item and ISO currency code: AI (393n)
        new GS1Field("394"),                                    // Percentage discount of a coupon: AI (394n)
        new GS1Field("395"),                                    // Amount payable per unit of measure single monetary area (variable measure trade item): AI (395n)

        // GS1 Application Identifiers starting with digit 4
        new GS1Field("400", 30),                                // Customer’s purchase order number
        new GS1Field("401", 30),                                // Global Identification Number for Consignment (GINC)
        new FixedLengthGS1Field("402", 17),                     // Global Shipment Identification Number (GSIN)
        new GS1Field("403", 30),                                // Routing code
        new FixedLengthGS1Field("410", 13),                     // Ship to - Deliver to Global Location Number (GLN)
        new FixedLengthGS1Field("411", 13),                     // Bill to - Invoice to Global Location Number (GLN)
        new FixedLengthGS1Field("412", 13),                     // Purchased from Global Location Number (GLN)
        new FixedLengthGS1Field("413", 13),                     // Ship for - Deliver for - Forward to Global Location Number (GLN)
        new FixedLengthGS1Field("414", 13),                     // Identification of a physical location - Global Location Number (GLN)
        new FixedLengthGS1Field("415", 13),                     // Global Location Number (GLN) of the invoicing party
        new FixedLengthGS1Field("416", 13),                     // Global Location Number (GLN) of the production or service location
        new FixedLengthGS1Field("417", 13),                     // Party Global Location Number (GLN)
        new GS1Field("42"),
        new GS1Field("43"),

        // GS1 Application Identifiers starting with digit 7

        new FixedLengthGS1Field<Int64?>("7001", 13),            //NATO Stock Number (NSN)
        new GS1Field("7002", 30),                               //UNECE meat carcasses classification
        new DateTimeGS1Field("7003", BarcodeDateTime.GS1DateTimeLongFormat), //Expiration date and time
        new FixedLengthGS1Field<int?>("7004", 4),               //Active Potency
        new GS1Field("7005", 12),                               //Catch Area
        new DateTimeGS1Field("7006"),                           //First freeze date
        new GS1Field<BarcodeDateTimeRange>("7007", 6, 12, $"{BarcodeDateTime.GS1DateShortFormat}{BarcodeDateTimeRange.DateTimeFormatSeparator}{BarcodeDateTime.GS1DateShortFormat}"), //Harvest Date date
        new GS1Field("7008", 3),                                // Species for fishery (FAO code)
        new GS1Field("7009", 10),                               // Fishing gear type
        new GS1Field("7010", 2),                                // Production method
        new GS1Field<BarcodeDateTime?>("7011", 6, 10, BarcodeDateTime.GS1DateOptionalTimeFormat),          // Test by date (YYMMDD[hhmm]) | Only 6 or 10 chars valid; optional time component. see 7003

        new GS1Field("7020", 20),                               // Refurbishment lot ID
        new GS1Field("7021", 20),                               // Functional status
        new GS1Field("7022", 20),                               // Revision status
        new GS1Field("7023", 30),                               // GIAI of an assembly
        new GS1Field("7030", 4, 30),                            // Processor #0 (ISO 3166-1 country + processor ID)
        new GS1Field("7031", 4, 30),                            // Processor #1
        new GS1Field("7032", 4, 30),                            // Processor #2
        new GS1Field("7033", 4, 30),                            // Processor #3
        new GS1Field("7034", 4, 30),                            // Processor #4
        new GS1Field("7035", 4, 30),                            // Processor #5
        new GS1Field("7036", 4, 30),                            // Processor #6
        new GS1Field("7037", 4, 30),                            // Processor #7
        new GS1Field("7038", 4, 30),                            // Processor #8
        new GS1Field("7039", 4, 30),                            // Processor #9
        new FixedLengthGS1Field("7040", 4),                     // GS1 UIC with Extension 1 and Importer index
        new GS1Field("7041", 4),                                // UN/CEFACT freight unit type

        new GS1Field("710", 20),                                //NHRN – Germany PZN
        new GS1Field("711", 20),                                //NHRN – France CIP
        new GS1Field("712", 20),                                //NHRN – Spain CN
        new GS1Field("713", 20),                                //NHRN – Brasil DRN
        new GS1Field("714", 20),                                //NHRN – Portugal AIM
        new GS1Field("715", 20),                                //NHRN – USA NDC
        new GS1Field("716", 20),                                //NHRN – Italy AIC
        new GS1Field("717", 20),                                //NHRN – Costa Rica SRN

        // GS1 Application Identifiers starting with 72
        new GS1Field("7230", 3, 30, null),                      // Certification reference #0
    new GS1Field("7231", 3, 30, null),                          // Certification reference #1
        new GS1Field("7232", 3, 30, null),                      // Certification reference #2
        new GS1Field("7233", 3, 30, null),                      // Certification reference #3
        new GS1Field("7234", 3, 30, null),                      // Certification reference #4
        new GS1Field("7235", 3, 30, null),                      // Certification reference #5
        new GS1Field("7236", 3, 30, null),                      // Certification reference #6
        new GS1Field("7237", 3, 30, null),                      // Certification reference #7
        new GS1Field("7238", 3, 30, null),                      // Certification reference #8
        new GS1Field("7239", 3, 30, null),                      // Certification reference #9
        new GS1Field("7240", 20),                               // Protocol ID
        new FixedLengthGS1Field("7241", 2),                     // AIDC media type
        new GS1Field("7242", 25),                               // Version Control Number (VCN)
        new DateTimeGS1Field("7250", BarcodeDateTime.GS1DateLongFormat),         // Date of birth
        new DateTimeGS1Field("7251", BarcodeDateTime.GS1DateTimeLongFormat),     // Date and time of birth
        new FixedLengthGS1Field<int?>("7252", 1),               // Biological sex
        new GS1Field("7253", 40),                               // Family name of person
        new GS1Field("7254", 40),                               // Given name of person
        new GS1Field("7255", 10),                               // Name suffix of person
        new GS1Field("7256", 90),                               // Full name of person
        new GS1Field("7257", 70),                               // Address of person
        new FixedLengthGS1Field("7258", 3),                     // Baby birth sequence
        new GS1Field("7259", 40),                               // Baby of family name

        // GS1 Application Identifiers starting with digit 8
        new GS1Field("80"),
        new GS1Field("81"),
        new GS1Field("82"),

        // GS1 Application Identifiers starting with digit 9
        new GS1Field("90", 30),                                 // Information mutually agreed between trading partners
        // Company internal information: AIs (91 - 99)
        new GS1Field("91", 90),
        new GS1Field("92", 90),
        new GS1Field("93", 90),
        new GS1Field("94", 90),
        new GS1Field("95", 90),
        new GS1Field("96", 90),
        new GS1Field("97", 90),
        new GS1Field("98", 90),
        new GS1Field("99", 90),
    ];

    public override AimSymbologyIdentifier? ReaderInformation { get; protected set; }

    public override ProductCode? ProductCode
    {
        get => (ProductCode?)BarcodeFields["01"].Value;
        set => BarcodeFields["01"].SetValue(value);
    }
    public override BarcodeDateTime? ProductionDate
    {
        get => (BarcodeDateTime?)BarcodeFields["11"].Value;
        set => BarcodeFields["11"].SetValue(value);
    }
    public override BarcodeDateTime? ExpirationDate
    {
        get => (BarcodeDateTime?)(BarcodeFields["17"].Value ?? BarcodeFields["7003"].Value);
        set
        {
            if(BarcodeFields["7003"].Value != null)
                BarcodeFields["7003"].SetValue(value);
            else
                BarcodeFields["17"].SetValue(value);
        }
    }

    public override string? BatchNumber
    {
        get => string.IsNullOrWhiteSpace((string?)BarcodeFields["10"].Value) ? null : (string?)BarcodeFields["10"].Value;
        set => BarcodeFields["10"].SetValue(value);
    }
    public override string? SerialNumber
    {
        get => string.IsNullOrWhiteSpace((string?)BarcodeFields["21"].Value) ? null : (string?)BarcodeFields["21"].Value;
        set => BarcodeFields["21"].SetValue(value);
    }

    public double? NetWeightInKg
    {
        get => (double?)BarcodeFields["310"].Value;
        set => BarcodeFields["310"].SetValue(value);
    }

    public double? NetWeightInPounds
    {
        get => (double?)BarcodeFields["320"].Value;
        set => BarcodeFields["320"].SetValue(value);
    }

    public double? Price
    {
        get => (double?)BarcodeFields["392"].Value;
        set => BarcodeFields["392"].SetValue(value);
    }
}

internal class GS1Field<T> : BarcodeField<T>
{
    public GS1Field(string identifier, int? maxLength = null) : base(BarcodeType.GS1, identifier, 1, maxLength ?? 90, null) { }
    public GS1Field(string identifier, int minLength, int maxLength) : base(BarcodeType.GS1, identifier, minLength, maxLength, null) { }
    public GS1Field(string identifier, int minLength, int maxLength, string? format) : base(BarcodeType.GS1, identifier, minLength, maxLength, format) { }
    public override void Parse(StringReader codeStream)
    {
        if (MinLength <= 0)
            throw new GS1ParseException($"{Identifier} : Invalid Field size.");

        var value = "";
        while (codeStream.Peek() > -1 && codeStream.Peek() != GS1Barcode.GroupSeparator)
        {
            value += (char)codeStream.Read();

            if (FixedLength && value.Length == MinLength)
                break;
        }

        if (value.Any(c => c == GS1Barcode.GroupSeparator))
            throw new GS1ParseException($"{Identifier} : Invalid GS1 value : value contains a group separator");

        try
        {
            Parse(value);
        }
        catch (Exception e)
        {
            throw new GS1ParseException($"{Identifier} : {e.Message}", e);
        }
    }
}

internal class FixedLengthGS1Field<T> : GS1Field<T>
{
    public FixedLengthGS1Field(string identifier, int length) : base(identifier, length, length, null) { }
    public FixedLengthGS1Field(string identifier, int length, string? format) : base(identifier, length, length, format) { }
}

internal class DateTimeGS1Field(string identifier, string format) : FixedLengthGS1Field<BarcodeDateTime?>(identifier, format.Length, format)
{
    internal DateTimeGS1Field(string identifier) : this(identifier, BarcodeDateTime.GS1DateShortFormat) { }
}

internal class GS1Field : GS1Field<string?>
{
    public GS1Field(string identifier, int? maxLength = null) : base(identifier, maxLength) { }
    public GS1Field(string identifier, int minLength, int maxLength) : base(identifier, minLength, maxLength) { }
    public GS1Field(string identifier, int minLength, int maxLength, string? format) : base(identifier, minLength, maxLength, format) { }
}

internal class FixedLengthGS1Field : FixedLengthGS1Field<string?>
{
    public FixedLengthGS1Field(string identifier, int length) : base(identifier, length, null) { }
    public FixedLengthGS1Field(string identifier, int length, string? format) : base(identifier, length, format) { }
}
