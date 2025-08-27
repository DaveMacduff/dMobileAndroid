// <copyright file="BarCodeSettings.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock.Settings
{
    using System;
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using dMobile.Bedrock;

    /// <summary>
    /// Bar Code Settings Class.
    /// </summary>
    [Serializable]
    public class BarCodeSettings : IBarCodeSettingsEventPublisher
    {
        [NonSerialized]
        private readonly object writeLockObject = new object();

        [NonSerialized]
        private readonly object readLockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="BarCodeSettings"/> class.
        /// </summary>
        public BarCodeSettings()
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
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the full path and file name of the xml file.
        /// </summary>
        /// <remarks>
        /// Includes the drive letter, path and file name.
        /// </remarks>
        public string FileASCIIZ { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file name of the xml file.
        /// </summary>
        /// <remarks>
        /// Does not include any path information.
        /// </remarks>
        public string FileName { get; set; } = "dMobileBarCodeSettings.xml";

        /// <summary>
        /// Gets or sets a value indicating whether class name to serialize.
        /// </summary>
        public string FullClassName { get; set; } = "dMobile.BarCodeSettings";

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_Code_128_AI01_BC_AI30_exclusive rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_Code_128_AI01_BC_AI30_exclusive { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_Code_128_AI01_BC_AI30_exclusive rule description.
        /// </summary>
        public string RuleDescription_Parse_Code_128_AI01_BC_AI30_exclusive { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_Code128_China_Seafood rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_Code128_China_Seafood { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_Code128_China_Seafood rule description.
        /// </summary>
        public string RuleDescription_Parse_Code128_China_Seafood { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU rule description.
        /// </summary>
        public string RuleDescription_Parse_UCC_12_UPC_E_Deli_PLU_Enabled { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether if we restrict the deli plu amount to 4 digits. (Down from 5).
        /// </summary>
        public bool IsRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount { get; set; } = false;

        /// <summary>
        /// Gets or sets the rule description of deli PLU amount restricted to 4 digits.
        /// </summary>
        public string RuleDescription_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount_Enabled { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ProductBarCodeMinimumLengthRequirement4Digits rule.
        /// </summary>
        public bool IsRuleEnabled_ProductBarCodeMinimumLengthRequirement4Digits { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ProductBarCodeMinimumLengthRequirement4Digits rule
        /// description.
        /// </summary>
        public string RuleDescription_ProductBarCodeMinimumLengthRequirement4Digits { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount
        /// rule description.
        /// </summary>
        public string RuleDescription_Parse_Code_128_AI01_Nossack_AI21_SkidNumber_BoxCount { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate
        /// rule.
        /// </summary>
        public bool IsRuleEnabled_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate
        /// rule description.
        /// </summary>
        public string RuleDescription_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether
        /// IsRuleEnabled_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate rule.
        /// </summary>
        public bool IsRuleEnabled_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether
        /// IsRuleEnabled_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate rule description.
        /// </summary>
        public string RuleDescription_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseWOOLWICH_Code_128_RemoveSpaces rule.
        /// </summary>
        public bool IsRuleEnabled_ParseWOOLWICH_Code_128_RemoveSpaces { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseWOOLWICH_Code_128_RemoveSpaces rule description.
        /// </summary>
        public string RuleDescription_ParseWOOLWICH_Code_128_RemoveSpaces { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseCode_128AsUCC_EAN_128_SymbolCompliant rule.
        /// </summary>
        public bool IsRuleEnabled_ParseCode_128AsUCC_EAN_128_SymbolCompliant { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseCode_128AsUCC_EAN_128_SymbolCompliant rule
        /// description.
        /// </summary>
        public string RuleDescription_ParseCode_128AsUCC_EAN_128_SymbolCompliant { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseFINICA_Code_128_as_EAN_UCC_128 rule.
        /// </summary>
        public bool IsRuleEnabled_ParseFINICA_Code_128_as_EAN_UCC_128 { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseFINICA_Code_128_as_EAN_UCC_128 description rule.
        /// </summary>
        public string RuleDescription_ParseFINICA_Code_128_as_EAN_UCC_128 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_NoCheckDigitInGlobalLocationNumber rule.
        /// </summary>
        public bool IsRuleEnabled_NoCheckDigitInGlobalLocationNumber { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_NoCheckDigitInGlobalLocationNumber rule description.
        /// </summary>
        public string RuleDescription_NoCheckDigitInGlobalLocationNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family rule.
        /// </summary>
        public bool IsRuleEnabled_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family rule
        /// description.
        /// </summary>
        public string RuleDescription_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_OvationFielding_Lamb rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_Code128_OvationFielding_Lamb { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_OvationFielding_Lamb rule description.
        /// </summary>
        public string RuleDescription_Parse_Code128_OvationFielding_Lamb { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Renees_Code_128_RemoveSpaces rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_Renees_Code_128_RemoveSpaces { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Renees_Code_128_RemoveSpaces rule description.
        /// </summary>
        public string RuleDescription_Parse_Renees_Code_128_RemoveSpaces { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_LanzMeats_Code_128_AI10_as_AI21 rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_LanzMeats_Code_128_AI10_as_AI21 { get; set; } = false;

        /// <summary>
        /// Gets or sets a value IsRuleEnabled_Parse_LanzMeats_Code_128_AI10_as_AI21 rule description.
        /// </summary>
        public string RuleDescription_Parse_LanzMeats_Code_128_AI10_as_AI21 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether
        /// IsRuleEnabled_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit rule.
        /// </summary>
        public bool IsRuleEnabled_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit { get; set; } = false;

        /// <summary>
        /// Gets or sets a value HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit rule description.
        /// </summary>
        public string RuleDescription_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether
        /// IsRuleEnabled_ParseNossackBrokerMultiNationalProductFromChili rule.
        /// </summary>
        public bool IsRuleEnabled_ParseNossackBrokerMultiNationalProductFromChili { get; set; } = false;

        /// <summary>
        /// Gets or sets a valueParseNossackBrokerMultiNationalProductFromChili rule description.
        /// </summary>
        public string RuleDescription_ParseNossackBrokerMultiNationalProductFromChili { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets if this will read the Nossack Greenlea Premier Meats barcodes.
        /// </summary>
        public bool IsRuleEnabled_ParseEAN_UCC128_Nossack_GreenleaPremierMeats { get; set; } = false;

        /// <summary>
        /// Gets or sets a rule description for Nossack Greenlea Premier Meats barcode rule.
        /// </summary>
        public string RuleDescription_ParseEAN_UCC128_Nossack_GreenleaPremierMeats { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_UCC_13_as_UPC_12_UPC_E rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_UCC_13_as_UPC_12_UPC_E { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_UCC_13_as_UPC_12_UPC_E rule description.
        /// </summary>
        public string RuleDescription_Parse_UCC_13_as_UPC_12_UPC_E_Enabled { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_HelaSpiceCanada_Code_128 rule.
        /// </summary>
        public bool IsRuleEnabled_Parse_HelaSpiceCanada_Code_128 { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_HelaSpiceCanada_Code_128 rule description.
        /// </summary>
        public string RuleDescription_Parse_HelaSpiceCanada_Code_128 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_HelaSpiceCanada_Code_128 rule.
        /// </summary>
        public bool IsRuleEnabled_NebraskaBeef_Parse_Lot_as_Serial_Code_128 { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_Parse_HelaSpiceCanada_Code_128 rule description.
        /// </summary>
        public string RuleDescription_NebraskaBeef_Parse_Lot_as_Serial_Code_128 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseNorthernCoop_AustraliaBeefTrim_Product25612 rule.
        /// </summary>
        public bool IsRuleEnabled_ParseNorthernCoop_AustraliaBeefTrim_Product25612 { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether ParseNorthernCoop_AustraliaBeefTrim_Product25612 description.
        /// </summary>
        public string RuleDescription_ParseNorthernCoop_AustraliaBeefTrim_Product25612 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct rule.
        /// </summary>
        public bool IsRuleEnabled_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct description.
        /// </summary>
        public string RuleDescription_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsRuleEnabled_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs rule.
        /// </summary>
        public bool IsRuleEnabled_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs { get; set; } = false;

        /// <summary>
        /// Gets or sets the ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs description.
        /// </summary>
        public string RuleDescription_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether enables AI(02) to be read as AI(01) and AI(15) fix of DDMMYY -> YYMMDD.
        /// </summary>
        public bool IsRuleEnabled_ParseEAN_UCC128_Dovre_KiiNaturalsCrisps { get; set; } = false;

        /// <summary>
        /// Gets or sets the Enables AI(02) to be read as AI(01) and AI(15) fix of DDMMYY -> YYMMDD description.
        /// </summary>
        public string RuleDescription_ParseEAN_UCC128_Dovre_KiiNaturalsCrisps { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00 rule.
        /// </summary>
        public bool IsRuleEnabled_ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00 { get; set; } = false;

        /// <summary>
        /// Gets or sets the ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00 description.
        /// description.
        /// </summary>
        public string RuleDescription_ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether when enabled it will suppress error messages for non-tracked fields.
        /// This allows the bar code to be fully parsed, even when a field or two is missing as long
        /// as they are non-tracked.
        /// </summary>
        public bool IsRuleEnabled_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents { get; set; } = false;

        /// <summary>
        /// Gets or sets when enabled it will suppress error messages for non-tracked fields description.
        /// This allows the bar code to be fully parsed, even when a field or two is missing as long
        /// as they are non-tracked.
        /// </summary>
        public string RuleDescription_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether strips invalid character 0x001A from the beginning of the string.
        /// </summary>
        public bool IsRuleEnabled_Parse_Code128_CleanSubstituteCharacter { get; set; } = false;

        /// <summary>
        /// Gets or sets the strips invalid character 0x001A from the beginning of the string.
        /// </summary>
        public string RuleDescription_Parse_Code128_CleanSubstituteCharacter { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether parse EAN / UCC Code 128 Bourvy Exports production date from the
        /// AI(21) serial number.
        /// </summary>
        public bool IsRuleEnabled_ParseEAN_UCC128_BourvyExports_ProductionDate { get; set; } = false;

        /// <summary>
        /// Gets or sets the parse EAN / UCC Code 128 Bourvy Exports production date from the AI(21) serial number string.
        /// </summary>
        public string RuleDescription_ParseEAN_UCC128_BourvyExports_ProductionDate { get; set; } = string.Empty;

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
        /// <returns><see cref="BarCodeSettings"/> Object of type BarCodeSettings.</returns>
        public BarCodeSettings LoadData()
        {
            lock (this.readLockObject)
            {
                var serializer = new XmlSerializer(typeof(BarCodeSettings));
                var barCodeSettings = new BarCodeSettings();

                // use reflection to get the current assembly path for the database
                var appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var settingsFileName = Path.Combine(appDir, this.FileName);

                if (!File.Exists(settingsFileName))
                {
                    this.SaveData(ref barCodeSettings);
                }

                // Create a FileStream to read with.
                using (var reader = new FileStream(settingsFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // Deserialize the stream to the object, and close the TextReader
                    barCodeSettings = (BarCodeSettings)serializer.Deserialize(reader);
                }

                return barCodeSettings;
            }
        }

        /// <summary>
        /// Serialize and save the data into a xml file.
        /// </summary>
        /// <param name="barCodeSettings"> Instance of BarCodeSettings to serialize.</param>
        /// <returns><see cref="bool"/> True if save was successful else false.</returns>
        public bool SaveData(ref BarCodeSettings barCodeSettings)
        {
            lock (this.writeLockObject)
            {
                var serializer = new XmlSerializer(typeof(BarCodeSettings));
                var isSaveComplete = false;

                try
                {
                    // use reflection to get the current assembly path for the database
                    var appDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    var settingsFileName = Path.Combine(appDir, this.FileName);

                    // Create a FileStream to write with.
                    using (var writer = new FileStream(
                        settingsFileName,
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
                                this.IsRuleEnabled_NebraskaBeef_Parse_Lot_as_Serial_Code_128 = enabled;
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
