namespace dMobile.BarCodeScanner
{
    using System;
    using dMobile.Bedrock;
    using System.Collections.Generic;

    [Serializable]
    public abstract class BarCodeParse : IDisposable
    {
        #region Fields

        /// <summary>
        /// Character used to identify the ending of a variable length AI.
        /// </summary>
        /// <remarks>0x1D is the group separator character</remarks>
        private static readonly short GroupSeparator = 29;

        /// <summary>
        /// Bar code settings xml file.
        /// </summary>
        /// <remarks>Contains parsing rules.</remarks>
        private static SysBarCodeSettings settings = new SysBarCodeSettings();

        /// <summary>
        /// Bar code raw data read, direct from the scan engine.
        /// </summary>
        /// <remarks>Contains the symbology characters</remarks>
        private static string raw = string.Empty;

        /// <summary>
        /// Bar code symbology, enumeration type.
        /// </summary>
        private static SysCommon.BarCodeSymbologies symbology = SysCommon.BarCodeSymbologies.UNDEFINED;

        /// <summary>
        /// Bar code symbology characters.
        /// </summary>
        /// <remarks>The symbology prefix is usually the first 3 characters
        /// taken from the raw data.</remarks>
        private static string symbologyPrefix = string.Empty;

        /// <summary>
        /// Bar code data, with the symbology prefix removed.
        /// </summary>
        private static string data = string.Empty;

        /// <summary>
        /// Portion of a bar code used to identify a product.
        /// </summary>
        private static string productIdentifier = string.Empty;

        /// <summary>
        /// Bar code data that could not be parsed / identified.
        /// Remainder after the parse code has be executed.
        /// </summary>
        private static string remainder = string.Empty;

        /// <summary>
        /// Bar code length of parsable data.
        /// </summary>
        /// <remarks>The symbology prefix length is not included just the parsable data.
        /// Decided to make this a property instead of checking the length of this.data,
        /// because this.data can be altered or shortened during parsing.</remarks>
        private static int length = 0;

        /// <summary>
        /// SSCC18 bar code data component.
        /// </summary>
        private static int packagingType = 0;

        /// <summary>
        /// Used in identifying the type of UPC code.
        /// </summary>
        private static int numberSystemCharacter = 0;

        /// <summary>
        /// GTIN company prefix.
        /// </summary>
        private static string companyPrefix = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        private static string itemReference = string.Empty;

        /// <summary>
        /// SSCC18 serial number.
        /// </summary>
        private static long manufacturerSerialNumber = 0;

        /// <summary>
        /// SSCC (Serial Shipping Container Code) N2+N18 SSCC: AI (00)
        /// </summary>
        private static string sscc18 = string.Empty;

        /// <summary>
        /// Identification of a Fixed Measure Trade Item (GTIN): AI (01)
        /// </summary>
        /// <remarks> The Application Identifier (01) indicates that the GS1 Application Identifier 
        /// data field contains a GTIN. The GTIN is used to identify trade items
        /// The GTIN for Fixed Measure Trade Items may be a GTIN-8, GTIN-12, GTIN-13 or a GTIN-14.
        /// The Check Digit verification, which must be carried out in the application software, 
        /// ensures that the number is correctly composed.</remarks>
        private static string gtin = string.Empty;

        /// <summary>
        /// GTIN of Contained Trade Items N2+N14 CONTENT: AI (02)
        /// </summary>
        /// <remarks>
        /// Identification of Fixed Measure Trade Items Contained in a Logistic Unit:
        /// AI (02)
        /// Identification of Variable Measure Trade Items Contained in a Logistic
        /// Unit: AI (02)
        /// The Application Identifier (02) indicates that the GS1 Application Identifier 
        /// data field includes the GTIN of the contained trade items. 
        /// Indicator digit 9 is required for a GTIN-14 used for Variable Measure Trade Items.
        /// Note: This Element String may be used only on a logistic unit that is not itself 
        /// a trade item and if all trade items that are contained at the same level have 
        /// the same GTIN. If the trade items are Variable Measure Trade Items, then this GTIN 
        /// will be the implied item number that does not appear on the items contained.
        /// The Application Identifier (02) indicates that the GS1 Application Identifier 
        /// data field includes the GTIN of the contained trade items.
        /// Note: This Element String may be used only on a logistic unit that is not itself 
        /// a trade item and if all trade items that are contained at the same level have 
        /// the same GTIN.
        /// The GTIN of the trade items contained represents the identification number of the 
        /// highest level of trade item contained in the logistic unit.
        /// The Check Digit verification, which must be carried out in the application software, 
        /// ensures that the number is correctly composed.</remarks>
        private static string gtinLogisticUnit = string.Empty;

        /// <summary>
        /// Serial Number: AI (21)
        /// </summary>
        /// <remarks>The Application Identifier (21) indicates that the GS1 Application Identifier 
        /// data field contains a serial number. A serial number is assigned to an entity 
        /// for its lifetime. When combined with a GTIN, a serial number uniquely identifies 
        /// an individual item. The serial number field is alphanumeric.
        /// The data transmitted from the bar code reader means that the Element String 
        /// denoting a serial number has been captured. As this Element String is an 
        /// attribute of a trade item, it must be processed together with the GTIN of the 
        /// trade item to which it relates.</remarks>
        private static string singleTradeItemSerialNumber = string.Empty;

        /// <summary>
        /// Batch or Lot Number: AI (10)
        /// </summary>
        /// <remarks>The Application Identifier (10) indicates that the GS1 Application Identifier 
        /// data field contains a batch or lot number. The batch or lot number associates
        /// an item with information the manufacturer considers relevant for traceability 
        /// of the trade item to which the Element String is applied. 
        /// The data may refer to the trade item itself or to items contained. 
        /// The number may be, for example, a production lot number, a shift number,
        /// a machine number, a time, or an internal production code. The data is alphanumeric.</remarks>
        private static string logisticUnitBatchLotNumber = string.Empty;

        /// <summary>
        /// Best Before Date: AI (15)
        /// </summary>
        /// <remarks>The Application Identifier (15) indicates that the GS1 Application Identifier 
        /// data fields contain a best before date. The best before date indicates the ideal 
        /// consumption or best effective use date of a product. 
        /// It is a statement about quality. It is often referred to as a sell by date or a
        /// minimum durability date.
        /// The structure is:
        /// Year: the tens and units of the year (e.g., 2003 = 03), which is mandatory
        /// Month: the number of the month (e.g., January = 01), which is mandatory
        /// Day: the number of the day of the relevant month (e.g., second day = 02); 
        /// if it is not necessary to specify the day, the field must be filled with two zeros</remarks>
        private static DateTime logisticUnitBestBeforeSellByDate = new DateTime(1900, 1, 1);

        /// <summary>
        /// Expiration Date: AI (17)
        /// </summary>
        /// <remarks>The Application Identifier (17) indicates that the GS1 Application Identifier 
        /// data fields contain an expiration date. The expiration date is the date that 
        /// determines the limit of consumption or use of a product. 
        /// Its meaning is determined based on the trade item context (e.g., for food, the date
        /// will indicate the possibility of a direct health risk resulting from use of the 
        /// product after the date, for pharmaceutical products, it will indicate the 
        /// possibility of an indirect health risk resulting from the ineffectiveness of 
        /// the product after the date). It is often referred to as "use by date" or 
        /// "maximum durability date.”
        /// The structure is:
        /// Year: the tens and units of the year (e.g., 2003 = 03), which is mandatory
        /// Month: the number of the month (e.g., January = 01), which is mandatory
        /// Day: the number of the day of the relevant month (e.g., second day = 02); 
        /// if it is not necessary to specify the day, the field must be filled with two zeros</remarks>
        private static DateTime logisticUnitExpiryUseByDate = new DateTime(1900, 1, 1);

        /// <summary>
        /// Production Date: AI (11)
        /// </summary>
        /// <remarks>The Application Identifier (11) indicates that the GS1 Application Identifier 
        /// data field contains a production date. The production date is the production 
        /// or assembly date determined by the manufacturer. 
        /// The date may refer to the trade item itself or to items contained.
        /// The structure is:
        /// Year: the tens and units of the year (e.g., 2003 = 03), which is mandatory
        /// Month: the number of the month (e.g., January = 01), which is mandatory
        /// Day: the number of the day of the relevant month (e.g., second day = 02); 
        /// if it is not necessary to specify the day, the field must be filled with two zeros</remarks>
        private static DateTime logisticUnitProductionDate = new DateTime(1900, 1, 1);

        /// <summary>
        /// Due Date for Amount on Payment Slip: AI (12)
        /// </summary>
        /// <remarks>The Application Identifier (12) indicates that the GS1 Application Identifier 
        /// data field contains the date by which an invoice should be paid. 
        /// This data element represents an attribute of a payment slip reference number, 
        /// AI (8020), and a Global Location Number (GLN) of the invoicing party.
        /// The structure is:
        /// Year: the tens and units of the year (e.g., 2003 = 03), which is mandatory
        /// Month: the number of the month (e.g., January = 01), which is mandatory
        /// Day: the number of the day of the relevant month (e.g., second day = 02); 
        /// if it is not necessary to specify the day, the field must be filled with two zeros</remarks>
        private static DateTime logisticUnitDueDate = new DateTime(1900, 1, 1);

        /// <summary>
        /// Packaging Date: AI (13)
        /// </summary>
        /// <remarks>The Application Identifier (13) indicates that the GS1 Application Identifier 
        /// data fields contain a packaging date. The packaging date is the date when 
        /// the goods were packed as determined by the packager. 
        /// The date may refer to the trade item itself or to items contained.
        /// The structure is:
        /// Year: the tens and units of the year (e.g., 2003 = 03), which is mandatory
        /// Month: the number of the month (e.g., January = 01), which is mandatory
        /// Day: the number of the day of the relevant month (e.g., second day = 02); 
        /// if it is not necessary to specify the day, the field must be filled with two zeros</remarks>
        private static DateTime logisticUnitPackagingDate = new DateTime(1900, 1, 1);

        /// <summary>
        /// Product Variant: AI (20)
        /// </summary>
        /// <remarks>This Element String may be used to distinguish a variant from the usual item 
        /// if the variation is not sufficiently significant to require a separate Global 
        /// Trade Item Number (GTIN) and the variation is relevant only to the brand owner 
        /// and any third party acting on its behalf.
        /// The product variant is only for use by the brand owner and any third party 
        /// acting on its behalf and not for dealings with any other trading partners. 
        /// The product variant shall not be used where the variation would trigger the 
        /// allocation of a different GTIN per the GTIN Allocation Rules. Although the 
        /// Element String will not have meaning to all trading partners, the Element String
        /// may remain on the item throughout distribution.
        /// The Application Identifier (20) indicates that the data field contains a 
        /// variant number.
        /// The variant number must only be assigned by the brand owner. It forms a 
        /// subsidiary numbering facility that can be used in addition to the item’s GTIN 
        /// and allows the creation of 100 variants of a particular item.
        /// The data transmitted from the bar code reader means that the Element String 
        /// denoting a product variant has been captured. The product variant must be 
        /// processed together with the GTIN of the same trade item. Beyond the brand owner 
        /// and any third party acting on its behalf, it should be ignored.</remarks>
        private static int logisticUnitProductVariant = 0;

        /// <summary>
        /// Net weight, kilograms (Variable Measure Trade Item) N4+N6 NET WEIGHT (kg)
        /// </summary>
        private static decimal netWeightMetricKG = 0;

        private static bool netWeightMetricKGFoundAI = false;

        /// <summary>
        /// Net weight, pounds (Variable Measure Trade Item) N4+N6 NET WEIGHT (lb)
        /// AI 320 (n)
        /// </summary>
        private static decimal netWeightNonMetricLB = 0;

        private static bool netWeightNonMetricLBFoundAI = false;

        /// <summary>
        /// Logistic weight, kilograms N4+N6 GROSS WEIGHT (kg)
        /// AI 330 (n)
        /// </summary>
        private static decimal grossWeightMetricKG = 0;

        private static bool grossWeightMetricKGFoundAI = false;

        /// <summary>
        /// Logistic weight, pounds N4+N6 GROSS WEIGHT (lb)
        /// AI 340 (n)
        /// </summary>
        private static decimal grossWeightNonMetricLB = 0;

        private static bool grossWeightNonMetricLBFoundAI = false;

        /// <summary>
        /// Length or 1st dimension, trade Meters
        /// AI 311 (n)
        /// </summary>
        private static decimal lengthOr1stDimensionMeters = 0;

        /// <summary>
        /// Identification of a Physical Location - Global Location Number N3+N13 LOC No
        /// </summary>
        /// <remarks>The Application Identifier (414) indicates that the GS1 Application Identifier
        /// data field contains the Global Location Number (GLN) of a physical location
        /// The structure is:
        /// AI: 414
        /// Company Prefix: N7
        /// Location Reference: N5
        /// Check Digit: N1</remarks>
        private static string locationIdentifier = string.Empty;

        /// <summary>
        /// AI 37 N2+N..8 (FNC1) COUNT
        /// Count of Trade Items Contained in a Logistic Unit: AI (37)
        /// </summary>
        /// <remarks>The Application Identifier (37) indicates that the GS1 Application Identifier data 
        /// field contains the Number of trade items contained in a logistic unit. 
        /// This Element String is a mandatory completion of AI (02) described in 
        /// Sections 3.3.4 and 3.3.5. The Count of Trade Items field contains the number of 
        /// trade items contained in the respective logistic unit. 
        /// This information refers to the identification number of the contained trade items.</remarks>
        private static int logisticUnitCountOfTradeItems = 0;

        /// <summary>
        /// AI 30 N2+N..8 (FNC1) VAR. COUNT
        /// Variable Count: AI (30)
        /// </summary>
        /// <remarks>The Application Identifier (30) indicates that the GS1 Application Identifier 
        /// data field contains the number of items contained in a Variable Measure Trade Item.
        /// This Element String is used to complete the identification of a Variable Measure 
        /// Trade Item and, therefore, should never be applied in isolation.
        /// The Count of Items field represents the quantity contained in the respective 
        /// trade item. It is of variable length and may have up to eight digits.</remarks>
        private static int logisticUnitVariableCount = 0;

        /// <summary>
        /// Company Internal Information: AI (91-99)
        /// FK Refno: AI (92)
        /// </summary>
        private static string actFKRefno = string.Empty;

        /// <summary>
        /// Company Internal Information: AI (91-99)
        /// FK Item Refno: AI (91)
        /// </summary>
        private static string actItemFKRefno = string.Empty;

        /// <summary>
        /// Company Internal Information: AI (91-99)
        /// FK Detail Refno: AI (93)
        /// </summary>
        private static string actDetailFKRefno = string.Empty;

        private static string locMapWhsCodeMask = string.Empty;
        private static string locMapSectCodeMask = string.Empty;
        private static string locMapSlotCodeMask = string.Empty;

        /// <summary>
        /// Assigned from system settings xml.
        /// </summary>
        private static string systemGTINCompanyPrefix = string.Empty;

        private static decimal deliScaleAmount = 0;

        protected BarCodeParseDetails parseDetails = new BarCodeParseDetails();

        #endregion

        #region Destructors

        ~BarCodeParse()
        {
            this.Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// AI 30 N2+N..8 (FNC1) VAR. COUNT
        /// Variable Count: AI (30)
        /// </summary>
        /// <remarks>The Application Identifier (30) indicates that the GS1 Application Identifier 
        /// data field contains the number of items contained in a Variable Measure Trade Item.
        /// This Element String is used to complete the identification of a Variable Measure 
        /// Trade Item and, therefore, should never be applied in isolation.
        /// The Count of Items field represents the quantity contained in the respective 
        /// trade item. It is of variable length and may have up to eight digits.</remarks>
        public int LogisticUnitVariableCount
        {
            get
            {
                return logisticUnitVariableCount;
            }

            set
            {
                logisticUnitVariableCount = value;
            }
        }

        /// <summary>
        /// SSCC18 serial number.
        /// </summary>
        public long ManufacturerSerialNumber
        {
            get
            {
                return manufacturerSerialNumber;
            }

            set
            {
                manufacturerSerialNumber = value;
            }
        }

        public decimal DeliScaleAmount
        {
            get
            {
                return deliScaleAmount;
            }

            set
            {
                deliScaleAmount = value;
            }
        }

        /// <summary>
        /// Bar code settings xml file.
        /// </summary>
        /// <remarks>Contains parsing rules.</remarks>
        public SysBarCodeSettings Settings
        {
            get
            {
                return settings;
            }

            set
            {
                settings = value;
            }
        }

        /// <summary>
        /// Bar code raw data read, direct from the scan engine.
        /// </summary>
        /// <remarks>Contains the symbology characters</remarks>
        public string Raw
        {
            get
            {
                return raw;
            }

            set
            {
                raw = value;
            }
        }

        /// <summary>
        /// Bar code symbology, enumeration type.
        /// </summary>
        public SysCommon.BarCodeSymbologies Symbology
        {
            get
            {
                return symbology;
            }

            set
            {
                symbology = value;
            }
        }

        /// <summary>
        /// Parse details which show which fields failed to parse correctly.
        /// </summary>
        public BarCodeParseDetails Details
        {
            get
            {
                return parseDetails;
            }
            set
            {
                parseDetails = value;
            }
        }

        /// <summary>
        /// Bar code symbology characters.
        /// </summary>
        /// <remarks>The symbology prefix is usually the first 3 characters
        /// taken from the raw data.</remarks>
        public string SymbologyPrefix
        {
            get
            {
                return symbologyPrefix;
            }

            set
            {
                symbologyPrefix = value;
            }
        }

        /// <summary>
        /// Bar code data, with the symbology prefix removed.
        /// </summary>
        public string Data
        {
            get
            {
                return data;
            }

            set
            {
                data = value;
            }
        }

        /// <summary>
        /// Portion of a bar code used to identify a product.
        /// </summary>
        public string ProductIdentifier
        {
            get
            {
                return productIdentifier;
            }

            set
            {
                productIdentifier = value;
            }
        }

        /// <summary>
        /// Bar code data that could not be parsed / identified.
        /// Remainder after the parse code has be executed.
        /// </summary>
        public string Remainder
        {
            get
            {
                return remainder;
            }

            set
            {
                remainder = value;
            }
        }

        /// <summary>
        /// Bar code length of parsable data.
        /// </summary>
        /// <remarks>The symbology prefix length is not included just the parsable data.
        /// Decided to make this a property instead of checking the length of this.data,
        /// because this.data can be altered or shortened during parsing.</remarks>
        public int Length
        {
            get
            {
                return length;
            }

            set
            {
                length = value;
            }
        }

        /// <summary>
        /// SSCC18 bar code data component.
        /// </summary>
        public int PackagingType
        {
            get
            {
                return packagingType;
            }

            set
            {
                packagingType = value;
            }
        }

        /// <summary>
        /// Used in identifying the type of UPC code.
        /// </summary>
        public int NumberSystemCharacter
        {
            get
            {
                return numberSystemCharacter;
            }

            set
            {
                numberSystemCharacter = value;
            }
        }

        /// <summary>
        /// SSCC (Serial Shipping Container Code) N2+N18 SSCC: AI (00)
        /// </summary>
        public string SSCC18
        {
            get
            {
                return sscc18;
            }

            set
            {
                sscc18 = value;
            }
        }

        /// <summary>
        /// GTIN company prefix.
        /// </summary>
        public string CompanyPrefix
        {
            get
            {
                return companyPrefix;
            }

            set
            {
                companyPrefix = value;
            }
        }

        public string ItemReference
        {
            get
            {
                return itemReference;
            }

            set
            {
                itemReference = value;
            }
        }

        /// <summary>
        /// Identification of a Fixed Measure Trade Item (GTIN): AI (01)
        /// </summary>
        /// <remarks> The Application Identifier (01) indicates that the GS1 Application Identifier 
        /// data field contains a GTIN. The GTIN is used to identify trade items
        /// The GTIN for Fixed Measure Trade Items may be a GTIN-8, GTIN-12, GTIN-13 or a GTIN-14.
        /// The Check Digit verification, which must be carried out in the application software, 
        /// ensures that the number is correctly composed.</remarks>
        public string GTIN
        {
            get
            {
                return gtin;
            }

            set
            {
                gtin = value;
            }
        }

        /// <summary>
        /// GTIN of Contained Trade Items N2+N14 CONTENT: AI (02)
        /// </summary>
        /// <remarks>
        /// Identification of Fixed Measure Trade Items Contained in a Logistic Unit:
        /// AI (02)
        /// Identification of Variable Measure Trade Items Contained in a Logistic
        /// Unit: AI (02)
        /// The Application Identifier (02) indicates that the GS1 Application Identifier 
        /// data field includes the GTIN of the contained trade items. 
        /// Indicator digit 9 is required for a GTIN-14 used for Variable Measure Trade Items.
        /// Note: This Element String may be used only on a logistic unit that is not itself 
        /// a trade item and if all trade items that are contained at the same level have 
        /// the same GTIN. If the trade items are Variable Measure Trade Items, then this GTIN 
        /// will be the implied item number that does not appear on the items contained.
        /// The Application Identifier (02) indicates that the GS1 Application Identifier 
        /// data field includes the GTIN of the contained trade items.
        /// Note: This Element String may be used only on a logistic unit that is not itself 
        /// a trade item and if all trade items that are contained at the same level have 
        /// the same GTIN.
        /// The GTIN of the trade items contained represents the identification number of the 
        /// highest level of trade item contained in the logistic unit.
        /// The Check Digit verification, which must be carried out in the application software, 
        /// ensures that the number is correctly composed.</remarks>
        public string GTINLogisticUnit
        {
            get
            {
                return gtinLogisticUnit;
            }

            set
            {
                gtinLogisticUnit = value;
            }
        }

        /// <summary>
        /// Serial Number: AI (21)
        /// </summary>
        /// <remarks>The Application Identifier (21) indicates that the GS1 Application Identifier 
        /// data field contains a serial number. A serial number is assigned to an entity 
        /// for its lifetime. When combined with a GTIN, a serial number uniquely identifies 
        /// an individual item. The serial number field is alphanumeric.
        /// The data transmitted from the bar code reader means that the Element String 
        /// denoting a serial number has been captured. As this Element String is an 
        /// attribute of a trade item, it must be processed together with the GTIN of the 
        /// trade item to which it relates.</remarks>
        public string SingleTradeItemSerialNumber
        {
            get
            {
                return singleTradeItemSerialNumber;
            }

            set
            {
                singleTradeItemSerialNumber = value;
            }
        }

        /// <summary>
        /// Batch or Lot Number: AI (10)
        /// </summary>
        /// <remarks>The Application Identifier (10) indicates that the GS1 Application Identifier 
        /// data field contains a batch or lot number. The batch or lot number associates
        /// an item with information the manufacturer considers relevant for traceability 
        /// of the trade item to which the Element String is applied. 
        /// The data may refer to the trade item itself or to items contained. 
        /// The number may be, for example, a production lot number, a shift number,
        /// a machine number, a time, or an internal production code. The data is alphanumeric.</remarks>
        public string LogisticUnitBatchLotNumber
        {
            get
            {
                return logisticUnitBatchLotNumber;
            }

            set
            {
                logisticUnitBatchLotNumber = value;
            }
        }

        /// <summary>
        /// Best Before Date: AI (15)
        /// </summary>
        /// <remarks>The Application Identifier (15) indicates that the GS1 Application Identifier 
        /// data fields contain a best before date. The best before date indicates the ideal 
        /// consumption or best effective use date of a product. 
        /// It is a statement about quality. It is often referred to as a sell by date or a
        /// minimum durability date.
        /// The structure is:
        /// Year: the tens and units of the year (e.g., 2003 = 03), which is mandatory
        /// Month: the number of the month (e.g., January = 01), which is mandatory
        /// Day: the number of the day of the relevant month (e.g., second day = 02); 
        /// if it is not necessary to specify the day, the field must be filled with two zeros</remarks>
        public DateTime LogisticUnitBestBeforeSellByDate
        {
            get
            {
                return logisticUnitBestBeforeSellByDate;
            }

            set
            {
                logisticUnitBestBeforeSellByDate = value;
            }
        }

        /// <summary>
        /// Expiration Date: AI (17)
        /// </summary>
        /// <remarks>The Application Identifier (17) indicates that the GS1 Application Identifier 
        /// data fields contain an expiration date. The expiration date is the date that 
        /// determines the limit of consumption or use of a product. 
        /// Its meaning is determined based on the trade item context (e.g., for food, the date
        /// will indicate the possibility of a direct health risk resulting from use of the 
        /// product after the date, for pharmaceutical products, it will indicate the 
        /// possibility of an indirect health risk resulting from the ineffectiveness of 
        /// the product after the date). It is often referred to as "use by date" or 
        /// "maximum durability date.”
        /// The structure is:
        /// Year: the tens and units of the year (e.g., 2003 = 03), which is mandatory
        /// Month: the number of the month (e.g., January = 01), which is mandatory
        /// Day: the number of the day of the relevant month (e.g., second day = 02); 
        /// if it is not necessary to specify the day, the field must be filled with two zeros</remarks>
        public DateTime LogisticUnitExpiryUseByDate
        {
            get
            {
                return logisticUnitExpiryUseByDate;
            }

            set
            {
                logisticUnitExpiryUseByDate = value;
            }
        }

        /// <summary>
        /// Production Date: AI (11)
        /// </summary>
        /// <remarks>The Application Identifier (11) indicates that the GS1 Application Identifier 
        /// data field contains a production date. The production date is the production 
        /// or assembly date determined by the manufacturer. 
        /// The date may refer to the trade item itself or to items contained.
        /// The structure is:
        /// Year: the tens and units of the year (e.g., 2003 = 03), which is mandatory
        /// Month: the number of the month (e.g., January = 01), which is mandatory
        /// Day: the number of the day of the relevant month (e.g., second day = 02); 
        /// if it is not necessary to specify the day, the field must be filled with two zeros</remarks>
        public DateTime LogisticUnitProductionDate
        {
            get
            {
                return logisticUnitProductionDate;
            }

            set
            {
                logisticUnitProductionDate = value;
            }
        }

        /// <summary>
        /// Due Date for Amount on Payment Slip: AI (12)
        /// </summary>
        /// <remarks>The Application Identifier (12) indicates that the GS1 Application Identifier 
        /// data field contains the date by which an invoice should be paid. 
        /// This data element represents an attribute of a payment slip reference number, 
        /// AI (8020), and a Global Location Number (GLN) of the invoicing party.
        /// The structure is:
        /// Year: the tens and units of the year (e.g., 2003 = 03), which is mandatory
        /// Month: the number of the month (e.g., January = 01), which is mandatory
        /// Day: the number of the day of the relevant month (e.g., second day = 02); 
        /// if it is not necessary to specify the day, the field must be filled with two zeros</remarks>
        public DateTime LogisticUnitDueDate
        {
            get
            {
                return logisticUnitDueDate;
            }

            set
            {
                logisticUnitDueDate = value;
            }
        }

        /// <summary>
        /// Packaging Date: AI (13)
        /// </summary>
        /// <remarks>The Application Identifier (13) indicates that the GS1 Application Identifier 
        /// data fields contain a packaging date. The packaging date is the date when 
        /// the goods were packed as determined by the packager. 
        /// The date may refer to the trade item itself or to items contained.
        /// The structure is:
        /// Year: the tens and units of the year (e.g., 2003 = 03), which is mandatory
        /// Month: the number of the month (e.g., January = 01), which is mandatory
        /// Day: the number of the day of the relevant month (e.g., second day = 02); 
        /// if it is not necessary to specify the day, the field must be filled with two zeros</remarks>
        public DateTime LogisticUnitPackagingDate
        {
            get
            {
                return logisticUnitPackagingDate;
            }

            set
            {
                logisticUnitPackagingDate = value;
            }
        }

        /// <summary>
        /// Product Variant: AI (20)
        /// </summary>
        /// <remarks>This Element String may be used to distinguish a variant from the usual item 
        /// if the variation is not sufficiently significant to require a separate Global 
        /// Trade Item Number (GTIN) and the variation is relevant only to the brand owner 
        /// and any third party acting on its behalf.
        /// The product variant is only for use by the brand owner and any third party 
        /// acting on its behalf and not for dealings with any other trading partners. 
        /// The product variant shall not be used where the variation would trigger the 
        /// allocation of a different GTIN per the GTIN Allocation Rules. Although the 
        /// Element String will not have meaning to all trading partners, the Element String
        /// may remain on the item throughout distribution.
        /// The Application Identifier (20) indicates that the data field contains a 
        /// variant number.
        /// The variant number must only be assigned by the brand owner. It forms a 
        /// subsidiary numbering facility that can be used in addition to the item’s GTIN 
        /// and allows the creation of 100 variants of a particular item.
        /// The data transmitted from the bar code reader means that the Element String 
        /// denoting a product variant has been captured. The product variant must be 
        /// processed together with the GTIN of the same trade item. Beyond the brand owner 
        /// and any third party acting on its behalf, it should be ignored.</remarks>
        public int LogisticUnitProductVariant
        {
            get
            {
                return logisticUnitProductVariant;
            }

            set
            {
                logisticUnitProductVariant = value;
            }
        }

        /// <summary>
        /// Net weight, kilograms (Variable Measure Trade Item) N4+N6 NET WEIGHT (kg)
        /// </summary>
        public decimal NetWeightMetricKG
        {
            get
            {
                return netWeightMetricKG;
            }

            set
            {
                netWeightMetricKG = value;
            }
        }

        public bool NetWeightMetricKGFoundAI
        {
            get
            {
                return netWeightMetricKGFoundAI;
            }

            set
            {
                netWeightMetricKGFoundAI = value;
            }
        }

        /// <summary>
        /// Net weight, pounds (Variable Measure Trade Item) N4+N6 NET WEIGHT (lb)
        /// AI 320 (n)
        /// </summary>
        public decimal NetWeightNonMetricLB
        {
            get
            {
                return netWeightNonMetricLB;
            }

            set
            {
                netWeightNonMetricLB = value;
            }
        }

        public bool NetWeightNonMetricLBFoundAI
        {
            get
            {
                return netWeightNonMetricLBFoundAI;
            }

            set
            {
                netWeightNonMetricLBFoundAI = value;
            }
        }

        /// <summary>
        /// Logistic weight, kilograms N4+N6 GROSS WEIGHT (kg)
        /// AI 330 (n)
        /// </summary>
        public decimal GrossWeightMetricKG
        {
            get
            {
                return grossWeightMetricKG;
            }

            set
            {
                grossWeightMetricKG = value;
            }
        }

        public bool GrossWeightMetricKGFoundAI
        {
            get
            {
                return grossWeightMetricKGFoundAI;
            }

            set
            {
                grossWeightMetricKGFoundAI = value;
            }
        }

        /// <summary>
        /// Logistic weight, pounds N4+N6 GROSS WEIGHT (lb)
        /// AI 340 (n)
        /// </summary>
        public decimal GrossWeightNonMetricLB
        {
            get
            {
                return grossWeightNonMetricLB;
            }

            set
            {
                grossWeightNonMetricLB = value;
            }
        }

        public bool GrossWeightNonMetricLBFoundAI
        {
            get
            {
                return grossWeightNonMetricLBFoundAI;
            }

            set
            {
                grossWeightNonMetricLBFoundAI = value;
            }
        }

        /// <summary>
        /// Length or 1st dimension, trade Meters
        /// AI 311 (n)
        /// </summary>
        public decimal LengthOr1stDimensionMeters
        {
            get
            {
                return lengthOr1stDimensionMeters;
            }

            set
            {
                lengthOr1stDimensionMeters = value;
            }
        }

        /// <summary>
        /// Identification of a Physical Location - Global Location Number N3+N13 LOC No
        /// </summary>
        /// <remarks>The Application Identifier (414) indicates that the GS1 Application Identifier
        /// data field contains the Global Location Number (GLN) of a physical location
        /// The structure is:
        /// AI: 414
        /// Company Prefix: N7
        /// Location Reference: N5
        /// Check Digit: N1</remarks>
        public string LocationIdentifier
        {
            get
            {
                return locationIdentifier;
            }

            set
            {
                locationIdentifier = value;
            }
        }

        /// <summary>
        /// AI 37 N2+N..8 (FNC1) COUNT
        /// Count of Trade Items Contained in a Logistic Unit: AI (37)
        /// </summary>
        /// <remarks>The Application Identifier (37) indicates that the GS1 Application Identifier data 
        /// field contains the Number of trade items contained in a logistic unit. 
        /// This Element String is a mandatory completion of AI (02) described in 
        /// Sections 3.3.4 and 3.3.5. The Count of Trade Items field contains the number of 
        /// trade items contained in the respective logistic unit. 
        /// This information refers to the identification number of the contained trade items.</remarks>
        public int LogisticUnitCountOfTradeItems
        {
            get
            {
                return logisticUnitCountOfTradeItems;
            }

            set
            {
                logisticUnitCountOfTradeItems = value;
            }
        }

        /// <summary>
        /// Company Internal Information: AI (91-99)
        /// FK Refno: AI (92)
        /// </summary>
        public string ActFKRefno
        {
            get
            {
                return actFKRefno;
            }

            set
            {
                actFKRefno = value;
            }
        }

        /// <summary>
        /// Company Internal Information: AI (91-99)
        /// FK Item Refno: AI (91)
        /// </summary>
        public string ActItemFKRefno
        {
            get
            {
                return actItemFKRefno;
            }

            set
            {
                actItemFKRefno = value;
            }
        }

        /// <summary>
        /// Company Internal Information: AI (91-99)
        /// FK Detail Refno: AI (93)
        /// </summary>
        public string ActDetailFKRefno
        {
            get
            {
                return actDetailFKRefno;
            }

            set
            {
                actDetailFKRefno = value;
            }
        }

        public string LocMapWhsCodeMask
        {
            get
            {
                return locMapWhsCodeMask;
            }

            set
            {
                locMapWhsCodeMask = value;
            }
        }

        public string LocMapSectCodeMask
        {
            get
            {
                return locMapSectCodeMask;
            }

            set
            {
                locMapSectCodeMask = value;
            }
        }

        public string LocMapSlotCodeMask
        {
            get
            {
                return locMapSlotCodeMask;
            }

            set
            {
                locMapSlotCodeMask = value;
            }
        }

        /// <summary>
        /// Assigned from system settings xml
        /// </summary>
        public string SystemGTINCompanyPrefix
        {
            get
            {
                return systemGTINCompanyPrefix;
            }

            set
            {
                systemGTINCompanyPrefix = value;
            }
        }

        #endregion

        #region Public Members

        public abstract void ParseBarCodeData(string barCodeData);

        /// <summary>
        /// 
        /// </summary>
        public void ClearResetBarCodeProperties()
        {
            this.ClearResetBarCodeProperties(true);
        }

        /// <summary>
        /// The bIncludeSymbologyData = true argument resets the values that where assigned 
        /// in the class BarCodeParseFacade method IdentifySymbology
        /// </summary>
        public void ClearResetBarCodeProperties(bool resetSymbologyData)
        {
            SystemSettings sysSettings = SysEnvironment.GetInstance().SysSettings;
            this.LocMapWhsCodeMask = sysSettings.LocMapWarehouseMask;
            this.LocMapSectCodeMask = sysSettings.LocMapSectionMask;
            this.LocMapSlotCodeMask = sysSettings.LocMapSlotMask;
            this.SystemGTINCompanyPrefix = sysSettings.GTINCompanyPrefix;

            if (resetSymbologyData)
            {
                this.Raw = string.Empty;
                this.SymbologyPrefix = string.Empty;
                this.Symbology = SysCommon.BarCodeSymbologies.UNDEFINED;
                this.Data = string.Empty;
                this.Length = 0;
            }

            this.ProductIdentifier = string.Empty;
            this.Remainder = string.Empty;
            this.PackagingType = 0;
            this.SSCC18 = string.Empty;
            this.CompanyPrefix = string.Empty;
            this.ItemReference = string.Empty;
            this.GTIN = string.Empty;
            this.GTINLogisticUnit = string.Empty;
            this.SingleTradeItemSerialNumber = string.Empty;
            this.LogisticUnitBatchLotNumber = string.Empty;
            this.LogisticUnitBestBeforeSellByDate = new DateTime(1900, 1, 1);
            this.LogisticUnitExpiryUseByDate = new DateTime(1900, 1, 1);
            this.LogisticUnitProductionDate = new DateTime(1900, 1, 1);
            this.LogisticUnitDueDate = new DateTime(1900, 1, 1);
            this.LogisticUnitPackagingDate = new DateTime(1900, 1, 1);
            this.LogisticUnitProductVariant = 0;
            this.NetWeightMetricKG = 0;
            this.NetWeightMetricKGFoundAI = false;
            this.NetWeightNonMetricLB = 0;
            this.NetWeightNonMetricLBFoundAI = false;
            this.GrossWeightMetricKG = 0;
            this.GrossWeightMetricKGFoundAI = false;
            this.GrossWeightNonMetricLB = 0;
            this.GrossWeightNonMetricLBFoundAI = false;
            this.LocationIdentifier = string.Empty;
            this.LogisticUnitCountOfTradeItems = 0;
            this.ActFKRefno = string.Empty;
            this.ActItemFKRefno = string.Empty;
            this.ActDetailFKRefno = string.Empty;
            this.DeliScaleAmount = 0;
            this.LogisticUnitVariableCount = 0;
            this.Details.ClearErrors();
        }


        /// <summary>
        /// Checks if there are errors caught for this parsing.
        /// </summary>
        /// <returns>True if errors exist.</returns>
        public bool HasErrors()
        {
            return (parseDetails != null) && (parseDetails.HasErrors);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Code to clean up managed resources
            }

            // Code to clean up unmanaged resources
        }

        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected Methods

        // Methods
        protected virtual string IdentifyProductIdentifier(string barCodeData)
        {
            if (barCodeData.Length > 14)
            {
                this.ProductIdentifier = barCodeData.Substring(0, 14);
            }
            else
            {
                this.ProductIdentifier = barCodeData;
            }

            barCodeData = barCodeData.Remove(0, this.ProductIdentifier.Length);

            return barCodeData;
        }


        /// <summary>
        /// Adds a parse exception to the list.
        /// </summary>
        /// <param name="exception"></param>
        protected void AddParseErrorDetail(BarCodeExceptionReason exception, BarCodeParseException e)
        {
            if (parseDetails == null)
                parseDetails = new BarCodeParseDetails();
            parseDetails.AddErrorDetail(exception, e);
        }

        protected virtual string IdentifyPackagingType(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyCompanyPrefix(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyItemReference(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyGTIN(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyGTINinLogisticUnit(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifySSCC18(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyLogisticUnitBatchLotNumber(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifySingleTradeItemSerialNumber(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyLogisticUnitProductionDate(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyLogisticUnitDueDate(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyLogisticUnitPackagingDate(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyLogisticUnitBestBeforeSellByDate(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyLogisticUnitExpiryUseByDate(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyLogisticUnitProductVariant(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyLogisticUnitCountOfTradeItems(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyLogisticUnitVariableCount(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyLocationIdentifier(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyNetWeightMetricKG(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyNetWeightNonMetricLB(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyGrossWeightMetricKG(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyGrossWeightNonMetricLB(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyActivityFKRefno(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyActivityItemFKRefno(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyActivityDetailFKRefno(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyManufacturerSerialNumber(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyNumberSystemCharacter(string barCodeData)
        {
            throw new NotImplementedException();
        }

        protected virtual string IdentifyLengthOr1stDimensionMeters(string barCodeData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find the location of the first group separator occurrance in a bar code.
        /// </summary>
        /// <param name="barCodeData">Bar code data</param>
        /// <returns>Zero based index location of the group separator.
        /// Negative 1 for no group separator found.</returns>
        public int FindGroupSeparatorInBarCode(string barCodeData)
        {
            if (barCodeData == null)
                return -1;

            int sepIndex = -1;
            char[] data = barCodeData.ToCharArray();

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == GroupSeparator)
                {
                    sepIndex = i;
                    break;
                }
            }

            return sepIndex;
        }

        #endregion
    }
}
