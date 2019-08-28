namespace dMobile.BarCodeScanner
{
    using System;
    using dMobile.Bedrock;

    public class BarCodeParseUCC_EAN_128_Symbol : BarCodeParse
    {
        #region Public Members

        public override void ParseBarCodeData(string barCodeData)
        {
            try
            {
                string parse = barCodeData;
                string compare = string.Empty;

                // GetType == typeof(...) makes sure the rules are checked once.
                // This way, a rule doesn't have to override ParseBarCodeData(...) with duplicate code.
                // It can instead just call this and run it again. Saves coding time, reduces duplicate code, keeps code in sync.
                if (GetType() == typeof(BarCodeParseUCC_EAN_128_Symbol))
                {
                    if (this.Settings.IsRuleEnabled_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs
                        && barCodeData.Length >= 14
                        && barCodeData.Substring(4, 6) == "896775")
                    {
                        this.ClearResetBarCodeProperties(false);
                        BarCodeParseUCC_EAN_128_Symbol_NossackProduct94600_ChileanBackRibs barCodeParse
                            = new BarCodeParseUCC_EAN_128_Symbol_NossackProduct94600_ChileanBackRibs();
                        barCodeParse.ParseBarCodeData(barCodeData);
                        return;
                    }
                    else if (this.Settings.IsRuleEnabled_ParseEAN_UCC128_Dovre_KiiNaturalsCrisps &&
                        barCodeData.Length >= 14)
                    {
                        this.ClearResetBarCodeProperties(false);
                        BarCodeParseUCC_EAN_128_KiiNaturalsCrisps_Dovre barCodeParse = new BarCodeParseUCC_EAN_128_KiiNaturalsCrisps_Dovre();
                        barCodeParse.ParseBarCodeData(barCodeData);
                        return; // stop executing since we already went through this function as long as you don't override ParseBarCodeData
                    }
                    else if (this.Settings.IsRuleEnabled_ParseEAN_UCC128_Nossack_GreenleaPremierMeats &&
                        barCodeData.Length == 20 &&
                        barCodeData.Substring(0, 2) != "00" && // don't parse out skid codes <WO 13873>
                        barCodeData.Substring(0, 7).Contains("05080")) // only use the rule with their product id of "05080"
                    {
                        this.ClearResetBarCodeProperties(false);
                        BarCodeParseUCC_EAN_128_GreenleaPremierMeats_Nossack barCodeParse = new BarCodeParseUCC_EAN_128_GreenleaPremierMeats_Nossack();
                        barCodeParse.ParseBarCodeData(barCodeData);
                        return;
                    }
                }

                do
                {
                    compare = parse;
                    parse = this.IdentifySSCC18(parse);
                    parse = this.IdentifyLocationIdentifier(parse);
                    parse = this.IdentifyGTIN(parse);
                    parse = this.IdentifyGTINinLogisticUnit(parse);
                    parse = this.IdentifyLogisticUnitBestBeforeSellByDate(parse);
                    parse = this.IdentifyLogisticUnitExpiryUseByDate(parse);
                    parse = this.IdentifyLogisticUnitDueDate(parse);
                    parse = this.IdentifyLogisticUnitProductionDate(parse);
                    parse = this.IdentifyLogisticUnitPackagingDate(parse);
                    parse = this.IdentifyLogisticUnitProductVariant(parse);
                    parse = this.IdentifyLogisticUnitCountOfTradeItems(parse);
                    parse = this.IdentifyLogisticUnitVariableCount(parse);
                    parse = this.IdentifyLogisticUnitBatchLotNumber(parse);
                    parse = this.IdentifySingleTradeItemSerialNumber(parse);
                    parse = this.IdentifyNetWeightMetricKG(parse);
                    parse = this.IdentifyNetWeightNonMetricLB(parse);
                    parse = this.IdentifyGrossWeightMetricKG(parse);
                    parse = this.IdentifyGrossWeightNonMetricLB(parse);
                    parse = this.IdentifyLengthOr1stDimensionMeters(parse);
                    parse = this.IdentifyActivityFKRefno(parse);
                    parse = this.IdentifyActivityItemFKRefno(parse);
                    parse = this.IdentifyActivityDetailFKRefno(parse);
                }
                while (string.Compare(parse, compare) != 0);

                this.IdentifyProductIdentifier(barCodeData);
                this.Remainder = parse;

                if (this.Settings.IsRuleEnabled_ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00
                    && !string.IsNullOrEmpty(this.SSCC18) && !string.IsNullOrEmpty(this.ProductIdentifier))
                {
                    this.SSCC18 = string.Empty;
                }

                SuppressErrorsRule();
            }
            catch (BarCodeParseException)
            {
                throw; // do not handle already formatted exception
            }
            catch (Exception e)
            {
                // General Barcode parsing failure.
                throw new BarCodeParseException(
                    string.Format(
#if MULTILANGUAGE
                        Translator.GetInstance().Translate(TranslatorVocabulary.FailedToParse),
#else
                        "Failed to parse '{0}'.",
#endif
                        barCodeData),
                    e);
            }
            finally
            {
                CleanupRules();
            }
        }


        #endregion

        #region Protected Methods

        /// <summary>
        /// Cleans up any extra rule information.
        /// </summary>
        protected void CleanupRules()
        {
            // rule not enabled, let us clear the remaining errors
            if (!this.Settings.IsRuleEnabled_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents)
            {
                if (this.Details != null)
                    this.Details.ClearErrors();
            }
        }

        /// <summary>
        /// Checks a rule if the errors should be suppressed.
        /// If not suppressed, throw errors like normal.
        /// </summary>
        protected void SuppressErrorsRule()
        {
            if (!this.Settings.IsRuleEnabled_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents &&
                this.Details.HasErrors)
            {
                // if rule is disabled, throw right now. Guarantee that non-suppressed are thrown
                this.Details.ThrowOnlyOneError(0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            string tempData = barCodeData;

            if (!string.IsNullOrEmpty(this.GTIN))
            {
                this.ProductIdentifier = this.GTIN;
            }
            else if (!string.IsNullOrEmpty(this.ActFKRefno))
            {
                this.ProductIdentifier = string.Empty;
            }
            else if (!string.IsNullOrEmpty(this.ActItemFKRefno))
            {
                this.ProductIdentifier = string.Empty;
            }
            else if (!string.IsNullOrEmpty(this.ActDetailFKRefno))
            {
                this.ProductIdentifier = string.Empty;
            }
            else if (!string.IsNullOrEmpty(this.LocationIdentifier))
            {
                this.ProductIdentifier = string.Empty;
            }
            else if (!string.IsNullOrEmpty(this.SSCC18))
            {
                this.ProductIdentifier = string.Empty;
            }
            else
            {
                if (tempData.Length > 14)
                {
                    this.ProductIdentifier = tempData.Substring(0, 14);
                }
                else
                {
                    this.ProductIdentifier = tempData;
                }
            }

            return tempData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyCompanyPrefix(string barCodeData)
        {
            string tempData = barCodeData;

            if (!string.IsNullOrEmpty(this.SystemGTINCompanyPrefix))
            {
                if (tempData.Contains(this.SystemGTINCompanyPrefix))
                {
                    this.CompanyPrefix = this.SystemGTINCompanyPrefix;
                }
            }

            return tempData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyGTIN(string barCodeData)
        {
            string tempData = barCodeData;

            if (tempData.StartsWith("01") && tempData.Length >= 16)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // assign the GTIN
                this.GTIN = tempData.Substring(0, 14);

                // remove the GTIN from the bar code data
                tempData = tempData.Remove(0, 14);

                this.IdentifyCompanyPrefix(this.GTIN);
            }
            else
            {
                if (this.Settings.IsRuleEnabled_ParseWOOLWICH_Code_128_RemoveSpaces)
                {
                    // If the data length is 18 and there are 4 spaces in it
                    // format '4 00 62482 00603 9'
                    if (tempData.Length == 18
                        && tempData.Length - tempData.Replace(" ", string.Empty).Length == 4)
                    {
                        this.GTIN = tempData.Replace(" ", string.Empty);

                        // remove the GTIN from the bar code data
                        tempData = tempData.Remove(0, 18);
                    }
                    else if (tempData.Length == 17
                        && tempData.Length - tempData.Replace(" ", string.Empty).Length == 3)
                    {
                        //// If the data length is 17 and there are 3 spaces in it
                        // format '400 62482 00603 9'
                        this.GTIN = tempData.Replace(" ", string.Empty);
                    }
                }

                if (this.Settings.IsRuleEnabled_Parse_Renees_Code_128_RemoveSpaces)
                {
                    // If the data length is 18 and there are 4 spaces in it
                    // format '7 00 62287 31202 2'
                    if (tempData.Length == 18
                        && tempData.Length - tempData.Replace(" ", string.Empty).Length == 4)
                    {
                        this.GTIN = tempData.Replace(" ", string.Empty);

                        // remove the GTIN from the bar code data
                        tempData = tempData.Remove(0, 18);
                    }
                }
            }

            return tempData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyGTINinLogisticUnit(string barCodeData)
        {
            string tempData = barCodeData;
            if (tempData.StartsWith("02") && tempData.Length >= 16)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // assign the GTIN Contained in a Logistic Unit
                this.GTINLogisticUnit = tempData.Substring(0, 14);

                // remove the GTIN from the bar code data
                tempData = tempData.Remove(0, 14);
            }

            return tempData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifySSCC18(string barCodeData)
        {
            string tempData = barCodeData;

            if (tempData.StartsWith("00") && tempData.Length >= 20)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // assign the SSCC
                this.SSCC18 = tempData.Substring(0, 18);

                // remove the extension digit
                tempData = tempData.Remove(0, 1);

                // remove the SSCC from the bar code data
                tempData = tempData.Remove(0, 16);

                // remove the check digit
                tempData = tempData.Remove(0, 1);

                this.IdentifyCompanyPrefix(this.SSCC18);
            }

            return tempData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyLogisticUnitBatchLotNumber(string barCodeData)
        {
            string tempData = barCodeData;

            // check length of 2 only because AI data is variable
            if (tempData.StartsWith("10") && tempData.Length >= 2)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);
                // variable AI look for group separator
                int aiIndex = this.FindGroupSeparatorInBarCode(tempData);
                int dataLength = 0;

                try
                {
                    if (aiIndex > -1)
                    {
                        dataLength = aiIndex;
                        aiIndex++;
                        if (dataLength > 20)
                        {
                            dataLength = 20;
                        }
                    }
                    else
                    {
                        aiIndex = tempData.Length;
                        dataLength = tempData.Length;
                        if (dataLength > 20)
                        {
                            dataLength = 20;
                            aiIndex = 20;
                        }
                    }

                    if (this.Settings.IsRuleEnabled_Parse_LanzMeats_Code_128_AI10_as_AI21 && this.GTIN.Contains("99419701"))
                    {
                        this.SingleTradeItemSerialNumber = tempData.Substring(0, dataLength);
                    }
                    else
                    {
                        // assign the BATCH LOT NUMBER
                        this.LogisticUnitBatchLotNumber = tempData.Substring(0, dataLength);
                    }
                }
                catch (Exception e)
                {
                    this.AddParseErrorDetail(
                        BarCodeExceptionReason.BatchLotNumber_InvalidFormat,
                        new BarCodeParseException(
                            string.Format(
#if MULTILANGUAGE
                                Translator.GetInstance().Translate(TranslatorVocabulary.FailedToResizeStringToLength),
#else
                                "Failed to resize string '{0}' to length {1}.",
#endif
                                tempData ?? "",
                                dataLength),
#if MULTILANGUAGE
                            Translator.GetInstance().Translate(TranslatorVocabulary.BatchLotNumberz),
#else
                            "Batch Lot Number",
#endif
                            e));
                }
                // remove the Batch / Lot Number from the bar code data
                tempData = tempData.Remove(0, aiIndex);


            }

            return tempData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifySingleTradeItemSerialNumber(string barCodeData)
        {
            string tempData = barCodeData;

            // check length of 2 only because AI data is variable
            if (tempData.StartsWith("21") && tempData.Length >= 2)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // variable AI look for group separator
                int aiIndex = this.FindGroupSeparatorInBarCode(tempData);
                int dataLength = 0;

                try
                {
                    if (aiIndex > -1)
                    {
                        dataLength = aiIndex;
                        aiIndex++;
                        if (dataLength > 20)
                        {
                            dataLength = 20;
                        }
                    }
                    else
                    {
                        aiIndex = tempData.Length;
                        dataLength = tempData.Length;
                        if (dataLength > 20)
                        {
                            dataLength = 20;
                            aiIndex = 20;
                        }
                    }

                    // assign the SERIAL NUMBER
                    this.SingleTradeItemSerialNumber = tempData.Substring(0, dataLength);
                }
                catch (Exception e)
                {
                    this.AddParseErrorDetail(
                        BarCodeExceptionReason.SerialNumber_InvalidFormat,
                        new BarCodeParseException(
                            string.Format(
#if MULTILANGUAGE
                                Translator.GetInstance().Translate(TranslatorVocabulary.FailedToResizeStringToLength),
#else
                                "Failed to resize string '{0}' to length {1}.",
#endif
                                tempData ?? "",
                                dataLength),
#if MULTILANGUAGE
                            Translator.GetInstance().Translate(TranslatorVocabulary.SerialNumber), e));
#else
                            "Serial Number", e));
#endif
                }
                // remove the Batch / Lot Number from the bar code data
                tempData = tempData.Remove(0, aiIndex);
            }

            return tempData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyLogisticUnitProductionDate(string barCodeData)
        {
            string tempData = barCodeData;

            if (tempData.StartsWith("11") && tempData.Length >= 8)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                try
                {
                    // assign the PRODUCTION DATE
                    int year = Convert.ToInt32(tempData.Substring(0, 2));
                    if (year < 80)
                    {
                        year = year + 2000;
                    }
                    else
                    {
                        year = year + 1900;
                    }

                    DateTime dt = new DateTime(
                        year,
                        Convert.ToInt32(tempData.Substring(2, 2)),
                        Convert.ToInt32(tempData.Substring(4, 2)));   // 101026

                    this.LogisticUnitProductionDate = dt;
                }
                catch (Exception e)
                {
                    this.AddParseErrorDetail(
                        BarCodeExceptionReason.ProductionDate_InvalidDate,
                        new BarCodeParseException(
                            string.Format(
#if MULTILANGUAGE
                                Translator.GetInstance().Translate(TranslatorVocabulary.CannotConvertToAValidYymmddDatetime),
#else
                                "Cannot convert {0} to a valid YYMMDD DateTime.",
#endif
                                barCodeData.Substring(2, 6)),
#if MULTILANGUAGE
                            Translator.GetInstance().Translate(TranslatorVocabulary.ProductionDatezz), e));
#else
                            "Production Date", e));
#endif
                }
                // remove the PRODUCTION DATE from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyLogisticUnitDueDate(string barCodeData)
        {
            string tempData = barCodeData;
            if (tempData.StartsWith("12") && tempData.Length >= 8)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                try
                {
                    // assign the DUE DATE
                    int year = Convert.ToInt32(tempData.Substring(0, 2));
                    if (year < 80)
                    {
                        year = year + 2000;
                    }
                    else
                    {
                        year = year + 1900;
                    }

                    DateTime dt = new DateTime(
                        year,
                        Convert.ToInt32(tempData.Substring(2, 2)),
                        Convert.ToInt32(tempData.Substring(4, 2)));   // 101026
                    this.LogisticUnitDueDate = dt;
                }
                catch (Exception e)
                {
                    this.AddParseErrorDetail(
                        BarCodeExceptionReason.DueDate_InvalidDate,
                        new BarCodeParseException(
                            string.Format(
#if MULTILANGUAGE
                                Translator.GetInstance().Translate(TranslatorVocabulary.CannotConvertToAValidYymmddDatetime),
#else
                                "Cannot convert {0} to a valid YYMMDD DateTime.",
#endif
                                barCodeData.Substring(2, 6)),
#if MULTILANGUAGE
                            Translator.GetInstance().Translate(TranslatorVocabulary.DueDatez), e));
#else
                            "Due Date", e));
#endif
                }

                // remove the DUE DATE from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyLogisticUnitPackagingDate(string barCodeData)
        {
            string tempData = barCodeData;
            if (tempData.StartsWith("13") && tempData.Length >= 8)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);
                try
                {
                    // assign the PACKAGING DATE
                    int year = Convert.ToInt32(tempData.Substring(0, 2));
                    if (year < 80)
                    {
                        year = year + 2000;
                    }
                    else
                    {
                        year = year + 1900;
                    }

                    DateTime dt = new DateTime(
                        year,
                        Convert.ToInt32(tempData.Substring(2, 2)),
                        Convert.ToInt32(tempData.Substring(4, 2)));   // 101026

                    if (this.Settings.IsRuleEnabled_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate)
                    {
                        this.LogisticUnitProductionDate = dt;
                    }
                    else
                    {
                        this.LogisticUnitPackagingDate = dt;
                    }
                }
                catch (Exception e)
                {
                    if (this.Settings.IsRuleEnabled_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate)
                    {
                        this.AddParseErrorDetail(
                           BarCodeExceptionReason.PackagingDateAsProductionDate_InvalidDate,
                           new BarCodeParseException(
                               string.Format(
#if MULTILANGUAGE
                                   Translator.GetInstance().Translate(TranslatorVocabulary.CannotConvertToAValidYymmddDatetime) + Environment.NewLine +
                                   Translator.GetInstance().Translate(TranslatorVocabulary.ParsePackagingDateAsProductionDateEnabled),
#else
                                   "Cannot convert {0} to a valid YYMMDD DateTime." + Environment.NewLine +
                                   "Parse 'Packaging Date' as 'Production Date' enabled.",
#endif
                                   barCodeData.Substring(2, 6)),
#if MULTILANGUAGE
                               Translator.GetInstance().Translate(TranslatorVocabulary.ProductionDatezz), e));
#else
                               "Production Date", e));
#endif
                    }
                    else
                    {
                        this.AddParseErrorDetail(
                           BarCodeExceptionReason.PackagingDate_InvalidDate,
                           new BarCodeParseException(
                               string.Format(
#if MULTILANGUAGE
                                   Translator.GetInstance().Translate(TranslatorVocabulary.CannotConvertToAValidYymmddDatetime),
#else
                                   "Cannot convert {0} to a valid YYMMDD DateTime.",
#endif
                                   barCodeData.Substring(2, 6)),
#if MULTILANGUAGE
                               Translator.GetInstance().Translate(TranslatorVocabulary.PackagingDatez), e));
#else
                               "Packaging Date", e));
#endif
                    }
                }

                // remove the PACKAGING DATE from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyLogisticUnitBestBeforeSellByDate(string barCodeData)
        {
            string tempData = barCodeData;
            if (tempData.StartsWith("15") && tempData.Length >= 8)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                try
                {
                    // assign the BEST BEFORE SELL BY DATE
                    int year = Convert.ToInt32(tempData.Substring(0, 2));
                    if (year < 80)
                    {
                        year = year + 2000;
                    }
                    else
                    {
                        year = year + 1900;
                    }

                    DateTime dt = new DateTime(
                        year,
                        Convert.ToInt32(tempData.Substring(2, 2)),
                        Convert.ToInt32(tempData.Substring(4, 2)));   // 101026

                    this.LogisticUnitBestBeforeSellByDate = dt;
                }
                catch (Exception e)
                {
                    this.AddParseErrorDetail(
                        BarCodeExceptionReason.BestBeforeSellByDate_InvalidDate,
                        new BarCodeParseException(
                            string.Format(
#if MULTILANGUAGE
                                Translator.GetInstance().Translate(TranslatorVocabulary.CannotConvertToAValidYymmddDatetime),
#else
                                "Cannot convert {0} to a valid YYMMDD DateTime.",
#endif
                                barCodeData.Substring(2, 6)),
#if MULTILANGUAGE
                            Translator.GetInstance().Translate(TranslatorVocabulary.BestBeforeSellByDate), e));
#else
                            "Best Before Sell By Date", e));
#endif
                }

                // remove the BEST BEFORE SELL BY DATE from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyLogisticUnitExpiryUseByDate(string barCodeData)
        {
            string tempData = barCodeData;
            if (tempData.StartsWith("17") && tempData.Length >= 8)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                try
                {
                    // assign the EXPIRY USE BY SELL BY DATE
                    int year = Convert.ToInt32(tempData.Substring(0, 2));
                    if (year < 80)
                    {
                        year = year + 2000;
                    }
                    else
                    {
                        year = year + 1900;
                    }

                    DateTime dt = new DateTime(
                        year,
                        Convert.ToInt32(tempData.Substring(2, 2)),
                        Convert.ToInt32(tempData.Substring(4, 2)));   // 101026

                    if (Settings.IsRuleEnabled_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate)
                    {
                        this.LogisticUnitBestBeforeSellByDate = dt;
                    }
                    else
                    {
                        this.LogisticUnitExpiryUseByDate = dt;
                    }
                }
                catch (Exception e)
                {
                    this.AddParseErrorDetail(
                        BarCodeExceptionReason.ExpiryUseByDate_InvalidDate,
                        new BarCodeParseException(
                            string.Format(
#if MULTILANGUAGE
                                Translator.GetInstance().Translate(TranslatorVocabulary.CannotConvertToAValidYymmddDatetime),
#else
                                "Cannot convert {0} to a valid YYMMDD DateTime.",
#endif
                                barCodeData.Substring(2, 6)),
#if MULTILANGUAGE
                            Translator.GetInstance().Translate(TranslatorVocabulary.ExpiryUseBySellByDate), e));
#else
                            "Expiry Use By Sell By Date", e));
#endif
                }

                // remove the EXPIRY USE BY DATE from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyLogisticUnitProductVariant(string barCodeData)
        {
            string tempData = barCodeData;
            if (tempData.StartsWith("20") && tempData.Length >= 4)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);
                try
                {
                    // assign the PRODUCT VARIANT
                    this.LogisticUnitProductVariant = Convert.ToInt32(tempData.Substring(0, 2));
                }
                catch (Exception e)
                {
                    this.AddParseErrorDetail(
                        BarCodeExceptionReason.ProductVariant_InvalidInteger,
                        new BarCodeParseException(
                        string.Format(
#if MULTILANGUAGE
                            Translator.GetInstance().Translate(TranslatorVocabulary.CannotConvertToAnInteger),
#else
                            "Cannot convert {0} to an integer.",
#endif
                            barCodeData.Substring(2, 2)),
#if MULTILANGUAGE
                        Translator.GetInstance().Translate(TranslatorVocabulary.ProductVariantz), e));
#else
                        "Product Variant", e));
#endif
                }

                // remove the PRODUCT VARIANT from the bar code data
                tempData = tempData.Remove(0, 2);
            }

            return tempData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyLogisticUnitCountOfTradeItems(string barCodeData)
        {
            string tempData = barCodeData;
            if (tempData.StartsWith("37") && tempData.Length >= 10)
            {
                // remove the AI 
                tempData = tempData.Remove(0, 2);
                try
                {
                    // assign the Count of Trade Items
                    this.LogisticUnitCountOfTradeItems = Convert.ToInt32(
                        tempData.Substring(0, 8));
                }
                catch (Exception e)
                {
                    this.AddParseErrorDetail(
                        BarCodeExceptionReason.CountOfTradeItems_InvalidInteger,
                        new BarCodeParseException(
                            string.Format(
#if MULTILANGUAGE
                                Translator.GetInstance().Translate(TranslatorVocabulary.CannotConvertToAnInteger),
#else
                                "Cannot convert {0} to an integer.",
#endif
                                barCodeData.Substring(2, 8)),
#if MULTILANGUAGE
                            Translator.GetInstance().Translate(TranslatorVocabulary.CountOfTradeItemsz), e));
#else
                            "Count of Trade Items", e));
#endif
                }

                // remove the COUNT OF TRADE ITEMS from the bar code data
                tempData = tempData.Remove(0, 8);
            }

            return tempData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyLogisticUnitVariableCount(string barCodeData)
        {
            string codeData = barCodeData;

            // Check the entire length (this.Data) not the barCodeData.length because it could have been parsed and altered.
            if (Settings.IsRuleEnabled_Parse_Code128_China_Seafood
                && barCodeData.StartsWith("30")
                && this.Length == 14)
            {
                // do not parse the AI
            }
            else
            {
                if (codeData.StartsWith("30") && codeData.Length >= 3)
                {
                    bool parseIt = true;

                    if (Settings.IsRuleEnabled_Parse_Code_128_AI01_BC_AI30_exclusive)
                    {
                        if (string.IsNullOrEmpty(this.CompanyPrefix))
                        {
                            parseIt = false;
                        }
                    }

                    if (parseIt)
                    {
                        // remove the AI
                        codeData = codeData.Remove(0, 2);

                        // variable AI look for group separator
                        int aiIndex = this.FindGroupSeparatorInBarCode(codeData);
                        int dataLength = 0;

                        try
                        {
                            if (aiIndex > -1)
                            {
                                dataLength = aiIndex;
                                aiIndex++;
                                if (dataLength > 8)
                                {
                                    dataLength = 8;
                                }
                            }
                            else
                            {
                                aiIndex = codeData.Length;
                                dataLength = codeData.Length;
                                if (dataLength > 8)
                                {
                                    dataLength = 8;
                                    aiIndex = 8;
                                }
                            }

                            // assign the VARIABLE COUNT
                            this.LogisticUnitVariableCount = Convert.ToInt32(codeData.Substring(0, dataLength));
                        }
                        catch (Exception e)
                        {
                            this.AddParseErrorDetail(
                                BarCodeExceptionReason.VariableCount_InvalidInteger,
                                new BarCodeParseException(
                                string.Format(
#if MULTILANGUAGE
                                    Translator.GetInstance().Translate(TranslatorVocabulary.CannotConvertToAnInteger),
#else
                                    "Cannot convert {0} to an integer.",
#endif
                                    codeData.Substring(0, dataLength)),
#if MULTILANGUAGE
                                Translator.GetInstance().Translate(TranslatorVocabulary.VariableCountz), e));
#else
                                "Variable Count", e));
#endif
                        }

                        // remove the Variable Count from the bar code data
                        codeData = codeData.Remove(0, aiIndex);
                    }
                }
            }

            return codeData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyLocationIdentifier(string barCodeData)
        {
            string tempData = barCodeData;


            // <revision date="20160928" />
            // Used to parse 414 && length >= 16
            // Changing to not care about length because some barcodes have smaller lengths due to legacy bugs.
            if (tempData.StartsWith("414"))
            {
                // remove the AI
                tempData = tempData.Remove(0, 3);

                int locationLength = Math.Min(13, tempData.Length);

                // assign the LOCATION IDENTIFIER
                // allowing alpha is not UCC/EAN compliant. But we need to support
                this.LocationIdentifier = tempData.Substring(0, locationLength);

                // remove the LOCATION IDENTIFIER from the bar code data
                tempData = tempData.Remove(0, locationLength);

                this.IdentifyCompanyPrefix(this.LocationIdentifier);
            }

            return tempData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyNetWeightMetricKG(string barCodeData)
        {
            string tempData = barCodeData;

            if (tempData.StartsWith("310") && tempData.Length >= 10)
            {
                this.NetWeightMetricKGFoundAI = true;

                // get the number of decimal places
                int places = 0;
                bool placesFound = false;
                try
                {
                    places = Convert.ToInt32(tempData.Substring(3, 1));
                    placesFound = true;

                    // remove the AI
                    tempData = tempData.Remove(0, 4);
                    // assign the METRIC NET WEIGHT KG
                    string strWholeNumber = tempData.Substring(0, 6);
                    if (places != 0)
                    {
                        string s = strWholeNumber.Substring(0, strWholeNumber.Length - places) + "."
                            + strWholeNumber.Substring(strWholeNumber.Length - places);
                        this.NetWeightMetricKG = Convert.ToDecimal(s);
                    }
                    else
                    {
                        this.NetWeightMetricKG = Convert.ToDecimal(strWholeNumber);
                    }
                }
                catch (Exception e)
                {
                    if (placesFound)
                    {
                        this.AddParseErrorDetail(
                            BarCodeExceptionReason.NetWeightMetricKG_InvalidDecimal,
                            new BarCodeParseException(
#if MULTILANGUAGE
                                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.FailedToParsez), barCodeData),
                            Translator.GetInstance().Translate(TranslatorVocabulary.MetricNetWeightKg), e));
#else
                                string.Format("Failed to parse '{0}'", barCodeData),
                            "Metric Net Weight KG", e));
#endif
                    }
                    else
                    {
                        this.AddParseErrorDetail(
                           BarCodeExceptionReason.NetWeightMetricKG_UnknownDecimalPlaces,
                           new BarCodeParseException(
                               string.Format(
#if MULTILANGUAGE
                                Translator.GetInstance().Translate(TranslatorVocabulary.FailedToParseCouldNotDetermineNumberOfDecimalPlaces),
#else
                                "Failed to parse '{0}'. Could not determine number of decimal places.",
#endif
                               barCodeData),
#if MULTILANGUAGE
                           Translator.GetInstance().Translate(TranslatorVocabulary.MetricNetWeightKg), e));
#else
                           "Metric Net Weight KG", e));
#endif
                    }
                }
                // remove the METRIC NET WEIGHT KG from the bar code data
                tempData = barCodeData.Substring(10);
            }

            return tempData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyNetWeightNonMetricLB(string barCodeData)
        {
            string tempData = barCodeData;

            if (tempData.StartsWith("320") && tempData.Length >= 10)
            {
                this.NetWeightNonMetricLBFoundAI = true;
                bool placesFound = false;
                try
                {
                    // get the number of decimal places
                    int places = 0;
                    places = Convert.ToInt32(tempData.Substring(3, 1));
                    placesFound = true;

                    // remove the AI
                    tempData = tempData.Remove(0, 4);

                    // assign the NON METRIC NET WEIGHT LB
                    string strWholeNumber = tempData.Substring(0, 6);
                    if (places != 0)
                    {
                        string s = strWholeNumber.Substring(0, strWholeNumber.Length - places) + "."
                            + strWholeNumber.Substring(strWholeNumber.Length - places);
                        this.NetWeightNonMetricLB = Convert.ToDecimal(s);
                    }
                    else
                    {
                        this.NetWeightNonMetricLB = Convert.ToDecimal(strWholeNumber);
                    }

                }
                catch (Exception e)
                {
                    if (placesFound)
                    {
                        this.AddParseErrorDetail(
                            BarCodeExceptionReason.NetWeightNonMetricLB_InvalidDecimal,
                            new BarCodeParseException(
#if MULTILANGUAGE
                                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.FailedToParse), barCodeData),
                                Translator.GetInstance().Translate(TranslatorVocabulary.NonMetricNetWeightLb), e));
#else
                                string.Format("Failed to parse '{0}'.", barCodeData),
                                "Non Metric Net Weight LB", e));
#endif
                    }
                    else
                    {
                        this.AddParseErrorDetail(
                            BarCodeExceptionReason.NetWeightNonMetricLB_UnknownDecimalPlaces,
                            new BarCodeParseException(
                                string.Format(
#if MULTILANGUAGE
                                    Translator.GetInstance().Translate(TranslatorVocabulary.FailedToParseCouldNotDetermineNumberOfDecimalPlaces),
#else
                                    "Failed to parse '{0}'. Could not determine number of decimal places.",
#endif
                                    barCodeData),
#if MULTILANGUAGE
                                Translator.GetInstance().Translate(TranslatorVocabulary.NonMetricNetWeightLb), e));
#else
                                "Non Metric Net Weight LB", e));
#endif
                    }
                }

                // remove the NON METRIC NET WEIGHT LB from the bar code data
                tempData = barCodeData.Substring(10);
            }

            return tempData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyGrossWeightMetricKG(string barCodeData)
        {
            string tempData = barCodeData;

            if (tempData.StartsWith("330") && tempData.Length >= 10)
            {
                this.GrossWeightMetricKGFoundAI = true;

                bool placesFound = false;
                try
                {
                    // get the number of decimal places
                    int places = 0;
                    places = Convert.ToInt32(tempData.Substring(3, 1));
                    placesFound = true;

                    // remove the AI
                    tempData = tempData.Remove(0, 4);

                    // assign the NON METRIC GROSS WEIGHT LB
                    string strWholeNumber = tempData.Substring(0, 6);
                    if (places != 0)
                    {
                        string s = strWholeNumber.Substring(0, strWholeNumber.Length - places) + "."
                            + strWholeNumber.Substring(strWholeNumber.Length - places);
                        this.GrossWeightMetricKG = Convert.ToDecimal(s);
                    }
                    else
                    {
                        this.GrossWeightMetricKG = Convert.ToDecimal(strWholeNumber);
                    }
                }
                catch (Exception e)
                {
                    if (placesFound)
                    {
                        this.AddParseErrorDetail(
                            BarCodeExceptionReason.GrossWeightMetricKG_InvalidDecimal,
                            new BarCodeParseException(
#if MULTILANGUAGE
                                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.FailedToParsez), barCodeData),
                            Translator.GetInstance().Translate(TranslatorVocabulary.MetricGrossWeightKg), e));
#else
                                string.Format("Failed to parse '{0}'", barCodeData),
                            "Metric Gross Weight KG", e));
#endif
                    }
                    else
                    {
                        this.AddParseErrorDetail(
                               BarCodeExceptionReason.GrossWeightMetricKG_UnknownDecimalPlaces,
                               new BarCodeParseException(
                                   string.Format(
#if MULTILANGUAGE
                                    Translator.GetInstance().Translate(TranslatorVocabulary.FailedToParseCouldNotDetermineNumberOfDecimalPlaces),
#else
                                    "Failed to parse '{0}'. Could not determine number of decimal places.",
#endif
                                    barCodeData),
#if MULTILANGUAGE
                               Translator.GetInstance().Translate(TranslatorVocabulary.MetricGrossWeightKg), e));
#else
                               "Metric Gross Weight KG", e));
#endif
                    }
                }
                // remove the NON METRIC WEIGHT LB from the bar code data

                // Change to substring of original to remove what was consumed.
                // This works even if an exception was thrown prior which would've removed the leading 4
                tempData = barCodeData.Substring(10);

            }

            return tempData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyGrossWeightNonMetricLB(string barCodeData)
        {
            string tempData = barCodeData;
            if (tempData.StartsWith("340") && tempData.Length >= 10)
            {
                this.GrossWeightNonMetricLBFoundAI = true;

                bool placesFound = false;
                try
                {
                    // get the number of decimal places
                    int places = 0;
                    places = Convert.ToInt32(tempData.Substring(3, 1));
                    placesFound = true;

                    // remove the AI
                    tempData = tempData.Remove(0, 4);

                    // assign the NON METRIC GROSS WEIGHT LB
                    string strWholeNumber = tempData.Substring(0, 6);
                    if (places != 0)
                    {
                        string s = strWholeNumber.Substring(0, strWholeNumber.Length - places) + "."
                            + strWholeNumber.Substring(strWholeNumber.Length - places);
                        this.GrossWeightNonMetricLB = Convert.ToDecimal(s);
                    }
                    else
                    {
                        this.GrossWeightNonMetricLB = Convert.ToDecimal(strWholeNumber);
                    }

                }
                catch (Exception e)
                {
                    if (placesFound)
                    {
                        this.AddParseErrorDetail(
                            BarCodeExceptionReason.GrossWeightNonMetricLB_InvalidDecimal,
                            new BarCodeParseException(
#if MULTILANGUAGE
                            string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.FailedToParsez), barCodeData),
                            Translator.GetInstance().Translate(TranslatorVocabulary.NonMetricGrossWeightLb),
#else
                            string.Format("Failed to parse '{0}'", barCodeData),
                            "Non Metric Gross Weight LB",
#endif
                            e));
                    }
                    else
                    {
                        this.AddParseErrorDetail(
                               BarCodeExceptionReason.GrossWeightNonMetricLB_UnknownDecimalPlaces,
                               new BarCodeParseException(
                                   string.Format(
#if MULTILANGUAGE
                                    Translator.GetInstance().Translate(TranslatorVocabulary.FailedToParseCouldNotDetermineNumberOfDecimalPlaces),
#else
                                    "Failed to parse '{0}'. Could not determine number of decimal places.",
#endif
                                    barCodeData),
#if MULTILANGUAGE
                               Translator.GetInstance().Translate(TranslatorVocabulary.NonMetricGrossWeightLb), e));
#else
                               "Non Metric Gross Weight LB", e));
#endif
                    }
                }

                // remove the NON METRIC GROSS WEIGHT LB from the bar code data
                tempData = barCodeData.Substring(10);
            }

            return tempData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyLengthOr1stDimensionMeters(string barCodeData)
        {
            string tempData = barCodeData;
            if (tempData.StartsWith("311") && tempData.Length >= 10)
            {
                bool placesFound = false;
                try
                {
                    // get the number of decimal places
                    int places = 0;
                    places = Convert.ToInt32(tempData.Substring(3, 1));
                    placesFound = true;

                    // remove the AI
                    tempData = tempData.Remove(0, 4);

                    // assign the LENGTH IN METERS
                    string strWholeNumber = tempData.Substring(0, 6);
                    if (places != 0)
                    {
                        string s = strWholeNumber.Substring(0, strWholeNumber.Length - places) + "."
                            + strWholeNumber.Substring(strWholeNumber.Length - places);
                        this.LengthOr1stDimensionMeters = Convert.ToDecimal(s);
                    }
                    else
                    {
                        this.LengthOr1stDimensionMeters = Convert.ToDecimal(strWholeNumber);
                    }

                }
                catch (Exception e)
                {
                    if (placesFound)
                    {
                        this.AddParseErrorDetail(
                            BarCodeExceptionReason.DimensionMeters_InvalidDecimal,
                            new BarCodeParseException(
#if MULTILANGUAGE
                                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.FailedToParsez), barCodeData),
                                Translator.GetInstance().Translate(TranslatorVocabulary.LengthInMeters),
#else
                                string.Format("Failed to parse '{0}'", barCodeData),
                                "Length in Meters",
#endif
                            e));
                    }
                    else
                    {
                        this.AddParseErrorDetail(
                            BarCodeExceptionReason.DimensionMeters_UnknownDecimalPlaces,
                            new BarCodeParseException(
                                string.Format(
#if MULTILANGUAGE
                                    Translator.GetInstance().Translate(TranslatorVocabulary.FailedToParseCouldNotDetermineNumberOfDecimalPlaces),
#else
                                    "Failed to parse '{0}'. Could not determine number of decimal places.",
#endif
                                    barCodeData),
#if MULTILANGUAGE
                                Translator.GetInstance().Translate(TranslatorVocabulary.LengthInMeters),
#else
                                "Length in Meters",
#endif
                            e));
                    }
                }

                // remove the LENGTH IN METERS from the bar code data
                tempData = barCodeData.Substring(10);
            }

            return tempData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyActivityFKRefno(string barCodeData)
        {
            string tempData = barCodeData;

            // check length of 2 only because AI data is variable
            if (tempData.StartsWith("92") && tempData.Length >= 2)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // variable AI look for group separator
                int aiIndex = this.FindGroupSeparatorInBarCode(tempData);
                int dataLength = 0;

                try
                {
                    if (aiIndex > -1)
                    {
                        aiIndex++;
                        if (dataLength > 30)
                        {
                            dataLength = 30;
                        }
                    }
                    else
                    {
                        aiIndex = tempData.Length;
                        dataLength = tempData.Length;
                        if (dataLength > 30)
                        {
                            dataLength = 30;
                            aiIndex = 30;
                        }
                    }

                    // assign the Activity FK Refno
                    this.ActFKRefno = tempData.Substring(0, dataLength);

                }
                catch (Exception e)
                {
                    this.AddParseErrorDetail(
                        BarCodeExceptionReason.ActivityFKRefno_InvalidFormat,
                        new BarCodeParseException(
#if MULTILANGUAGE
                            string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.FailedToParsez), barCodeData),
#else
                            string.Format("Failed to parse '{0}'", barCodeData),
#endif
                            "Activity FK Refno",
                        e));
                }
                // remove the Activity FK Refno data from the bar code data
                tempData = tempData.Remove(0, aiIndex);
            }

            return tempData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyActivityItemFKRefno(string barCodeData)
        {
            string tempData = barCodeData;

            // check length of 2 only because AI data is variable
            if (tempData.StartsWith("91") && tempData.Length >= 2)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // variable AI look for group separator
                int aiIndex = this.FindGroupSeparatorInBarCode(tempData);
                int dataLength = 0;

                try
                {
                    if (aiIndex > -1)
                    {
                        aiIndex++;
                        if (dataLength > 30)
                        {
                            dataLength = 30;
                        }
                    }
                    else
                    {
                        aiIndex = tempData.Length;
                        dataLength = tempData.Length;
                        if (dataLength > 30)
                        {
                            dataLength = 30;
                            aiIndex = 30;
                        }
                    }

                    // assign the Activity Item FK Refno
                    this.ActItemFKRefno = tempData.Substring(0, dataLength);

                }
                catch (Exception e)
                {
                    this.AddParseErrorDetail(
                        BarCodeExceptionReason.ActivityItemFKRefno_InvalidFormat,
                        new BarCodeParseException(
#if MULTILANGUAGE
                            string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.FailedToParsez), barCodeData),
#else
                            string.Format("Failed to parse '{0}'", barCodeData),
#endif
                        "Activity Item FK Refno",
                        e));
                }
                // remove the Activity Item FK Refno data from the bar code data
                tempData = tempData.Remove(0, aiIndex);
            }

            return tempData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        protected override string IdentifyActivityDetailFKRefno(string barCodeData)
        {
            string tempData = barCodeData;

            // check length of 2 only because AI data is variable
            if (tempData.StartsWith("93") && tempData.Length >= 2)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // variable AI look for group separator
                int aiIndex = this.FindGroupSeparatorInBarCode(tempData);
                int dataLength = 0;

                try
                {
                    if (aiIndex > -1)
                    {
                        aiIndex++;
                        if (dataLength > 30)
                        {
                            dataLength = 30;
                        }
                    }
                    else
                    {
                        aiIndex = tempData.Length;
                        dataLength = tempData.Length;
                        if (dataLength > 30)
                        {
                            dataLength = 30;
                            aiIndex = 30;
                        }
                    }

                    // assign the Activity Detail FK Refno
                    this.ActDetailFKRefno = tempData.Substring(0, dataLength);

                }
                catch (Exception e)
                {
                    this.AddParseErrorDetail(BarCodeExceptionReason.ActivityDetailFKRefno_InvalidFormat,
                        new BarCodeParseException(
#if MULTILANGUAGE
                                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.FailedToParsez), barCodeData),
#else
                                string.Format("Failed to parse '{0}'", barCodeData),
#endif
                            "Activity Detail FK Refno",
                            e));
                }
                // remove the Activity Detail FK Refno data from the bar code data
                tempData = tempData.Remove(0, aiIndex);
            }

            return tempData;
        }

        #endregion
    }
}
