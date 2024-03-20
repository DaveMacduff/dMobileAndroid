// <copyright file="BarCodeSettings.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Bar Code Settings Class.
    /// </summary>
    [Serializable]
    public class SysBarCodeSettings : IBarCodeSettingsEventPublisher
    {
        [NonSerialized]
        private readonly object writeLockObject = new object();

        [NonSerialized]
        private readonly object readLockObject = new object();

        /// <summary>
        /// Contains full bar code settings class name.
        /// </summary>
        [NonSerialized]
        private string fullClassName = "dMobile.BarCodeSettings";

        /// <summary>
        /// Contains bar code settings file name.
        /// </summary>
        [NonSerialized]
        private string fileName = "dMobileBarCodeSettings.xml";

        /// <summary>
        /// Contains bar code settings path name.
        /// </summary>
        [NonSerialized]
        private string filePath = string.Empty;

        /// <summary>
        /// Contains bar code settings drive letter, path and file name.
        /// </summary>
        [NonSerialized]
        private string fileASCIIZ = string.Empty;

        /// <summary>
        /// Rule: Sometimes a bar code is fully EAN/UCC 128 Symbol parsable EXCEPT the actual symbology is Code 128.
        /// </summary>
        private bool isRuleEnabled_ParseCode_128AsUCC_EAN_128_SymbolCompliant = false;

        /// <summary>
        /// Rule Description: Sometimes a bar code is fully EAN/UCC 128 Symbol parsable EXCEPT the actual symbology is
        /// Code 128.
        /// </summary>
        private string ruleDescription_ParseCode_128AsUCC_EAN_128_SymbolCompliant = string.Empty;

        /// <summary>
        /// Rule: Vendor is FINICA FOOD SPECIALITIES.
        /// </summary>
        private bool isRuleEnabled_ParseFINICA_Code_128_as_EAN_UCC_128 = false;

        /// <summary>
        /// Rule Description: Vendor is FINICA FOOD SPECIALITIES.
        /// </summary>
        private string ruleDescription_ParseFINICA_Code_128_as_EAN_UCC_128 = string.Empty;

        /// <summary>
        /// Rule: GS1 UCC/EAN-128 AI(414) Global Location Number should have a check digit on the end (414)  9.
        /// </summary>
        private bool isRuleEnabled_NoCheckDigitInGlobalLocationNumber = false;

        /// <summary>
        /// Rule Description: GS1 UCC/EAN-128 AI(414) Global Location Number should have a check digit on the end (414)  9.
        /// </summary>
        private string ruleDescription_NoCheckDigitInGlobalLocationNumber = string.Empty;

        /// <summary>
        /// Rule: No zero padding of product identifier for UCC/EAN family.
        /// </summary>
        /// <remarks>
        /// GTIN-12 (UPC)
        /// GTIN-13 (EAN-13)
        /// GTIN-14 (EAN/UCC-128 or ITF-14)
        /// GTIN-8 (EAN-8).
        /// </remarks>
        private bool isRuleEnabled_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family;

        /// <summary>
        /// Rule Description: No zero padding of product identifier for UCC/EAN family.
        /// </summary>
        private string ruleDescription_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family;

        /// <summary>
        /// Rule: Vendor is WOOLWICH DAIRY (cheese products) Bar Code is a UCC/EAN 128 symbology with 4 spaces in the GTIN,
        /// no other AI's exist. Length is 18.
        /// </summary>
        private bool isRuleEnabled_ParseWOOLWICH_Code_128_RemoveSpaces = false;

        /// <summary>
        /// Rule Description: Vendor is WOOLWICH DAIRY (cheese products) Bar Code is a UCC/EAN 128 symbology with 4 spaces
        /// in the GTIN, no other AI's exist. Length is 18.
        /// </summary>
        private string ruleDescription_ParseWOOLWICH_Code_128_RemoveSpaces = string.Empty;

        /// <summary>
        /// Rule: Recognize Code 128 AI(13) Packaging Date as a AI(11) Production Date.
        /// </summary>
        private bool isRuleEnabled_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate = false;

        /// <summary>
        /// Rule Description: Recognize Code 128 AI(13) Packaging Date as a AI(11) Production Date.
        /// </summary>
        private string ruleDescription_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate = string.Empty;

        /// <summary>
        /// Rule: Recognize Code 128 AI(17) Expiry Date as a AI(15) Best Before Date.
        /// </summary>
        private bool ruleEnabled_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate = false;

        /// <summary>
        /// Rule Description: Recognize Code 128 AI(17) Expiry Date as a AI(15) Best Before Date.
        /// </summary>
        private string ruleDescription_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate = string.Empty;

        /// <summary>
        /// Rule: Parse EAN/UCC Code 128 AI(01) with Nossack Company Prefix, AI(21) contains Skid Number and Box Count.
        /// </summary>
        private bool ruleEnabled_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount = false;

        /// <summary>
        /// Rule Description: Parse EAN/UCC Code 128 AI(01) with Nossack Company Prefix, AI(21) contains Skid Number and
        /// Box Count.
        /// </summary>
        private string ruleDescription_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount = string.Empty;

        /// <summary>
        /// Rule: Product Bar Code Minimum Length Requirement 4 Digits.
        /// </summary>
        private bool isRuleEnabled_ProductBarCodeMinimumLengthRequirement4Digits = false;

        /// <summary>
        /// Rule Description: Product Bar Code Minimum Length Requirement 4 Digits.
        /// </summary>
        private string ruleDescription_ProductBarCodeMinimumLengthRequirement4Digits = string.Empty;

        /// <summary>
        /// Rule: Enable POS PLU parsing UCC-12 / UPC-E.
        /// </summary>
        private bool isRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU = false;

        /// <summary>
        /// Rule Description: Enable POS PLU parsing UCC-12 / UPC-E.
        /// </summary>
        private string ruleDescription_Parse_UCC_12_UPC_E_Deli_PLU_Enabled = string.Empty;

        /// <summary>
        /// Rule: Enable to restrict deli scale amount to the last 4 digits. e.g. ignore the hundreds digit.
        /// This restricts amounts to the [0,99.99] range.
        /// </summary>
        /// <remarks>
        /// This was written for Prairie's bad label printing which creates bad amounts over 99.99.
        /// </remarks>
        private bool isRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount = false;

        /// <summary>
        /// Rule Description: Parse UCC_12_UPC_E as Deli PLU 4 Digit Amount. Restricts amount to the 0-99.99 range.
        /// </summary>s
        private string ruleDescription_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount_Enabled = string.Empty;

        /// <summary>
        /// Rule: New Zealand Lamb Ovation Fielding parsing.
        /// </summary>
        private bool isRuleEnabled_Parse_Code128_OvationFielding_Lamb = false;

        /// <summary>
        /// Rule Description: New Zealand Lamb Ovation Fielding parsing.
        /// </summary>
        private string ruleDescription_Parse_Code128_OvationFielding_Lamb = string.Empty;

        /// <summary>
        /// Rule: Parse China seafood EAN/UCC Code 128, length is 14, data starts with '30'.
        /// </summary>
        private bool isRuleEnabled_Parse_Code128_China_Seafood = false;

        /// <summary>
        /// Rule Description: Parse China seafood EAN/UCC Code 128, length is 14, data starts with '30'.
        /// </summary>
        private string ruleDescription_Parse_Code128_China_Seafood = string.Empty;

        /// <summary>
        /// Rule: Parse EAN/UCC Code 128 AI(01) with B&C Company Prefix, AI(30) contains quantity of units, exclusive.
        /// Rule changed to ignore company prefix. Oct 20/2014.
        /// </summary>
        private bool isRuleEnabled_Parse_Code_128_AI01_BC_AI30_exclusive = false;

        /// <summary>
        /// Rule Description: Parse EAN/UCC Code 128 AI(01) with B&C Company Prefix, AI(30) contains quantity of units,
        /// exclusive.
        /// Rule changed to ignore the company prefix. Oct 20/2014.
        /// </summary>
        private string ruleDescription_Parse_Code_128_AI01_BC_AI30_exclusive = string.Empty;

        /// <summary>
        /// Rule: Vendor is Renee's Gourmet Bar Code is a UCC/EAN 128 symbology with 4 spaces in the GTIN,
        /// no other AI's exist. Length is 18.
        /// </summary>
        private bool isRuleEnabled_Parse_Renees_Code_128_RemoveSpaces = false;

        /// <summary>
        /// Rule Description: Vendor is Renee's Bar Code is a UCC/EAN 128 symbology with 4 spaces in the GTIN,
        /// no other AI's exist. Length is 18.
        /// </summary>
        private string ruleDescription_Parse_Renees_Code_128_RemoveSpaces = string.Empty;

        /// <summary>
        /// Rule: Vendor is Lanz Meats, Bar Code is a UCC/EAN 128 symbology, AI(10) Lot Number will become
        /// AI(21) Serial Number.
        /// </summary>
        private bool isRuleEnabled_ParseLanzMeats_Code_128_AI10_as_AI21 = false;

        /// <summary>
        /// Rule Description: Vendor is Lanz Meats, Bar Code is a UCC/EAN 128 symbology, AI(10) Lot Number will become
        /// AI(21) Serial Number.
        /// </summary>
        private string ruleDescription_ParseLanzMeats_Code_128_AI10_as_AI21 = string.Empty;

        /// <summary>
        /// Rule: Heritage Global Location Identifier do a second pass ignoring the check digit.
        /// </summary>
        /// <remarks>A check digit calculation flaw cause the the global location identifiers check digits to be printed
        /// incorrectly. We needed a way to ignore them. This is a temporary solution, until new location
        /// identifier's can be printed with the corrected check digits.</remarks>
        private bool isRuleEnabled_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit = false;

        /// <summary>
        /// Rule Description: Heritage Global Location Identifier do a second pass ignoring the check digit.
        /// </summary>
        /// <remarks>A check digit calculation flaw cause the the global location identifiers check digits to be printed
        /// incorrectly. We needed a way to ignore them. This is a temporary solution, until new location
        /// identifier's can be printed with the corrected check digits.</remarks>
        private string ruleDescription_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit = string.Empty;

        /// <summary>
        /// Rule: Parse Nossack Broker Multi National Product From Chili Company Prefix 896775.
        /// </summary>
        private bool isRuleEnabled_ParseNossackBrokerMultiNationalProductFromChili = false;

        /// <summary>
        /// Rule Description: Parse Nossack Broker Multi National Product From Chili Company Prefix 896775.
        /// </summary>
        private string ruleDescription_ParseNossackBrokerMultiNationalProductFromChili = string.Empty;

        /// <summary>
        /// Rule: Parse Nossack Greenlea Premier Meats.
        /// </summary>
        private bool isRuleEnabled_ParseEAN_UCC128_Nossack_GreenleaPremierMeats = false;

        /// <summary>
        /// Rule Description: Parse Nossack Greenlea Premier Meats.
        /// </summary>
        private string ruleDescription_ParseEAN_UCC128_Nossack_GreenleaPremierMeats = string.Empty;

        /// <summary>
        /// Rule: Enable Parse UCC_13 as UPC_12_UPC_E.
        /// </summary>
        private bool isRuleEnabled_Parse_UCC_13_as_UPC_12_UPC_E = false;

        /// <summary>
        /// Rule Description: Enable Parse UCC_13 as UPC_12 UPC_E.
        /// </summary>
        private string ruleDescription_Parse_UCC_13_as_UPC_12_UPC_E_Enabled = string.Empty;

        /// <summary>
        /// Rule: Enable Parse Hela Spice Canada Code 128.
        /// </summary>
        private bool isRuleEnabled_Parse_HelaSpiceCanada_Code_128 = false;

        /// <summary>
        /// Rule Description: Enable Parse Hela Spice Canada Code 128.
        /// </summary>
        private string ruleDescription_Parse_HelaSpiceCanada_Code_128 = string.Empty;

        /// <summary>
        /// Rule: Enable Nebraska Beef parse Lot Number as Serial Number.
        /// </summary>
        private bool isRuleEnabled_NebraskaBeef_Parse_Lot_as_Serial_Code_128 = false;

        /// <summary>
        /// Rule Description: Enable Nebraska Beef parse Lot Number as Serial Number.
        /// </summary>
        private string ruleDescription_NebraskaBeef_Parse_Lot_as_Serial_Code_128 = string.Empty;

        /// <summary>
        /// Rule: Enable Parse Northern Coop Australia Beef Trim Product '25612'.
        /// </summary>
        private bool isRuleEnabled_ParseNorthernCoop_AustraliaBeefTrim_Product25612 = false;

        /// <summary>
        /// Rule Description: Enable Parse Northern Coop Australia Beef Trim Product '25612'.
        /// </summary>
        private string ruleDescription_ParseNorthernCoop_AustraliaBeefTrim_Product25612 = string.Empty;

        /// <summary>
        /// Rule: Enable Parse Sumwa Farm Fed Whole Duck use Weight Range To ID Product.
        /// </summary>
        private bool isRuleEnabled_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct = false;

        /// <summary>
        /// Rule Description: Enable Parse Sumwa Farm Fed Whole Duck use Weight Range To ID Product.
        /// </summary>
        private string ruleDescription_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct = string.Empty;

        /// <summary>
        /// Rule: Enable Parse EAN/UCC Code 128 Nossack Product 94600 Vendor Chilean Back Ribs fix AI(01).
        /// </summary>
        private bool isRuleEnabled_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs = false;

        /// <summary>
        /// Rule Description: Enable Parse EAN/UCC Code 128 Nossack Product 94600 Vendor Chilean Back Ribs fix AI(01).
        /// </summary>
        private string ruleDescription_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs = string.Empty;

        /// <summary>
        /// Rule: Enable Parse EAN/UCC Code 128 Dovre Kii Natural Crisps fix AI(02) as AI(01) and AI(15) DDMMYY to YYMMDD.
        /// </summary>
        private bool isRuleEnabled_ParseEAN_UCC128_Dovre_KiiNaturalsCrisps = false;

        /// <summary>
        /// Rule Description: Enable Parse EAN/UCC Code 128 Dovre Kii Natural Crisps fix AI(02) as AI(01) and AI(15) DDMMYY to YYMMDD.
        /// </summary>
        private string ruleDescription_ParseEAN_UCC128_Dovre_KiiNaturalsCrisps = string.Empty;

        /// <summary>
        /// Rule: Enable Parse EAN/UCC Code 128 Nossack Product 51134 Frozen Pork Collar remove AI(00).
        /// </summary>
        private bool isRuleEnabled_ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00 = false;

        /// <summary>
        /// Rule Description: Enable Parse EAN/UCC Code 128 Nossack Product 51134 Frozen Pork Collar remove AI(00).
        /// </summary>
        private string ruleDescription_ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00 = string.Empty;

        /// <summary>
        /// Rule: When enabled it will suppress error messages for non-tracked fields.
        /// This allows the bar code to be fully parsed, even when a field or two is missing as long
        /// as they are non-tracked.
        /// </summary>
        private bool isRuleEnabled_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents = false;

        /// <summary>
        /// Rule Description: When enabled it will suppress error messages for non-tracked fields.
        /// This allows the bar code to be fully parsed, even when a field or two is missing as long
        /// as they are non-tracked.
        /// </summary>
        private string ruleDescription_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents = string.Empty;

        /// <summary>
        /// Removes invalid character 0x001A from beginning of Code128 bar codes.
        /// </summary>
        private bool isRuleEnabled_Parse_Code128_CleanSubstituteCharacter = false;

        /// <summary>
        /// Removes invalid character 0x001A from beginning of Code128 bar codes.
        /// </summary>
        private string ruleDescription_Parse_Code128_CleanSubstituteCharacter = string.Empty;

        /// <summary>
        /// Parse EAN / UCC Code 128 Bourvy Exports production date from the AI(21) serial number.
        /// </summary>
        private bool isRuleEnabled_ParseEAN_UCC128_BourvyExports_ProductionDate = false;

        /// <summary>
        /// Parse EAN / UCC Code 128 Bourvy Exports production date from the AI(21) serial number.
        /// </summary>
        private string ruleDescription_ParseEAN_UCC128_BourvyExports_ProductionDate = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="SysBarCodeSettings"/> class.
        /// </summary>
        public SysBarCodeSettings()
        {
            this.FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            this.FileASCIIZ = Path.Combine(this.FilePath, this.FileName);
        }

        /// <summary>
        /// On bar code settings status out event handler.
        /// </summary>
        /// <param name="publisher"><see cref="IBarCodeSettingsEventPublisher"/> Publisher.</param>
        /// <param name="eventArgs"><see cref="BarCodeSettingsEventArgs"/> Bar code setting event arguments.</param>
        private delegate void OnBarCodeSettingsStatusOutEventHandler(
            IBarCodeSettingsEventPublisher publisher,
            BarCodeSettingsEventArgs eventArgs);

        /// <summary>
        /// On bar code settings progress out event hander.
        /// </summary>
        /// <param name="publisher"><see cref="IBarCodeSettingsEventPublisher"/> Publisher.</param>
        /// <param name="eventArgs"><see cref="BarCodeSettingsEventArgs"/> Bar code setting event arguments.</param>
        private delegate void OnBarCodeSettingsProgressOutEventHandler(
            IBarCodeSettingsEventPublisher publisher,
            BarCodeSettingsEventArgs eventArgs);

        /// <summary>
        /// On bar code settings status out event.
        /// </summary>
        private event OnBarCodeSettingsStatusOutEventHandler OnBarCodeSettingsStatusOut;

        /// <summary>
        /// On bar code settings progress out event.
        /// </summary>
        private event OnBarCodeSettingsProgressOutEventHandler OnBarCodeSettingsProgressOut;

        /// <summary>
        /// Gets or sets the path name of the xml file.
        /// </summary>
        public string FilePath
        {
            get => this.filePath;

            set => this.filePath = value;
        }

        /// <summary>
        /// Gets or sets the full path and file name of the xml file.
        /// </summary>
        /// <remarks>
        /// Includes the drive letter, path and file name.
        /// </remarks>
        public string FileASCIIZ
        {
            get => this.fileASCIIZ;

            set => this.fileASCIIZ = value;
        }

        /// <summary>
        /// Gets or sets the file name of the xml file.
        /// </summary>
        /// <remarks>
        /// Does not include any path information.
        /// </remarks>
        public string FileName
        {
            get => this.fileName;

            set => this.fileName = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether class name to serialize.
        /// </summary>
        public string FullClassName
        {
            get => this.fullClassName;

            set => this.fullClassName = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_Code_128_AI01_BC_AI30_exclusive rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_Code_128_AI01_BC_AI30_exclusive
        {
            get => this.isRuleEnabled_Parse_Code_128_AI01_BC_AI30_exclusive;

            set => this.isRuleEnabled_Parse_Code_128_AI01_BC_AI30_exclusive = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_Code_128_AI01_BC_AI30_exclusive rule description.
        /// </summary>
        public string RuleDescription_Parse_Code_128_AI01_BC_AI30_exclusive
        {
            get => this.ruleDescription_Parse_Code_128_AI01_BC_AI30_exclusive;

            set => this.ruleDescription_Parse_Code_128_AI01_BC_AI30_exclusive = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_Code128_China_Seafood rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_Code128_China_Seafood
        {
            get => this.isRuleEnabled_Parse_Code128_China_Seafood;

            set => this.isRuleEnabled_Parse_Code128_China_Seafood = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_Code128_China_Seafood rule description.
        /// </summary>
        public string RuleDescription_Parse_Code128_China_Seafood
        {
            get => this.ruleDescription_Parse_Code128_China_Seafood;

            set => this.ruleDescription_Parse_Code128_China_Seafood = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU
        {
            get => this.isRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU;

            set => this.isRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU rule description.
        /// </summary>
        public string RuleDescription_Parse_UCC_12_UPC_E_Deli_PLU_Enabled
        {
            get => this.ruleDescription_Parse_UCC_12_UPC_E_Deli_PLU_Enabled;

            set => this.ruleDescription_Parse_UCC_12_UPC_E_Deli_PLU_Enabled = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether if we restrict the deli plu amount to 4 digits. (Down from 5).
        /// </summary>
        public bool IsRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount
        {
            get => this.isRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount;

            set => this.isRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount = value;
        }

        /// <summary>
        /// Gets or sets the rule description of deli PLU amount restricted to 4 digits.
        /// </summary>
        public string RuleDescription_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount_Enabled
        {
            get => this.ruleDescription_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount_Enabled;

            set => this.ruleDescription_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount_Enabled = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ProductBarCodeMinimumLengthRequirement4Digits rule.
        /// </summary>
        public bool IsRuleEnabled_ProductBarCodeMinimumLengthRequirement4Digits
        {
            get => this.isRuleEnabled_ProductBarCodeMinimumLengthRequirement4Digits;

            set => this.isRuleEnabled_ProductBarCodeMinimumLengthRequirement4Digits = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ProductBarCodeMinimumLengthRequirement4Digits rule
        /// description.
        /// </summary>
        public string RuleDescription_ProductBarCodeMinimumLengthRequirement4Digits
        {
            get => this.ruleDescription_ProductBarCodeMinimumLengthRequirement4Digits;

            set => this.ruleDescription_ProductBarCodeMinimumLengthRequirement4Digits = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount
        {
            get => this.ruleEnabled_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount;

            set => this.ruleEnabled_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount
        /// rule description.
        /// </summary>
        public string RuleDescription_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount
        {
            get => this.ruleDescription_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount;

            set => this.ruleDescription_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate
        /// rule.
        /// </summary>
        public bool IsRuleEnabled_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate
        {
            get => this.ruleEnabled_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate;

            set => this.ruleEnabled_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate
        /// rule description.
        /// </summary>
        public string RuleDescription_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate
        {
            get => this.ruleDescription_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate;

            set => this.ruleDescription_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// IsRuleEnabled_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate rule.
        /// </summary>
        public bool IsRuleEnabled_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate
        {
            get => this.isRuleEnabled_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate;

            set => this.isRuleEnabled_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// IsRuleEnabled_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate rule description.
        /// </summary>
        public string RuleDescription_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate
        {
            get => this.ruleDescription_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate;

            set => this.ruleDescription_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseWOOLWICH_Code_128_RemoveSpaces rule.
        /// </summary>
        public bool IsRuleEnabled_ParseWOOLWICH_Code_128_RemoveSpaces
        {
            get => this.isRuleEnabled_ParseWOOLWICH_Code_128_RemoveSpaces;

            set => this.isRuleEnabled_ParseWOOLWICH_Code_128_RemoveSpaces = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseWOOLWICH_Code_128_RemoveSpaces rule description.
        /// </summary>
        public string RuleDescription_ParseWOOLWICH_Code_128_RemoveSpaces
        {
            get => this.ruleDescription_ParseWOOLWICH_Code_128_RemoveSpaces;

            set => this.ruleDescription_ParseWOOLWICH_Code_128_RemoveSpaces = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseCode_128AsUCC_EAN_128_SymbolCompliant rule.
        /// </summary>
        public bool IsRuleEnabled_ParseCode_128AsUCC_EAN_128_SymbolCompliant
        {
            get => this.isRuleEnabled_ParseCode_128AsUCC_EAN_128_SymbolCompliant;

            set => this.isRuleEnabled_ParseCode_128AsUCC_EAN_128_SymbolCompliant = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseCode_128AsUCC_EAN_128_SymbolCompliant rule
        /// description.
        /// </summary>
        public string RuleDescription_ParseCode_128AsUCC_EAN_128_SymbolCompliant
        {
            get => this.ruleDescription_ParseCode_128AsUCC_EAN_128_SymbolCompliant;

            set => this.ruleDescription_ParseCode_128AsUCC_EAN_128_SymbolCompliant = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseFINICA_Code_128_as_EAN_UCC_128 rule.
        /// </summary>
        public bool IsRuleEnabled_ParseFINICA_Code_128_as_EAN_UCC_128
        {
            get => this.isRuleEnabled_ParseFINICA_Code_128_as_EAN_UCC_128;

            set => this.isRuleEnabled_ParseFINICA_Code_128_as_EAN_UCC_128 = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseFINICA_Code_128_as_EAN_UCC_128 description rule.
        /// </summary>
        public string RuleDescription_ParseFINICA_Code_128_as_EAN_UCC_128
        {
            get => this.ruleDescription_ParseFINICA_Code_128_as_EAN_UCC_128;

            set => this.ruleDescription_ParseFINICA_Code_128_as_EAN_UCC_128 = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_NoCheckDigitInGlobalLocationNumber rule.
        /// </summary>
        public bool IsRuleEnabled_NoCheckDigitInGlobalLocationNumber
        {
            get => this.isRuleEnabled_NoCheckDigitInGlobalLocationNumber;

            set => this.isRuleEnabled_NoCheckDigitInGlobalLocationNumber = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_NoCheckDigitInGlobalLocationNumber rule description.
        /// </summary>
        public string RuleDescription_NoCheckDigitInGlobalLocationNumber
        {
            get => this.ruleDescription_NoCheckDigitInGlobalLocationNumber;

            set => this.ruleDescription_NoCheckDigitInGlobalLocationNumber = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family rule.
        /// </summary>
        public bool IsRuleEnabled_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family
        {
            get => this.isRuleEnabled_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family;

            set => this.isRuleEnabled_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family rule
        /// description.
        /// </summary>
        public string RuleDescription_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family
        {
            get => this.ruleDescription_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family;

            set => this.ruleDescription_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_OvationFielding_Lamb rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_Code128_OvationFielding_Lamb
        {
            get => this.isRuleEnabled_Parse_Code128_OvationFielding_Lamb;

            set => this.isRuleEnabled_Parse_Code128_OvationFielding_Lamb = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_OvationFielding_Lamb rule description.
        /// </summary>
        public string RuleDescription_Parse_Code128_OvationFielding_Lamb
        {
            get => this.ruleDescription_Parse_Code128_OvationFielding_Lamb;

            set => this.ruleDescription_Parse_Code128_OvationFielding_Lamb = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Renees_Code_128_RemoveSpaces rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_Renees_Code_128_RemoveSpaces
        {
            get => this.isRuleEnabled_Parse_Renees_Code_128_RemoveSpaces;

            set => this.isRuleEnabled_Parse_Renees_Code_128_RemoveSpaces = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Renees_Code_128_RemoveSpaces rule description.
        /// </summary>
        public string RuleDescription_Parse_Renees_Code_128_RemoveSpaces
        {
            get => this.ruleDescription_Parse_Renees_Code_128_RemoveSpaces;

            set => this.ruleDescription_Parse_Renees_Code_128_RemoveSpaces = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_LanzMeats_Code_128_AI10_as_AI21 rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_LanzMeats_Code_128_AI10_as_AI21
        {
            get => this.isRuleEnabled_ParseLanzMeats_Code_128_AI10_as_AI21;

            set => this.isRuleEnabled_ParseLanzMeats_Code_128_AI10_as_AI21 = value;
        }

        /// <summary>
        /// Gets or sets a value IsRuleEnabled_Parse_LanzMeats_Code_128_AI10_as_AI21 rule description.
        /// </summary>
        public string RuleDescription_Parse_LanzMeats_Code_128_AI10_as_AI21
        {
            get => this.ruleDescription_ParseLanzMeats_Code_128_AI10_as_AI21;

            set => this.ruleDescription_ParseLanzMeats_Code_128_AI10_as_AI21 = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// IsRuleEnabled_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit rule.
        /// </summary>
        public bool IsRuleEnabled_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit
        {
            get => this.isRuleEnabled_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit;

            set => this.isRuleEnabled_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit = value;
        }

        /// <summary>
        /// Gets or sets a value HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit rule description.
        /// </summary>
        public string RuleDescription_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit
        {
            get => this.ruleDescription_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit;

            set => this.ruleDescription_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// IsRuleEnabled_ParseNossackBrokerMultiNationalProductFromChili rule.
        /// </summary>
        public bool IsRuleEnabled_ParseNossackBrokerMultiNationalProductFromChili
        {
            get => this.isRuleEnabled_ParseNossackBrokerMultiNationalProductFromChili;

            set => this.isRuleEnabled_ParseNossackBrokerMultiNationalProductFromChili = value;
        }

        /// <summary>
        /// Gets or sets a valueParseNossackBrokerMultiNationalProductFromChili rule description.
        /// </summary>
        public string RuleDescription_ParseNossackBrokerMultiNationalProductFromChili
        {
            get => this.ruleDescription_ParseNossackBrokerMultiNationalProductFromChili;

            set => this.ruleDescription_ParseNossackBrokerMultiNationalProductFromChili = value;
        }

        /// <summary>
        /// Gets or sets if this will read the Nossack Greenlea Premier Meats barcodes.
        /// </summary>
        public bool IsRuleEnabled_ParseEAN_UCC128_Nossack_GreenleaPremierMeats
        {
            get => this.isRuleEnabled_ParseEAN_UCC128_Nossack_GreenleaPremierMeats;

            set => this.isRuleEnabled_ParseEAN_UCC128_Nossack_GreenleaPremierMeats = value;
        }

        /// <summary>
        /// Gets or sets a rule description for Nossack Greenlea Premier Meats barcode rule.
        /// </summary>
        public string RuleDescription_ParseEAN_UCC128_Nossack_GreenleaPremierMeats
        {
            get => this.ruleDescription_ParseEAN_UCC128_Nossack_GreenleaPremierMeats;

            set => this.ruleDescription_ParseEAN_UCC128_Nossack_GreenleaPremierMeats = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_UCC_13_as_UPC_12_UPC_E rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_UCC_13_as_UPC_12_UPC_E
        {
            get => this.isRuleEnabled_Parse_UCC_13_as_UPC_12_UPC_E;

            set => this.isRuleEnabled_Parse_UCC_13_as_UPC_12_UPC_E = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_UCC_13_as_UPC_12_UPC_E rule description.
        /// </summary>
        public string RuleDescription_Parse_UCC_13_as_UPC_12_UPC_E_Enabled
        {
            get => this.ruleDescription_Parse_UCC_13_as_UPC_12_UPC_E_Enabled;

            set => this.ruleDescription_Parse_UCC_13_as_UPC_12_UPC_E_Enabled = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_HelaSpiceCanada_Code_128 rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_HelaSpiceCanada_Code_128
        {
            get => this.isRuleEnabled_Parse_HelaSpiceCanada_Code_128;

            set => this.isRuleEnabled_Parse_HelaSpiceCanada_Code_128 = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_HelaSpiceCanada_Code_128 rule description.
        /// </summary>
        public string RuleDescription_Parse_HelaSpiceCanada_Code_128
        {
            get => this.ruleDescription_Parse_HelaSpiceCanada_Code_128;

            set => this.ruleDescription_Parse_HelaSpiceCanada_Code_128 = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_HelaSpiceCanada_Code_128 rule.
        /// </summary>
        public bool IsRuleEnabled_NebraskaBeef_Parse_Lot_as_Serial_Code_128
        {
            get => this.isRuleEnabled_NebraskaBeef_Parse_Lot_as_Serial_Code_128;

            set => this.isRuleEnabled_NebraskaBeef_Parse_Lot_as_Serial_Code_128 = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_HelaSpiceCanada_Code_128 rule description.
        /// </summary>
        public string RuleDescription_NebraskaBeef_Parse_Lot_as_Serial_Code_128
        {
            get => this.ruleDescription_NebraskaBeef_Parse_Lot_as_Serial_Code_128;

            set => this.ruleDescription_NebraskaBeef_Parse_Lot_as_Serial_Code_128 = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseNorthernCoop_AustraliaBeefTrim_Product25612 rule.
        /// </summary>
        public bool IsRuleEnabled_ParseNorthernCoop_AustraliaBeefTrim_Product25612
        {
            get => this.isRuleEnabled_ParseNorthernCoop_AustraliaBeefTrim_Product25612;

            set => this.isRuleEnabled_ParseNorthernCoop_AustraliaBeefTrim_Product25612 = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether ParseNorthernCoop_AustraliaBeefTrim_Product25612 description.
        /// </summary>
        public string RuleDescription_ParseNorthernCoop_AustraliaBeefTrim_Product25612
        {
            get => this.ruleDescription_ParseNorthernCoop_AustraliaBeefTrim_Product25612;

            set => this.ruleDescription_ParseNorthernCoop_AustraliaBeefTrim_Product25612 = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct rule.
        /// </summary>
        public bool IsRuleEnabled_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct
        {
            get => this.isRuleEnabled_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct;

            set => this.isRuleEnabled_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct description.
        /// </summary>
        public string RuleDescription_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct
        {
            get => this.ruleDescription_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct;

            set => this.ruleDescription_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs rule.
        /// </summary>
        public bool IsRuleEnabled_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs
        {
            get => this.isRuleEnabled_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs;

            set => this.isRuleEnabled_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs = value;
        }

        /// <summary>
        /// Gets or sets the ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs description.
        /// </summary>
        public string RuleDescription_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs
        {
            get => this.ruleDescription_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs;

            set => this.ruleDescription_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether enables AI(02) to be read as AI(01) and AI(15) fix of DDMMYY -> YYMMDD.
        /// </summary>
        public bool IsRuleEnabled_ParseEAN_UCC128_Dovre_KiiNaturalsCrisps
        {
            get => this.isRuleEnabled_ParseEAN_UCC128_Dovre_KiiNaturalsCrisps;

            set => this.isRuleEnabled_ParseEAN_UCC128_Dovre_KiiNaturalsCrisps = value;
        }

        /// <summary>
        /// Gets or sets the Enables AI(02) to be read as AI(01) and AI(15) fix of DDMMYY -> YYMMDD description.
        /// </summary>
        public string RuleDescription_ParseEAN_UCC128_Dovre_KiiNaturalsCrisps
        {
            get => this.ruleDescription_ParseEAN_UCC128_Dovre_KiiNaturalsCrisps;

            set => this.ruleDescription_ParseEAN_UCC128_Dovre_KiiNaturalsCrisps = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00 rule.
        /// </summary>
        public bool IsRuleEnabled_ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00
        {
            get => this.isRuleEnabled_ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00;

            set => this.isRuleEnabled_ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00 = value;
        }

        /// <summary>
        /// Gets or sets the ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00 description.
        /// description.
        /// </summary>
        public string RuleDescription_ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00
        {
            get => this.ruleDescription_ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00;

            set => this.ruleDescription_ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00 = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether when enabled it will suppress error messages for non-tracked fields.
        /// This allows the bar code to be fully parsed, even when a field or two is missing as long
        /// as they are non-tracked.
        /// </summary>
        public bool IsRuleEnabled_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents
        {
            get => this.isRuleEnabled_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents;

            set => this.isRuleEnabled_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents = value;
        }

        /// <summary>
        /// Gets or sets when enabled it will suppress error messages for non-tracked fields description.
        /// This allows the bar code to be fully parsed, even when a field or two is missing as long
        /// as they are non-tracked.
        /// </summary>
        public string RuleDescription_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents
        {
            get => this.ruleDescription_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents;

            set => this.ruleDescription_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether strips invalid character 0x001A from the beginning of the string.
        /// </summary>
        public bool IsRuleEnabled_Parse_Code128_CleanSubstituteCharacter
        {
            get => this.isRuleEnabled_Parse_Code128_CleanSubstituteCharacter;

            set => this.isRuleEnabled_Parse_Code128_CleanSubstituteCharacter = value;
        }

        /// <summary>
        /// Gets or sets the strips invalid character 0x001A from the beginning of the string.
        /// </summary>
        public string RuleDescription_Parse_Code128_CleanSubstituteCharacter
        {
            get => this.ruleDescription_Parse_Code128_CleanSubstituteCharacter;

            set => this.ruleDescription_Parse_Code128_CleanSubstituteCharacter = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether parse EAN / UCC Code 128 Bourvy Exports production date from the
        /// AI(21) serial number.
        /// </summary>
        public bool IsRuleEnabled_ParseEAN_UCC128_BourvyExports_ProductionDate
        {
            get => this.isRuleEnabled_ParseEAN_UCC128_BourvyExports_ProductionDate;

            set => this.isRuleEnabled_ParseEAN_UCC128_BourvyExports_ProductionDate = value;
        }

        /// <summary>
        /// Gets or sets the parse EAN / UCC Code 128 Bourvy Exports production date from the AI(21) serial number string.
        /// </summary>
        public string RuleDescription_ParseEAN_UCC128_BourvyExports_ProductionDate
        {
            get => this.ruleDescription_ParseEAN_UCC128_BourvyExports_ProductionDate;

            set => this.ruleDescription_ParseEAN_UCC128_BourvyExports_ProductionDate = value;
        }

        /// <summary>
        /// Request the bar code settings data and save the file.
        /// </summary>
        public void RequestBarCodeSettings()
        {
            var msgEventArg = new BarCodeSettingsEventArgs();

            try
            {
                var responseData = string.Empty;

                if (this.OnBarCodeSettingsStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Seeding bar code settings.";
                    this.OnBarCodeSettingsStatusOut(this, msgEventArg);
                }

                responseData = SysCommon.SubmitComm(this.CreateXMLRequestBarCodeSettings());
                this.Save(responseData);

                if (this.OnBarCodeSettingsStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Seeding bar code settings complete.";
                    this.OnBarCodeSettingsStatusOut(this, msgEventArg);
                }
            }
            catch (Exception ex)
            {
                if (this.OnBarCodeSettingsStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Seeding bar code settings failed.";
                    this.OnBarCodeSettingsStatusOut(this, msgEventArg);
                }

                throw new Exception("Seeding bar code settings failed.", ex);
            }
        }

        /// <summary>
        /// Load and object of type BarCodeSettings from a serialized xml file.
        /// </summary>
        /// <returns><see cref="SysBarCodeSettings"/> Object of type BarCodeSettings.</returns>
        public SysBarCodeSettings LoadData()
        {
            lock (this.readLockObject)
            {
                var serializer = new XmlSerializer(typeof(SysBarCodeSettings));
                var barCodeSettings = new SysBarCodeSettings();

                // use reflection to get the current assembly path for the database
                var appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var fullPathToMyFile = Path.Combine(appDir, this.FileName);

                if (!File.Exists(fullPathToMyFile))
                {
                    this.SaveData(ref barCodeSettings);
                }

                // Create a FileStream to read with.
                using (var reader = new FileStream(fullPathToMyFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // Deserialize the stream to the object, and close the TextReader
                    barCodeSettings = (SysBarCodeSettings)serializer.Deserialize(reader);
                }

                return barCodeSettings;
            }
        }

        /// <summary>
        /// Serialize and save the data into a xml file.
        /// </summary>
        /// <param name="barCodeSettings"> Instance of BarCodeSettings to serialize.</param>
        /// <returns><see cref="bool"/> True if save was successful else false.</returns>
        public bool SaveData(ref SysBarCodeSettings barCodeSettings)
        {
            lock (this.writeLockObject)
            {
                var serializer = new XmlSerializer(typeof(SysBarCodeSettings));
                var isSaveComplete = false;

                try
                {
                    // use reflection to get the current assembly path for the database
                    var appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    var fullPathToMyFile = Path.Combine(appDir, this.FileName);

                    // Create a FileStream to write with.
                    using (var writer = new FileStream(
                        fullPathToMyFile,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None))
                    {
                        // Serialize the object, and close the TextWriter
                        serializer.Serialize(writer, barCodeSettings);
                    }

                    isSaveComplete = true;
                }
                catch
                {
                    isSaveComplete = false;
                }

                return isSaveComplete;
            }
        }

        /// <summary>
        /// Register bar code settings status out listener.
        /// </summary>
        /// <param name="barCodeSettingsEventListener"><see cref="IBarCodeSettingsEventListener"/> Bar code settings
        /// event listener.</param>
        public void RegisterBarCodeSettingsListenerStatusOut(IBarCodeSettingsEventListener barCodeSettingsEventListener)
        {
            this.OnBarCodeSettingsStatusOut += new OnBarCodeSettingsStatusOutEventHandler(
                barCodeSettingsEventListener.OnBarCodeSettingsStatusOut);
        }

        /// <summary>
        /// Unregister bar code settings status out listener.
        /// </summary>
        /// <param name="barCodeSettingsEventListener"><see cref="IBarCodeSettingsEventListener"/> Bar code settings
        /// event listener.</param>
        public void UnregisterBarCodeSettingsListenerStatusOut(IBarCodeSettingsEventListener barCodeSettingsEventListener)
        {
            this.OnBarCodeSettingsStatusOut -= new OnBarCodeSettingsStatusOutEventHandler(
                barCodeSettingsEventListener.OnBarCodeSettingsStatusOut);
        }

        /// <summary>
        /// Register bar code settings event listener.
        /// </summary>
        /// <param name="barCodeSettingsEventListener"><see cref="IBarCodeSettingsEventListener"/> Bar code settings
        /// event listener.</param>
        public void RegisterBarCodeSettingsListenerProgressOut(IBarCodeSettingsEventListener barCodeSettingsEventListener)
        {
            this.OnBarCodeSettingsProgressOut += new OnBarCodeSettingsProgressOutEventHandler(
                barCodeSettingsEventListener.OnBarCodeSettingsProgressOut);
        }

        /// <summary>
        /// Unregister bar code settings event listener.
        /// </summary>
        /// <param name="barCodeSettingsEventListener"><see cref="IBarCodeSettingsEventListener"/> Bar code settings
        /// event listener.</param>
        public void UnregisterBarCodeSettingsListenerProgressOut(IBarCodeSettingsEventListener barCodeSettingsEventListener)
        {
            this.OnBarCodeSettingsProgressOut -= new OnBarCodeSettingsProgressOutEventHandler(
                barCodeSettingsEventListener.OnBarCodeSettingsProgressOut);
        }

        /// <summary>
        /// Process the xml data from the communication server and store it.
        /// </summary>
        /// <param name="xmlData"> Xml data received from the communication server.</param>
        private void Save(string xmlData)
        {
            var msgEventArg = new BarCodeSettingsEventArgs();

            try
            {
                using var ds = new DataSet();
                if (this.OnBarCodeSettingsStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Reading received data.";
                    this.OnBarCodeSettingsStatusOut(this, msgEventArg);
                }

                ds.ReadXml(SysCommon.GetXmlReader(ref xmlData), XmlReadMode.Auto);

                if (this.OnBarCodeSettingsStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Saving bar code settings.";
                    this.OnBarCodeSettingsStatusOut(this, msgEventArg);
                }

                if (ds.Tables.Count != 0)
                {
                    var progMax = ds.Tables["tBarCodeSettings"].Rows.Count;

                    if (this.OnBarCodeSettingsStatusOut != null)
                    {
                        msgEventArg.StatusOutMsg = $"Records {progMax}.";
                        this.OnBarCodeSettingsStatusOut(this, msgEventArg);
                    }

                    var progValue = 0;
                    if (this.OnBarCodeSettingsProgressOut != null)
                    {
                        msgEventArg.CurrentValue = 0;
                        msgEventArg.MinValue = 0;
                        msgEventArg.MaxValue = progMax;
                        this.OnBarCodeSettingsProgressOut(this, msgEventArg);
                    }

                    foreach (DataRow drow in ds.Tables["tBarCodeSettings"].Rows)
                    {
                        var rule = (SysCommon.BarCodeRules)Convert.ToInt32(drow["BarCodeSettingsRuleFKRefno"]);
                        var enabled = Convert.ToBoolean(drow["BarCodeSettingsRuleEnabled"]);
                        var description = Convert.ToString(drow["BarCodeSettingsRuleDescription"]);
                        switch (rule)
                        {
                            case SysCommon.BarCodeRules.ParseCode_128AsUCC_EAN_128_SymbolCompliant:
                                this.IsRuleEnabled_ParseCode_128AsUCC_EAN_128_SymbolCompliant = enabled;
                                this.RuleDescription_ParseCode_128AsUCC_EAN_128_SymbolCompliant = description;
                                break;

                            case SysCommon.BarCodeRules.ParseFINICA_Code_128_as_EAN_UCC_128:
                                this.IsRuleEnabled_ParseFINICA_Code_128_as_EAN_UCC_128 = enabled;
                                this.RuleDescription_ParseFINICA_Code_128_as_EAN_UCC_128 = description;
                                break;

                            case SysCommon.BarCodeRules.NoCheckDigitInGlobalLocationNumber:
                                this.IsRuleEnabled_NoCheckDigitInGlobalLocationNumber = enabled;
                                this.RuleDescription_NoCheckDigitInGlobalLocationNumber = description;
                                break;

                            case SysCommon.BarCodeRules.NoZeroPaddingOfProductIdentifierForUCC_EAN_Family:
                                this.IsRuleEnabled_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family = enabled;
                                this.RuleDescription_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family = description;
                                break;

                            case SysCommon.BarCodeRules.ParseWOOLWICH_Code_128_RemoveSpaces:
                                this.IsRuleEnabled_ParseWOOLWICH_Code_128_RemoveSpaces = enabled;
                                this.RuleDescription_ParseWOOLWICH_Code_128_RemoveSpaces = description;
                                break;

                            case SysCommon.BarCodeRules.Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate:
                                this.IsRuleEnabled_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate = enabled;
                                this.RuleDescription_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate = description;
                                break;

                            case SysCommon.BarCodeRules.Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate:
                                this.IsRuleEnabled_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate = enabled;
                                this.RuleDescription_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate = description;
                                break;

                            case SysCommon.BarCodeRules.Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount:
                                this.IsRuleEnabled_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount = enabled;
                                this.RuleDescription_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount = description;
                                break;

                            case SysCommon.BarCodeRules.ProductBarCodeMinimumLength4Digits:
                                this.IsRuleEnabled_ProductBarCodeMinimumLengthRequirement4Digits = enabled;
                                this.RuleDescription_ProductBarCodeMinimumLengthRequirement4Digits = description;
                                break;

                            case SysCommon.BarCodeRules.Parse_UCC_12_UPC_E_as_Deli_PLU:
                                this.IsRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU = enabled;
                                this.RuleDescription_Parse_UCC_12_UPC_E_Deli_PLU_Enabled = description;
                                break;

                            case SysCommon.BarCodeRules.Parse_Code128_OvationFielding_Lamb:
                                this.IsRuleEnabled_Parse_Code128_OvationFielding_Lamb = enabled;
                                this.RuleDescription_Parse_Code128_OvationFielding_Lamb = description;
                                break;

                            case SysCommon.BarCodeRules.Parse_Code128_China_Seafood:
                                this.IsRuleEnabled_Parse_Code128_China_Seafood = enabled;
                                this.RuleDescription_Parse_Code128_China_Seafood = description;
                                break;

                            case SysCommon.BarCodeRules.Parse_Code_128_AI01_BC_AI30_exclusive:
                                this.IsRuleEnabled_Parse_Code_128_AI01_BC_AI30_exclusive = enabled;
                                this.RuleDescription_Parse_Code_128_AI01_BC_AI30_exclusive = description;
                                break;

                            case SysCommon.BarCodeRules.ParseRenees_Code_128_RemoveSpaces:
                                this.IsRuleEnabled_Parse_Renees_Code_128_RemoveSpaces = enabled;
                                this.RuleDescription_Parse_Renees_Code_128_RemoveSpaces = description;
                                break;

                            case SysCommon.BarCodeRules.ParseLanzMeats_Code_128_AI10_as_AI21:
                                this.IsRuleEnabled_Parse_LanzMeats_Code_128_AI10_as_AI21 = enabled;
                                this.RuleDescription_Parse_LanzMeats_Code_128_AI10_as_AI21 = description;
                                break;

                            case SysCommon.BarCodeRules.HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit:
                                this.IsRuleEnabled_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit = enabled;
                                this.RuleDescription_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit = description;
                                break;

                            case SysCommon.BarCodeRules.ParseNossackBrokerMultiNationalProductFromChili:
                                this.IsRuleEnabled_ParseNossackBrokerMultiNationalProductFromChili = enabled;
                                this.RuleDescription_ParseNossackBrokerMultiNationalProductFromChili = description;
                                break;

                            case SysCommon.BarCodeRules.ParseUCC_13asUPC_12_UPC_E:
                                this.IsRuleEnabled_Parse_UCC_13_as_UPC_12_UPC_E = enabled;
                                this.RuleDescription_Parse_UCC_13_as_UPC_12_UPC_E_Enabled = description;
                                break;

                            case SysCommon.BarCodeRules.Parse_HelaSpiceCanada_Code_128:
                                this.IsRuleEnabled_Parse_HelaSpiceCanada_Code_128 = enabled;
                                this.RuleDescription_Parse_HelaSpiceCanada_Code_128 = description;
                                break;

                            case SysCommon.BarCodeRules.NebraskaBeef_Parse_Lot_as_Serial_Code_128:
                                this.isRuleEnabled_NebraskaBeef_Parse_Lot_as_Serial_Code_128 = enabled;
                                this.RuleDescription_NebraskaBeef_Parse_Lot_as_Serial_Code_128 = description;
                                break;

                            case SysCommon.BarCodeRules.ParseNorthernCoop_AustraliaBeefTrim_Product25612:
                                this.IsRuleEnabled_ParseNorthernCoop_AustraliaBeefTrim_Product25612 = enabled;
                                this.RuleDescription_ParseNorthernCoop_AustraliaBeefTrim_Product25612 = description;
                                break;

                            case SysCommon.BarCodeRules.ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct:
                                this.IsRuleEnabled_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct = enabled;
                                this.RuleDescription_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct = description;
                                break;

                            case SysCommon.BarCodeRules.ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs:
                                this.IsRuleEnabled_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs = enabled;
                                this.RuleDescription_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs = description;
                                break;

                            case SysCommon.BarCodeRules.ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00:
                                this.IsRuleEnabled_ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00 = enabled;
                                this.RuleDescription_ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00 = description;
                                break;

                            case SysCommon.BarCodeRules.ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents:
                                this.IsRuleEnabled_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents = enabled;
                                this.RuleDescription_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents = description;
                                break;

                            case SysCommon.BarCodeRules.ParseEAN_UCC128_Nossack_GreenleaPremierMeats:
                                this.IsRuleEnabled_ParseEAN_UCC128_Nossack_GreenleaPremierMeats = enabled;
                                this.RuleDescription_ParseEAN_UCC128_Nossack_GreenleaPremierMeats = description;
                                break;

                            case SysCommon.BarCodeRules.ParseEAN_UCC128_Dovre_KiiNaturalsCrisps:
                                this.IsRuleEnabled_ParseEAN_UCC128_Dovre_KiiNaturalsCrisps = enabled;
                                this.RuleDescription_ParseEAN_UCC128_Dovre_KiiNaturalsCrisps = description;
                                break;

                            case SysCommon.BarCodeRules.Parse_Code128_CleanSubstituteCharacter:
                                this.IsRuleEnabled_Parse_Code128_CleanSubstituteCharacter = enabled;
                                this.RuleDescription_Parse_Code128_CleanSubstituteCharacter = description;
                                break;

                            case SysCommon.BarCodeRules.Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount_Enabled:
                                this.IsRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount = enabled;
                                this.RuleDescription_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount_Enabled = description;
                                break;

                            case SysCommon.BarCodeRules.ParseEAN_UCC128_BourvyExports_ProductionDate:
                                this.IsRuleEnabled_ParseEAN_UCC128_BourvyExports_ProductionDate = enabled;
                                this.RuleDescription_ParseEAN_UCC128_BourvyExports_ProductionDate = description;
                                break;

                            default:
                                throw new NotImplementedException("Bar code rule setting not defined.");
                        }

                        progValue++;
                        if (this.OnBarCodeSettingsProgressOut != null)
                        {
                            msgEventArg.CurrentValue = progValue;
                            msgEventArg.MinValue = 0;
                            msgEventArg.MaxValue = progMax;
                            this.OnBarCodeSettingsProgressOut(this, msgEventArg);
                        }
                    } // foreach bar code rule

                    if (this.OnBarCodeSettingsProgressOut != null)
                    {
                        msgEventArg.CurrentValue = progMax;
                        msgEventArg.MinValue = 0;
                        msgEventArg.MaxValue = progMax;
                        this.OnBarCodeSettingsProgressOut(this, msgEventArg);
                    }
                }

                if (this.OnBarCodeSettingsStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Saving bar code settings complete.";
                    this.OnBarCodeSettingsStatusOut(this, msgEventArg);
                }
            }
            catch (Exception ex)
            {
                if (this.OnBarCodeSettingsStatusOut != null)
                {
                    msgEventArg.StatusOutMsg = "Saving bar code settings failed.";
                    this.OnBarCodeSettingsStatusOut(this, msgEventArg);
                }

                throw new Exception("Saving bar code settings failed.", ex);
            }
        }

        /// <summary>
        /// Create the xml request string to send to the communication server.
        /// </summary>
        /// <returns><see cref="string"/> String of the xml request to get the bar code settings.</returns>
        private string CreateXMLRequestBarCodeSettings()
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);
            var revision = new DateTime(1900, 1, 1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var sysEnv = SysEnvironment.GetInstance();

            var request = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                string.Empty,
                new XElement(
                    "imclient",
                    new XElement(
                        "command",
                        new XElement("action", "REQUEST BAR CODE SETTINGS"),
                        new XElement("device_id", sysEnv.DeviceId),
                        new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                        new XElement("revision_datetime", revision))));

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
