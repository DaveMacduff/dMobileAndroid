namespace dMobile.Bedrock
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Microsoft.VisualBasic;

    /// <summary>
    /// Bar Code Settings Class
    /// </summary>
    [Serializable]
    public class SysBarCodeSettings : IBarCodeSettingsEventPublisher
    {
        #region Fields

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

        [NonSerialized]
        private readonly object writeLockObject = new object();

        [NonSerialized]
        private readonly object readLockObject = new object();

        /// <summary>
        /// Rule: Sometimes a bar code is fully EAN/UCC 128 Symbol parsable EXCEPT the actual symbology is Code 128
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Parsable and Symbology is correct")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_ParseCode_128AsUCC_EAN_128_SymbolCompliant = false;

        /// <summary>
        /// Rule Description: Sometimes a bar code is fully EAN/UCC 128 Symbol parsable EXCEPT the actual symbology is 
        /// Code 128
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Parsable and Symbology is correct")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_ParseCode_128AsUCC_EAN_128_SymbolCompliant = string.Empty;

        /// <summary>
        /// Rule: Vendor is FINICA FOOD SPECIALITIES 
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_ParseFINICA_Code_128_as_EAN_UCC_128 = false;

        /// <summary>
        /// Rule Description: Vendor is FINICA FOOD SPECIALITIES 
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_ParseFINICA_Code_128_as_EAN_UCC_128 = string.Empty;

        /// <summary>
        /// Rule: GS1 UCC/EAN-128 AI(414) Global Location Number should have a check digit on the end (414)  9 
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_NoCheckDigitInGlobalLocationNumber = false;

        /// <summary>
        /// Rule Description: GS1 UCC/EAN-128 AI(414) Global Location Number should have a check digit on the end (414)  9 
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_NoCheckDigitInGlobalLocationNumber = string.Empty;

        /// <summary>
        /// Rule: No zero padding of product identifier for UCC/EAN family.
        /// </summary>
        /// <remarks>
        /// GTIN-12 (UPC)
        /// GTIN-13 (EAN-13)
        /// GTIN-14 (EAN/UCC-128 or ITF-14)
        /// GTIN-8 (EAN-8)
        /// </remarks>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family;

        /// <summary>
        /// Rule Description: No zero padding of product identifier for UCC/EAN family.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family;

        /// <summary>
        /// Rule: Vendor is WOOLWICH DAIRY (cheese products) Bar Code is a UCC/EAN 128 symbology with 4 spaces in the GTIN, 
        /// no other AI's exist. Length is 18 
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Symbology is correct")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_ParseWOOLWICH_Code_128_RemoveSpaces = false;

        /// <summary>
        /// Rule Description: Vendor is WOOLWICH DAIRY (cheese products) Bar Code is a UCC/EAN 128 symbology with 4 spaces 
        /// in the GTIN, no other AI's exist. Length is 18 
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Symbology is correct")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_ParseWOOLWICH_Code_128_RemoveSpaces = string.Empty;

        /// <summary>
        /// Rule: Recognize Code 128 AI(13) Packaging Date as a AI(11) Production Date   
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate = false;

        /// <summary>
        /// Rule Description: Recognize Code 128 AI(13) Packaging Date as a AI(11) Production Date   
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate = string.Empty;

        /// <summary>
        /// Rule: Recognize Code 128 AI(17) Expiry Date as a AI(15) Best Before Date 
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool ruleEnabled_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate = false;

        /// <summary>
        /// Rule Description: Recognize Code 128 AI(17) Expiry Date as a AI(15) Best Before Date 
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate = string.Empty;

        /// <summary>
        /// Rule: Parse EAN/UCC Code 128 AI(01) with Nossack Company Prefix, AI(21) contains Skid Number and Box Count
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Nossack is correct")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool ruleEnabled_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount = false;

        /// <summary>
        /// Rule Description: Parse EAN/UCC Code 128 AI(01) with Nossack Company Prefix, AI(21) contains Skid Number and 
        /// Box Count
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Nossack is correct")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount = string.Empty;

        /// <summary>
        /// Rule: Product Bar Code Minimum Length Requirement 4 Digits 
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_ProductBarCodeMinimumLengthRequirement4Digits = false;

        /// <summary>
        /// Rule Description: Product Bar Code Minimum Length Requirement 4 Digits 
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_ProductBarCodeMinimumLengthRequirement4Digits = string.Empty;

        /// <summary>
        /// Rule: Enable POS PLU parsing UCC-12 / UPC-E
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU = false;

        /// <summary>
        /// Rule Description: Enable POS PLU parsing UCC-12 / UPC-E
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_Parse_UCC_12_UPC_E_Deli_PLU_Enabled = string.Empty;

        /// <summary>
        /// Rule: Enable to restrict deli scale amount to the last 4 digits. ie ignore the hundreds digit.
        /// This restricts amounts to the [0,99.99] range.
        /// </summary>
        /// <remarks>
        /// This was written for Prairie's bad label printing which creates bad amounts over 99.99.
        /// </remarks>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount = false;

        /// <summary>
        /// Rule Description: Parse UCC_12_UPC_E as Deli PLU 4 Digit Amount. Restricts amount to the 0-99.99 range.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
          Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount_Enabled = string.Empty;

        /// <summary>
        /// Rule: New Zeland Lamb Ovation Fielding parsing
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_Parse_Code128_OvationFielding_Lamb = false;

        /// <summary>
        /// Rule Description: New Zeland Lamb Ovation Fielding parsing
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_Parse_Code128_OvationFielding_Lamb = string.Empty;

        /// <summary>
        /// Rule: Parse China seafood EAN/UCC Code 128, length is 14, data starts with '30'
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_Parse_Code128_China_Seafood = false;

        /// <summary>
        /// Rule Description: Parse China seafood EAN/UCC Code 128, length is 14, data starts with '30'
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_Parse_Code128_China_Seafood = string.Empty;

        /// <summary>
        /// Rule: Parse EAN/UCC Code 128 AI(01) with B&C Company Prefix, AI(30) contains quantity of units, exclusive.
        /// Rule changed to ignore company prefix. Oct 20/2014.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_Parse_Code_128_AI01_BC_AI30_exclusive = false;

        /// <summary>
        /// Rule Description: Parse EAN/UCC Code 128 AI(01) with B&C Company Prefix, AI(30) contains quantity of units, 
        /// exclusive.
        /// Rule changed to ignore the company prefix. Oct 20/2014.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_Parse_Code_128_AI01_BC_AI30_exclusive = string.Empty;

        /// <summary>
        /// Rule: Vendor is Renee's Gourmet Bar Code is a UCC/EAN 128 symbology with 4 spaces in the GTIN, 
        /// no other AI's exist. Length is 18 
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_Parse_Renees_Code_128_RemoveSpaces = false;

        /// <summary>
        /// Rule Description: Vendor is Renee's Bar Code is a UCC/EAN 128 symbology with 4 spaces in the GTIN, 
        /// no other AI's exist. Length is 18 
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_Parse_Renees_Code_128_RemoveSpaces = string.Empty;

        /// <summary>
        /// Rule: Vendor is Lanz Meats, Bar Code is a UCC/EAN 128 symbology, AI(10) Lot Number will become 
        /// AI(21) Serial Number.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_ParseLanzMeats_Code_128_AI10_as_AI21 = false;

        /// <summary>
        /// Rule Description: Vendor is Lanz Meats, Bar Code is a UCC/EAN 128 symbology, AI(10) Lot Number will become 
        /// AI(21) Serial Number.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_ParseLanzMeats_Code_128_AI10_as_AI21 = string.Empty;

        /// <summary>
        /// Rule: Heritage Global Location Identifier do a second pass ignoring the check digit.
        /// </summary>
        /// <remarks>A check digit calculation flaw cause the the global location identifiers check digits to be printed
        /// incorrectly. We needed a way to ignore them. This is a temporary solution, until new location
        /// indenitfers can be printed with the corrected check digits.</remarks>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit = false;

        /// <summary>
        /// Rule Description: Heritage Global Location Identifier do a second pass ignoring the check digit. 
        /// </summary>
        /// <remarks>A check digit calculation flaw cause the the global location identifiers check digits to be printed
        /// incorrectly. We needed a way to ignore them. This is a temporary solution, until new location
        /// indenitfers can be printed with the corrected check digits.</remarks>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit = string.Empty;

        /// <summary>
        /// Rule: Parse Nossack Broker Multi National Product From Chili Company Prefix 896775.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_ParseNossackBrokerMultiNationalProductFromChili = false;

        /// <summary>
        /// Rule Description: Parse Nossack Broker Multi National Product From Chili Company Prefix 896775. 
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_ParseNossackBrokerMultiNationalProductFromChili = string.Empty;

        /// <summary>
        /// Rule: Parse Nossack Greenlea Premier Meats.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
          Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_ParseEAN_UCC128_Nossack_GreenleaPremierMeats = false;

        /// <summary>
        /// Rule Description: Parse Nossack Greenlea Premier Meats.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
          Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_ParseEAN_UCC128_Nossack_GreenleaPremierMeats = string.Empty;

        /// <summary>
        /// Rule: Enable Parse UCC_13 as UPC_12_UPC_E.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_Parse_UCC_13_as_UPC_12_UPC_E = false;

        /// <summary>
        /// Rule Description: Enable Parse UCC_13 as UPC_12 UPC_E.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_Parse_UCC_13_as_UPC_12_UPC_E_Enabled = string.Empty;

        /// <summary>
        /// Rule: Enable Parse Hela Spice Canada Code 128.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_Parse_HelaSpiceCanada_Code_128 = false;

        /// <summary>
        /// Rule Description: Enable Parse Hela Spice Canada Code 128.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_Parse_HelaSpiceCanada_Code_128 = string.Empty;

        /// <summary>
        /// Rule: Enable Nebraska Beef parse Lot Number as Serial Number.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_NebraskaBeef_Parse_Lot_as_Serial_Code_128 = false;

        /// <summary>
        /// Rule Description: Enable Nebraska Beef parse Lot Number as Serial Number.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_NebraskaBeef_Parse_Lot_as_Serial_Code_128 = string.Empty;

        /// <summary>
        /// Rule: Enable Parse Northern Coop Australia Beef Trim Product '25612'.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_ParseNorthernCoop_AustraliaBeefTrim_Product25612 = false;

        /// <summary>
        /// Rule Description: Enable Parse Northern Coop Australia Beef Trim Product '25612'.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_ParseNorthernCoop_AustraliaBeefTrim_Product25612 = string.Empty;

        /// <summary>
        /// Rule: Enable Parse Sumwa Farm Fed Whole Duck use Weight Range To ID Product.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct = false;

        /// <summary>
        /// Rule Description: Enable Parse Sumwa Farm Fed Whole Duck use Weight Range To ID Product.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct = string.Empty;

        /// <summary>
        /// Rule: Enable Parse EAN/UCC Code 128 Nossack Product 94600 Vendor Chilean Back Ribs fix AI(01).
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs = false;

        /// <summary>
        /// Rule Description: Enable Parse EAN/UCC Code 128 Nossack Product 94600 Vendor Chilean Back Ribs fix AI(01).
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs = string.Empty;

        /// <summary>
        /// Rule: Enable Parse EAN/UCC Code 128 Dovre Kii Natural Crisps fix AI(02) as AI(01) and AI(15) DDMMYY to YYMMDD.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_ParseEAN_UCC128_Dovre_KiiNaturalsCrisps = false;

        /// <summary>
        /// Rule Description: Enable Parse EAN/UCC Code 128 Dovre Kii Natural Crisps fix AI(02) as AI(01) and AI(15) DDMMYY to YYMMDD.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_ParseEAN_UCC128_Dovre_KiiNaturalsCrisps = string.Empty;

        /// <summary>
        /// Rule: Enable Parse EAN/UCC Code 128 Nossack Product 51134 Frozen Pork Collar remove AI(00).
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00 = false;

        /// <summary>
        /// Rule Description: Enable Parse EAN/UCC Code 128 Nossack Product 51134 Frozen Pork Collar remove AI(00).
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00 = string.Empty;

        /// <summary>
        /// Rule: When enabled it will suppress error messages for non-tracked fields.
        /// This allows the barcode to be fully parsed, even when a field or two is missing as long
        /// as they are non-tracked.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private bool isRuleEnabled_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents = false;

        /// <summary>
        /// Rule Description: When enabled it will suppress error messages for non-tracked fields.
        /// This allows the barcode to be fully parsed, even when a field or two is missing as long
        /// as they are non-tracked.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        private string ruleDescription_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents = string.Empty;

        /// <summary>
        /// Removes invalid character 0x001A from beginning of Code128 barcodes.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        bool isRuleEnabled_Parse_Code128_CleanSubstituteCharacter;
        /// <summary>
        /// Removes invalid character 0x001A from beginning of Code128 barcodes.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Bar code rules are hard to read without under scores")]
        string ruleDescription_Parse_Code128_CleanSubstituteCharacter;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BarCodeSettings"/> class.
        /// </summary>
        public SysBarCodeSettings()
        {
            this.FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            this.FileASCIIZ = Path.Combine(this.FilePath, this.FileName);
        }

        #endregion

        #region Delegates

        private delegate void OnBarCodeSettingsStatusOutEventHandler(
            IBarCodeSettingsEventPublisher BarCodeSettingsEventPublisher,
            BarCodeSettingsEventArgs BarCodeSettingsEventArgs);

        private delegate void OnBarCodeSettingsProgressOutEventHandler(
            IBarCodeSettingsEventPublisher BarCodeSettingsEventPublisher,
            BarCodeSettingsEventArgs BarCodeSettingsEventArgs);

        #endregion

        #region Events

        private event OnBarCodeSettingsStatusOutEventHandler OnBarCodeSettingsStatusOut;

        private event OnBarCodeSettingsProgressOutEventHandler OnBarCodeSettingsProgressOut;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the path name of the xml file.
        /// </summary>
        public string FilePath
        {
            get
            {
                return this.filePath;
            }

            set
            {
                this.filePath = value;
            }
        }

        /// <summary>
        /// Gets or sets the full path and file name of the xml file.
        /// </summary>
        /// <remarks>
        /// Includes the drive letter, path and file name.
        /// </remarks>
        public string FileASCIIZ
        {
            get
            {
                return this.fileASCIIZ;
            }

            set
            {
                this.fileASCIIZ = value;
            }
        }

        /// <summary>
        /// Gets or sets the file name of the xml file. 
        /// </summary>
        /// <remarks>
        /// Does not include any path information.
        /// </remarks>
        public string FileName
        {
            get
            {
                return this.fileName;
            }

            set
            {
                this.fileName = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether class name to serialize.
        /// </summary>
        public string FullClassName
        {
            get
            {
                return this.fullClassName;
            }

            set
            {
                this.fullClassName = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_Code_128_AI01_BC_AI30_exclusive rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_Code_128_AI01_BC_AI30_exclusive
        {
            get
            {
                return this.isRuleEnabled_Parse_Code_128_AI01_BC_AI30_exclusive;
            }

            set
            {
                this.isRuleEnabled_Parse_Code_128_AI01_BC_AI30_exclusive = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_Code_128_AI01_BC_AI30_exclusive rule description.
        /// </summary>
        public string RuleDescription_Parse_Code_128_AI01_BC_AI30_exclusive
        {
            get
            {
                return this.ruleDescription_Parse_Code_128_AI01_BC_AI30_exclusive;
            }

            set
            {
                this.ruleDescription_Parse_Code_128_AI01_BC_AI30_exclusive = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_Code128_China_Seafood rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_Code128_China_Seafood
        {
            get
            {
                return this.isRuleEnabled_Parse_Code128_China_Seafood;
            }

            set
            {
                this.isRuleEnabled_Parse_Code128_China_Seafood = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_Code128_China_Seafood rule description.
        /// </summary>
        public string RuleDescription_Parse_Code128_China_Seafood
        {
            get
            {
                return this.ruleDescription_Parse_Code128_China_Seafood;
            }

            set
            {
                this.ruleDescription_Parse_Code128_China_Seafood = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU
        {
            get
            {
                return this.isRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU;
            }

            set
            {
                this.isRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU rule description.
        /// </summary>
        public string RuleDescription_Parse_UCC_12_UPC_E_Deli_PLU_Enabled
        {
            get
            {
                return this.ruleDescription_Parse_UCC_12_UPC_E_Deli_PLU_Enabled;
            }

            set
            {
                this.ruleDescription_Parse_UCC_12_UPC_E_Deli_PLU_Enabled = value;
            }
        }

        /// <summary>
        /// Gets or sets if we restrict the deli plu amount to 4 digits. (Down from 5).
        /// </summary>
        public bool IsRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount
        {
            get
            {
                return isRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount;
            }
            set
            {
                isRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount = value;
            }
        }

        /// <summary>
        /// Rule description of deli plu amount restricted to 4 digits.
        /// </summary>
        public string RuleDescription_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount_Enabled
        {
            get
            {
                return ruleDescription_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount_Enabled;
            }
            set
            {
                ruleDescription_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount_Enabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ProductBarCodeMinimumLengthRequirement4Digits rule.
        /// </summary>
        public bool IsRuleEnabled_ProductBarCodeMinimumLengthRequirement4Digits
        {
            get
            {
                return this.isRuleEnabled_ProductBarCodeMinimumLengthRequirement4Digits;
            }

            set
            {
                this.isRuleEnabled_ProductBarCodeMinimumLengthRequirement4Digits = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ProductBarCodeMinimumLengthRequirement4Digits rule 
        /// description.
        /// </summary>
        public string RuleDescription_ProductBarCodeMinimumLengthRequirement4Digits
        {
            get
            {
                return this.ruleDescription_ProductBarCodeMinimumLengthRequirement4Digits;
            }

            set
            {
                this.ruleDescription_ProductBarCodeMinimumLengthRequirement4Digits = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount rule.
        /// </summary>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Nossack is correct")]
        public bool IsRuleEnabled_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount
        {
            get
            {
                return this.ruleEnabled_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount;
            }

            set
            {
                this.ruleEnabled_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount 
        /// rule description.
        /// </summary>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Nossack is correct")]
        public string RuleDescription_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount
        {
            get
            {
                return this.ruleDescription_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount;
            }

            set
            {
                this.ruleDescription_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate
        /// rule.
        /// </summary>
        public bool IsRuleEnabled_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate
        {
            get
            {
                return this.ruleEnabled_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate;
            }

            set
            {
                this.ruleEnabled_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate 
        /// rule description.
        /// </summary>
        public string RuleDescription_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate
        {
            get
            {
                return this.ruleDescription_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate;
            }

            set
            {
                this.ruleDescription_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 
        /// IsRuleEnabled_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate rule.
        /// </summary>
        public bool IsRuleEnabled_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate
        {
            get
            {
                return this.isRuleEnabled_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate;
            }

            set
            {
                this.isRuleEnabled_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 
        /// IsRuleEnabled_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate rule description.
        /// </summary>
        public string RuleDescription_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate
        {
            get
            {
                return this.ruleDescription_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate;
            }

            set
            {
                this.ruleDescription_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseWOOLWICH_Code_128_RemoveSpaces rule.
        /// </summary>
        public bool IsRuleEnabled_ParseWOOLWICH_Code_128_RemoveSpaces
        {
            get
            {
                return this.isRuleEnabled_ParseWOOLWICH_Code_128_RemoveSpaces;
            }

            set
            {
                this.isRuleEnabled_ParseWOOLWICH_Code_128_RemoveSpaces = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseWOOLWICH_Code_128_RemoveSpaces rule description.
        /// </summary>
        public string RuleDescription_ParseWOOLWICH_Code_128_RemoveSpaces
        {
            get
            {
                return this.ruleDescription_ParseWOOLWICH_Code_128_RemoveSpaces;
            }

            set
            {
                this.ruleDescription_ParseWOOLWICH_Code_128_RemoveSpaces = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseCode_128AsUCC_EAN_128_SymbolCompliant rule.
        /// </summary>
        public bool IsRuleEnabled_ParseCode_128AsUCC_EAN_128_SymbolCompliant
        {
            get
            {
                return this.isRuleEnabled_ParseCode_128AsUCC_EAN_128_SymbolCompliant;
            }

            set
            {
                this.isRuleEnabled_ParseCode_128AsUCC_EAN_128_SymbolCompliant = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseCode_128AsUCC_EAN_128_SymbolCompliant rule 
        /// description.
        /// </summary>
        public string RuleDescription_ParseCode_128AsUCC_EAN_128_SymbolCompliant
        {
            get
            {
                return this.ruleDescription_ParseCode_128AsUCC_EAN_128_SymbolCompliant;
            }

            set
            {
                this.ruleDescription_ParseCode_128AsUCC_EAN_128_SymbolCompliant = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseFINICA_Code_128_as_EAN_UCC_128 rule.
        /// </summary>
        public bool IsRuleEnabled_ParseFINICA_Code_128_as_EAN_UCC_128
        {
            get
            {
                return this.isRuleEnabled_ParseFINICA_Code_128_as_EAN_UCC_128;
            }

            set
            {
                this.isRuleEnabled_ParseFINICA_Code_128_as_EAN_UCC_128 = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseFINICA_Code_128_as_EAN_UCC_128 description rule.
        /// </summary>
        public string RuleDescription_ParseFINICA_Code_128_as_EAN_UCC_128
        {
            get
            {
                return this.ruleDescription_ParseFINICA_Code_128_as_EAN_UCC_128;
            }

            set
            {
                this.ruleDescription_ParseFINICA_Code_128_as_EAN_UCC_128 = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_NoCheckDigitInGlobalLocationNumber rule.
        /// </summary>
        public bool IsRuleEnabled_NoCheckDigitInGlobalLocationNumber
        {
            get
            {
                return this.isRuleEnabled_NoCheckDigitInGlobalLocationNumber;
            }

            set
            {
                this.isRuleEnabled_NoCheckDigitInGlobalLocationNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_NoCheckDigitInGlobalLocationNumber rule description.
        /// </summary>
        public string RuleDescription_NoCheckDigitInGlobalLocationNumber
        {
            get
            {
                return this.ruleDescription_NoCheckDigitInGlobalLocationNumber;
            }

            set
            {
                this.ruleDescription_NoCheckDigitInGlobalLocationNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family rule.
        /// </summary>
        public bool IsRuleEnabled_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family
        {
            get
            {
                return this.isRuleEnabled_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family;
            }

            set
            {
                this.isRuleEnabled_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family rule 
        /// description.
        /// </summary>
        public string RuleDescription_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family
        {
            get
            {
                return this.ruleDescription_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family;
            }

            set
            {
                this.ruleDescription_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_OvationFielding_Lamb rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_Code128_OvationFielding_Lamb
        {
            get
            {
                return this.isRuleEnabled_Parse_Code128_OvationFielding_Lamb;
            }

            set
            {
                this.isRuleEnabled_Parse_Code128_OvationFielding_Lamb = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_OvationFielding_Lamb rule description.
        /// </summary>
        public string RuleDescription_Parse_Code128_OvationFielding_Lamb
        {
            get
            {
                return this.ruleDescription_Parse_Code128_OvationFielding_Lamb;
            }

            set
            {
                this.ruleDescription_Parse_Code128_OvationFielding_Lamb = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Renees_Code_128_RemoveSpaces rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_Renees_Code_128_RemoveSpaces
        {
            get
            {
                return this.isRuleEnabled_Parse_Renees_Code_128_RemoveSpaces;
            }

            set
            {
                this.isRuleEnabled_Parse_Renees_Code_128_RemoveSpaces = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Renees_Code_128_RemoveSpaces rule description.
        /// </summary>
        public string RuleDescription_Parse_Renees_Code_128_RemoveSpaces
        {
            get
            {
                return this.ruleDescription_Parse_Renees_Code_128_RemoveSpaces;
            }

            set
            {
                this.ruleDescription_Parse_Renees_Code_128_RemoveSpaces = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_LanzMeats_Code_128_AI10_as_AI21 rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_LanzMeats_Code_128_AI10_as_AI21
        {
            get
            {
                return this.isRuleEnabled_ParseLanzMeats_Code_128_AI10_as_AI21;
            }

            set
            {
                this.isRuleEnabled_ParseLanzMeats_Code_128_AI10_as_AI21 = value;
            }
        }

        /// <summary>
        /// Gets or sets a value IsRuleEnabled_Parse_LanzMeats_Code_128_AI10_as_AI21 rule description.
        /// </summary>
        public string RuleDescription_Parse_LanzMeats_Code_128_AI10_as_AI21
        {
            get
            {
                return this.ruleDescription_ParseLanzMeats_Code_128_AI10_as_AI21;
            }

            set
            {
                this.ruleDescription_ParseLanzMeats_Code_128_AI10_as_AI21 = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 
        /// IsRuleEnabled_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit rule.
        /// </summary>
        public bool IsRuleEnabled_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit
        {
            get
            {
                return this.isRuleEnabled_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit;
            }

            set
            {
                this.isRuleEnabled_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit = value;
            }
        }

        /// <summary>
        /// Gets or sets a value HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit rule description.
        /// </summary>
        public string RuleDescription_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit
        {
            get
            {
                return this.ruleDescription_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit;
            }

            set
            {
                this.ruleDescription_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 
        /// IsRuleEnabled_ParseNossackBrokerMultiNationalProductFromChili rule.
        /// </summary>
        public bool IsRuleEnabled_ParseNossackBrokerMultiNationalProductFromChili
        {
            get
            {
                return this.isRuleEnabled_ParseNossackBrokerMultiNationalProductFromChili;
            }

            set
            {
                this.isRuleEnabled_ParseNossackBrokerMultiNationalProductFromChili = value;
            }
        }

        /// <summary>
        /// Gets or sets a valueParseNossackBrokerMultiNationalProductFromChili rule description.
        /// </summary>
        public string RuleDescription_ParseNossackBrokerMultiNationalProductFromChili
        {
            get
            {
                return this.ruleDescription_ParseNossackBrokerMultiNationalProductFromChili;
            }

            set
            {
                this.ruleDescription_ParseNossackBrokerMultiNationalProductFromChili = value;
            }
        }

        /// <summary>
        /// Gets or sets if this will read the Nossack Greenlea Premier Meats barcodes.
        /// </summary>
        public bool IsRuleEnabled_ParseEAN_UCC128_Nossack_GreenleaPremierMeats
        {
            get
            {
                return this.isRuleEnabled_ParseEAN_UCC128_Nossack_GreenleaPremierMeats;
            }
            set
            {
                this.isRuleEnabled_ParseEAN_UCC128_Nossack_GreenleaPremierMeats = value;
            }
        }

        /// <summary>
        /// Gets or sets a rule description for Nossack Greenlea Premier Meats barcode rule.
        /// </summary>
        public string RuleDescription_ParseEAN_UCC128_Nossack_GreenleaPremierMeats
        {
            get
            {
                return this.ruleDescription_ParseEAN_UCC128_Nossack_GreenleaPremierMeats;
            }
            set
            {
                this.ruleDescription_ParseEAN_UCC128_Nossack_GreenleaPremierMeats = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_UCC_13_as_UPC_12_UPC_E rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_UCC_13_as_UPC_12_UPC_E
        {
            get
            {
                return this.isRuleEnabled_Parse_UCC_13_as_UPC_12_UPC_E;
            }

            set
            {
                this.isRuleEnabled_Parse_UCC_13_as_UPC_12_UPC_E = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_UCC_13_as_UPC_12_UPC_E rule description.
        /// </summary>
        public string RuleDescription_Parse_UCC_13_as_UPC_12_UPC_E_Enabled
        {
            get
            {
                return this.ruleDescription_Parse_UCC_13_as_UPC_12_UPC_E_Enabled;
            }

            set
            {
                this.ruleDescription_Parse_UCC_13_as_UPC_12_UPC_E_Enabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_HelaSpiceCanada_Code_128 rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_HelaSpiceCanada_Code_128
        {
            get
            {
                return this.isRuleEnabled_Parse_HelaSpiceCanada_Code_128;
            }

            set
            {
                this.isRuleEnabled_Parse_HelaSpiceCanada_Code_128 = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_HelaSpiceCanada_Code_128 rule description.
        /// </summary>
        public string RuleDescription_Parse_HelaSpiceCanada_Code_128
        {
            get
            {
                return this.ruleDescription_Parse_HelaSpiceCanada_Code_128;
            }

            set
            {
                this.ruleDescription_Parse_HelaSpiceCanada_Code_128 = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_HelaSpiceCanada_Code_128 rule.
        /// </summary>
        public bool IsRuleEnabled_NebraskaBeef_Parse_Lot_as_Serial_Code_128
        {
            get
            {
                return this.isRuleEnabled_NebraskaBeef_Parse_Lot_as_Serial_Code_128;
            }

            set
            {
                this.isRuleEnabled_NebraskaBeef_Parse_Lot_as_Serial_Code_128 = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_HelaSpiceCanada_Code_128 rule description.
        /// </summary>
        public string RuleDescription_NebraskaBeef_Parse_Lot_as_Serial_Code_128
        {
            get
            {
                return this.ruleDescription_NebraskaBeef_Parse_Lot_as_Serial_Code_128;
            }

            set
            {
                this.ruleDescription_NebraskaBeef_Parse_Lot_as_Serial_Code_128 = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseNorthernCoop_AustraliaBeefTrim_Product25612 rule.
        /// </summary>
        public bool IsRuleEnabled_ParseNorthernCoop_AustraliaBeefTrim_Product25612
        {
            get
            {
                return this.isRuleEnabled_ParseNorthernCoop_AustraliaBeefTrim_Product25612;
            }

            set
            {
                this.isRuleEnabled_ParseNorthernCoop_AustraliaBeefTrim_Product25612 = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ParseNorthernCoop_AustraliaBeefTrim_Product25612 description.
        /// </summary>
        public string RuleDescription_ParseNorthernCoop_AustraliaBeefTrim_Product25612
        {
            get
            {
                return this.ruleDescription_ParseNorthernCoop_AustraliaBeefTrim_Product25612;
            }

            set
            {
                this.ruleDescription_ParseNorthernCoop_AustraliaBeefTrim_Product25612 = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct rule.
        /// </summary>
        public bool IsRuleEnabled_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct
        {
            get
            {
                return this.isRuleEnabled_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct;
            }

            set
            {
                this.isRuleEnabled_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct description.
        /// </summary>
        public string RuleDescription_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct
        {
            get
            {
                return this.ruleDescription_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct;
            }

            set
            {
                this.ruleDescription_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs rule.
        /// </summary>
        public bool IsRuleEnabled_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs
        {
            get
            {
                return this.isRuleEnabled_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs;
            }

            set
            {
                this.isRuleEnabled_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs description.
        /// </summary>
        public string RuleDescription_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs
        {
            get
            {
                return this.ruleDescription_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs;
            }

            set
            {
                this.ruleDescription_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs = value;
            }
        }

        /// <summary>
        /// Enables AI(02) to be read as AI(01) and AI(15) fix of DDMMYY -> YYMMDD.
        /// </summary>
        public bool IsRuleEnabled_ParseEAN_UCC128_Dovre_KiiNaturalsCrisps
        {
            get
            {
                return isRuleEnabled_ParseEAN_UCC128_Dovre_KiiNaturalsCrisps;
            }
            set
            {
                isRuleEnabled_ParseEAN_UCC128_Dovre_KiiNaturalsCrisps = value;
            }
        }

        /// <summary>
        /// Enables AI(02) to be read as AI(01) and AI(15) fix of DDMMYY -> YYMMDD.
        /// </summary>
        public string RuleDescription_ParseEAN_UCC128_Dovre_KiiNaturalsCrisps
        {
            get
            {
                return ruleDescription_ParseEAN_UCC128_Dovre_KiiNaturalsCrisps;
            }
            set
            {
                ruleDescription_ParseEAN_UCC128_Dovre_KiiNaturalsCrisps = value;
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00 rule.
        /// </summary>
        public bool IsRuleEnabled_ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00
        {
            get
            {
                return this.isRuleEnabled_ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00;
            }

            set
            {
                this.isRuleEnabled_ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00 = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00 
        /// description.
        /// </summary>
        public string RuleDescription_ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00
        {
            get
            {
                return this.ruleDescription_ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00;
            }

            set
            {
                this.ruleDescription_ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00 = value;
            }
        }

        /// <summary>
        /// When enabled it will suppress error messages for non-tracked fields.
        /// This allows the barcode to be fully parsed, even when a field or two is missing as long
        /// as they are non-tracked.
        /// </summary>
        public bool IsRuleEnabled_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents
        {
            get
            {
                return this.isRuleEnabled_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents;
            }
            set
            {
                this.isRuleEnabled_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents = value;
            }
        }

        /// <summary>
        /// When enabled it will suppress error messages for non-tracked fields.
        /// This allows the barcode to be fully parsed, even when a field or two is missing as long
        /// as they are non-tracked.
        /// </summary>
        public string RuleDescription_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents
        {
            get
            {
                return this.ruleDescription_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents;
            }
            set
            {
                this.ruleDescription_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents = value;
            }
        }


        /// <summary>
        /// Strips invalid character 0x001A from the beginning of the string.
        /// </summary>
        public bool IsRuleEnabled_Parse_Code128_CleanSubstituteCharacter
        {
            get
            {
                return this.isRuleEnabled_Parse_Code128_CleanSubstituteCharacter;
            }
            set
            {
                this.isRuleEnabled_Parse_Code128_CleanSubstituteCharacter = value;
            }
        }
        /// <summary>
        /// Strips invalid character 0x001A from the beginning of the string.
        /// </summary>
        public string RuleDescription_Parse_Code128_CleanSubstituteCharacter
        {
            get
            {
                return ruleDescription_Parse_Code128_CleanSubstituteCharacter;
            }
            set
            {
                ruleDescription_Parse_Code128_CleanSubstituteCharacter = value;
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Request the bar code settings data and save the file.
        /// </summary>
        public void RequestBarCodeSettings()
        {
            BarCodeSettingsEventArgs msgEventArg = new BarCodeSettingsEventArgs();

            try
            {
                string reqData = string.Empty;

                if (this.OnBarCodeSettingsStatusOut != null)
                {
                    // TODO: Resource strings
                    msgEventArg.StatusOutMsg = "Seeding bar code settings.";
                    this.OnBarCodeSettingsStatusOut(this, msgEventArg);
                }

                reqData = SysCommon.SubmitComm(this.CreateXMLRequestBarCodeSettings());
                if (reqData.Length > 0)
                {
                    // check if result is an error message
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument docError = new XmlDocument();
                        docError.LoadXml(reqData);
                        throw new Exception(docError.DocumentElement["detail"].InnerText);
                    }

                    this.Save(reqData);
                }

                if (this.OnBarCodeSettingsStatusOut != null)
                {
                    // TODO: Resource strings
                    msgEventArg.StatusOutMsg = "Seeding bar code settings complete.";
                    this.OnBarCodeSettingsStatusOut(this, msgEventArg);
                }
            }
            catch (Exception ex)
            {
                if (this.OnBarCodeSettingsStatusOut != null)
                {
                    // TODO: Resource strings
                    msgEventArg.StatusOutMsg = "Seeding bar code settings failed.";
                    this.OnBarCodeSettingsStatusOut(this, msgEventArg);
                }

                // TODO: Resource strings
                throw new Exception("Seeding bar code settings failed.", ex);
            }
        }

        /// <summary>
        /// Load and object of type BarCodeSettings from a serialized xml file.
        /// </summary>
        /// <returns>Object of type BarCodeSettings</returns>
        public SysBarCodeSettings LoadData()
        {
            lock (readLockObject)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SysBarCodeSettings));
                SysBarCodeSettings barCodeSettings = new SysBarCodeSettings();

                // use relection to get the current assembly path for the database
                string appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string fullPathToMyFile = Path.Combine(appDir, this.FileName);

                if (!File.Exists(fullPathToMyFile))
                {
                    this.SaveData(ref barCodeSettings);
                }

                // Create a FileStream to read with.
                using (FileStream reader = new FileStream(fullPathToMyFile, FileMode.Open))
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
        /// <returns>True if save was successful else false</returns>
        public bool SaveData(ref SysBarCodeSettings barCodeSettings)
        {
            lock (writeLockObject)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SysBarCodeSettings));
                bool isSaveComplete = false;

                try
                {
                    // use relection to get the current assembly path for the database
                    string appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    string fullPathToMyFile = Path.Combine(appDir, this.FileName);

                    // Create a FileStream to write with.
                    using (FileStream writer = new FileStream(fullPathToMyFile, FileMode.Create))
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

        #region IBarCodeSettingsEventPublisher Members

        public void RegisterBarCodeSettingsListenerStatusOut(IBarCodeSettingsEventListener barCodeSettingsEventListener)
        {
            this.OnBarCodeSettingsStatusOut += new OnBarCodeSettingsStatusOutEventHandler(
                barCodeSettingsEventListener.OnBarCodeSettingsStatusOut);
        }

        public void UnregisterBarCodeSettingsListenerStatusOut(IBarCodeSettingsEventListener barCodeSettingsEventListener)
        {
            this.OnBarCodeSettingsStatusOut -= new OnBarCodeSettingsStatusOutEventHandler(
                barCodeSettingsEventListener.OnBarCodeSettingsStatusOut);
        }

        public void RegisterBarCodeSettingsListenerProgressOut(IBarCodeSettingsEventListener barCodeSettingsEventListener)
        {
            this.OnBarCodeSettingsProgressOut += new OnBarCodeSettingsProgressOutEventHandler(
                barCodeSettingsEventListener.OnBarCodeSettingsProgressOut);
        }

        public void UnregisterBarCodeSettingsListenerProgressOut(IBarCodeSettingsEventListener barCodeSettingsEventListener)
        {
            this.OnBarCodeSettingsProgressOut -= new OnBarCodeSettingsProgressOutEventHandler(
                barCodeSettingsEventListener.OnBarCodeSettingsProgressOut);
        }

        #endregion

        #endregion

        #region Private Members

        /// <summary>
        /// Process the xml data from the communication server and store it.
        /// </summary>
        /// <param name="xmlData"> Xml data received from the communication server</param>
        private void Save(string xmlData)
        {
            BarCodeSettingsEventArgs msgEventArg = new BarCodeSettingsEventArgs();

            try
            {
                using (DataSet ds = new DataSet())
                {
                    if (this.OnBarCodeSettingsStatusOut != null)
                    {
                        // TODO: Resource strings
                        msgEventArg.StatusOutMsg = "Reading received data.";
                        this.OnBarCodeSettingsStatusOut(this, msgEventArg);
                    }

                    ds.ReadXml(SysCommon.GetXmlReader(ref xmlData), XmlReadMode.Auto);

                    if (this.OnBarCodeSettingsStatusOut != null)
                    {
                        // TODO: Resource strings
                        msgEventArg.StatusOutMsg = "Saving bar code settings.";
                        this.OnBarCodeSettingsStatusOut(this, msgEventArg);
                    }

                    if (ds.Tables.Count != 0)
                    {
                        int progMax = ds.Tables["tBarCodeSettings"].Rows.Count;

                        if (this.OnBarCodeSettingsStatusOut != null)
                        {
                            // TODO: Resource strings
                            msgEventArg.StatusOutMsg = string.Format("Records {0}.", progMax);
                            this.OnBarCodeSettingsStatusOut(this, msgEventArg);
                        }

                        int progValue = 0;
                        if (this.OnBarCodeSettingsProgressOut != null)
                        {
                            msgEventArg.CurrentValue = 0;
                            msgEventArg.MinValue = 0;
                            msgEventArg.MaxValue = progMax;
                            this.OnBarCodeSettingsProgressOut(this, msgEventArg);
                        }

                        // ParseCode_128AsUCC_EAN_128_SymbolCompliant = 1,
                        // ParseFINICA_Code_128_as_EAN_UCC_128 = 2,
                        // NoCheckDigitInGlobalLocationNumber = 3,
                        // NoZeroPaddingOfProductIdentifierForUCC_EAN_Family = 4,
                        // ParseWOOLWICH_Code_128_RemoveSpaces = 5,
                        // Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate = 6,
                        // Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate = 7,
                        // Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount = 8,
                        // ProductBarCodeMinimumLength4Digits = 9,
                        // Parse_UCC_12_UPC_E_as_Deli_PLU = 10,
                        // Parse_Code128_OvationFielding_Lamb = 11,
                        // Parse_Code128_China_Seafood = 12,
                        // Parse_Code_128_AI01_BC_AI30_exclusive = 13,
                        // ParseRenees_Code_128_RemoveSpaces = 14
                        // ParseLanzMeats_Code_128_AI10_as_AI21 = 15
                        // HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit = 16
                        // ParseNossackBrokerMultiNationalProductFromChili = 17
                        // ParseUCC_13asUPC_12_UPC_E = 18
                        // Parse_HelaSpiceCanada_Code_128 = 19
                        // NebraskaBeef_Parse_Lot_as_Serial_Code_128 = 20
                        // ParseNorthernCoop_AustraliaBeefTrim_Product25612 = 21
                        // ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct = 22
                        // ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs = 23
                        // ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00 = 24

                        foreach (DataRow drow in ds.Tables["tBarCodeSettings"].Rows)
                        {
                            SysCommon.BarCodeRules rule = (SysCommon.BarCodeRules)Convert.ToInt32(drow["BarCodeSettingsRuleFKRefno"]);
                            bool enabled = Convert.ToBoolean(drow["BarCodeSettingsRuleEnabled"]);
                            string description = Convert.ToString(drow["BarCodeSettingsRuleDescription"]);
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
                            }

                            progValue = progValue + 1;
                            if (this.OnBarCodeSettingsProgressOut != null)
                            {
                                msgEventArg.CurrentValue = progValue;
                                msgEventArg.MinValue = 0;
                                msgEventArg.MaxValue = progMax;
                                this.OnBarCodeSettingsProgressOut(this, msgEventArg);
                            }
                        } // foreach barcode rule

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
                        // TODO: Resource strings
                        msgEventArg.StatusOutMsg = "Saving bar code settings complete.";
                        this.OnBarCodeSettingsStatusOut(this, msgEventArg);
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.OnBarCodeSettingsStatusOut != null)
                {
                    // TODO: Resource strings
                    msgEventArg.StatusOutMsg = "Saving bar code settings failed.";
                    this.OnBarCodeSettingsStatusOut(this, msgEventArg);
                }

                // TODO: Resource strings
                throw new Exception("Saving bar code settings failed.", ex);
            }
        }

        /// <summary>
        /// Create the xml request string to send to the communication server.
        /// </summary>
        /// <returns>String of the xml request to get the bar code settings</returns>
        private string CreateXMLRequestBarCodeSettings()
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            string revision = new DateTime(1900, 1, 1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            SysEnvironment sysEnv = SysEnvironment.GetInstance();

            XDocument request = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                string.Empty,
                new XElement(
                    "imclient",
                    new XElement(
                        "command",
                        new XElement("action", "REQUEST BAR CODE SETTINGS"),
                        new XElement("device_id", sysEnv.DeviceID),
                        new XElement("login_id", sysEnv.ContactLog.LoginID.ToString()),
                        new XElement("revision_datetime", revision))));

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        #endregion
    }
}
