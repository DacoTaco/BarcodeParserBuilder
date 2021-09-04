# BarcodeParserBuilder  
.NET library to parse and build barcode strings.  
  
The library allows .NET applications to throw any barcode string to it and receive an class object back that contains all the barcode data like  
type, product code , product code type/schema, expiration date etc etc...  
  
The library also verifies all data when parsing & setting properties so that it follows the specifications of the barcode type.  
A barcode can be parsed and build in less than 4 lines of code.  
  
Currently supports GS1, GS1-128, EAN/UPC, PPN, MSI & HIBC.  
  
When combined with barcode imaging projects like Zxing you can scan, parse, build & create any barcode to your needs.  
  
## Parsing Example  
The following code  
```C#
bool isSuccessful = ParserBuilder.TryParse($"0134567890123457103456789{(char)0x1D}213456789-012", out Barcode barcode);  
Console.WriteLine($"The barcode was {(isSuccessful ? "parsed" : "unparsed")}!");  
Console.WriteLine($"Barcode Type : {barcode.BarcodeType}");  
Console.WriteLine($"Object Type : {barcode.GetType().Name}");  
Console.WriteLine($"Product Code Schema : {barcode.ProductCode.Schema}");  
Console.WriteLine($"Product Code : {barcode.ProductCode.Value}");  
Console.WriteLine($"Batch : {barcode.BatchNumber}");  
Console.WriteLine($"Serial : {barcode.SerialNumber}");
```

will result in the following output : 
> The barcode was parsed!  
> Barcode Type : GS1  
> Object Type : GS1Barcode  
> Product Code Schema : GTIN  
> Product Code : 34567890123457  
> Batch : 3456789  
> Serial : 3456789-012 
  
The returned 'barcode' object can be cast into a 'GS1Barcode' object, which allows for GS1 specific content to be accessed since the example is a GS1 barcode.  
This is because all barcode classes are derrived from the barcode class.  
  
## Building Example  
The following code  
```C#
GS1Barcode barcode = New GS1Barcode();  
barcode.ProductCode = ProductCode.ParseGtin("34567890123457");  
barcode.BatchNumber = "3456789";  
barcode.SerialNumber = "3456789-012";  
string barcodeString = ParserBuilder.Build(barcode);  
Console.WriteLine($"barcode String : {barcodeString}");  
```

Will result in the following output : 
> barcode String : 0134567890123457103456789\<gs>213456789-012  
  
Note that the '\<gs>' in the string is the GS1 group separator and is in reality the actual character(0x1D), which is a none-readable character.  
This is why i placed '\<gs>' in the example.  
  
## TODO : 
Our Current Todo exists of adding support for the following barcodes : 
* ISBT 128
  
## Licensing & Rules  
The project is released under the LGPL license. In short that means that any project can use this library or its released packages for any usage (including commercial usage).  
However, Any alterations done need to be open source or publically mentioned what has been changed.  
I also kindly ask that any alterations are merged back into the main repository so that anyone can benefit from the changes/added functionality.  
This is because we are still lacking a lot of functionality (like HIBC parsed into PPN as to the HIBC 2.4 standard) and everyone could benefit from that.  
If you would like me to look into adding a barcode type , don't hesitate to create an issue or pull request!  