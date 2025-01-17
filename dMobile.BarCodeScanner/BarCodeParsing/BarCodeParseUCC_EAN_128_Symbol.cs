// <copyright file="BarCodeParseUCC_EAN_128_Symbol.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;
    using Bedrock;

    /// <summary>
    /// Represents a parser for UCC/EAN 128 bar code symbology.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class BarCodeParseUCC_EAN_128_Symbol : BarCodeParse
    {
        /// <summary>
        /// Parses the bar code data.
        /// </summary>
        /// <param name="barCodeData">The bar code data to parse.</param>
        public override void ParseBarCodeData(string barCodeData)
        {
            try
            {
                var parse = barCodeData;
                string compare;

                // GetType == typeof(...) makes sure the rules are checked once.
                // This way, a rule doesn't have to override ParseBarCodeData(...) with duplicate code.
                // It can instead just call this and run it again. Saves coding time, reduces duplicate code,
                // keeps code in sync.
                if (this.GetType() == typeof(BarCodeParseUCC_EAN_128_Symbol))
                {
                    if (this.Settings.IsRuleEnabled_ParseEAN_UCC128_NossackProduct94600_ChileanBackRibs &&
                        barCodeData.Length >= 14 &&
                        barCodeData.Substring(4, 6) == "896775")
                    {
                        this.ClearResetBarCodeProperties(resetSymbologyData: false);
                        var barCodeParse = new BarCodeParseUCC_EAN_128_Symbol_NossackProduct94600_ChileanBackRibs();
                        barCodeParse.ParseBarCodeData(barCodeData);
                        return;
                    }
                    else if (this.Settings.IsRuleEnabled_ParseEAN_UCC128_Dovre_KiiNaturalsCrisps &&
                             barCodeData.Length >= 14)
                    {
                        this.ClearResetBarCodeProperties(resetSymbologyData: false);
                        var barCodeParse = new BarCodeParseUCC_EAN_128_KiiNaturalsCrisps_Dovre();
                        barCodeParse.ParseBarCodeData(barCodeData);

                        // stop executing since we already went through this function as long as
                        // you don't override ParseBarCodeData
                        return;
                    }
                    else if (this.Settings.IsRuleEnabled_ParseEAN_UCC128_Nossack_GreenleaPremierMeats &&
                             barCodeData.Length == 20 &&
                             barCodeData[..2] != "00" &&
                             barCodeData[..7].Contains("05080"))
                    {
                        // don't parse out skid codes <WO 13873>
                        // only use the rule with their product id of "05080"
                        this.ClearResetBarCodeProperties(false);
                        var barCodeParse = new BarCodeParseUCC_EAN_128_GreenleaPremierMeats_Nossack();
                        barCodeParse.ParseBarCodeData(barCodeData);
                        return;
                    }
                }

                do
                {
                    compare = parse;
                    parse = this.IdentifySscc18(parse);
                    parse = this.IdentifyLocationIdentifier(parse);
                    parse = this.IdentifyGtin(parse);
                    parse = this.IdentifyGtiNinLogisticUnit(parse);
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
                    parse = this.IdentifyNetWeightMetricKg(parse);
                    parse = this.IdentifyNetWeightNonMetricLb(parse);
                    parse = this.IdentifyGrossWeightMetricKg(parse);
                    parse = this.IdentifyGrossWeightNonMetricLb(parse);
                    parse = this.IdentifyLengthOr1StDimensionMeters(parse);
                    parse = this.IdentifyActivityFkRefno(parse);
                    parse = this.IdentifyActivityItemFkRefno(parse);
                    parse = this.IdentifyActivityDetailFkRefno(parse);
                }
                while (string.CompareOrdinal(parse, compare) != 0);

                this.IdentifyProductIdentifier(barCodeData);
                this.Remainder = parse;

                if (this.Settings.IsRuleEnabled_ParseEAN_UCC128_NossackProduct51134_FrozenPorkCollarRemoveAI00 &&
                    !string.IsNullOrEmpty(this.Sscc18) &&
                    !string.IsNullOrEmpty(this.ProductIdentifier))
                {
                    this.Sscc18 = string.Empty;
                }

                this.SuppressErrorsRule();
            }
            catch (BarCodeParseException)
            {
                throw; // do not handle already formatted exception
            }
            catch (Exception e)
            {
                // General Barcode parsing failure.
                throw new BarCodeParseException($"Failed to parse '{barCodeData}'.", e);
            }
            finally
            {
                this.CleanupRules();
            }
        }

        /// <summary>
        /// Cleans up the parsing rules.
        /// </summary>
        protected void CleanupRules()
        {
            // rule not enabled, let us clear the remaining errors
            if (!this.Settings.IsRuleEnabled_ParseEAN_UCC128_SuppressAllParseErrorsForNonTrackedComponents)
            {
                this.Details?.ClearErrors();
            }
        }

        /// <summary>
        ///     Checks a rule if the errors should be suppressed.
        ///     If not suppressed, throw errors like normal.
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
        /// Identifies the product identifier from the bar code data.
        /// </summary>
        /// <param name="barCodeData">The bar code data.</param>
        /// <returns>The product identifier.</returns>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            var tempData = barCodeData;
            if (!string.IsNullOrEmpty(this.Gtin))
            {
                this.ProductIdentifier = this.Gtin;
            }
            else if (!string.IsNullOrEmpty(this.ActFkRefno))
            {
                this.ProductIdentifier = string.Empty;
            }
            else if (!string.IsNullOrEmpty(this.ActItemFkRefno))
            {
                this.ProductIdentifier = string.Empty;
            }
            else if (!string.IsNullOrEmpty(this.ActDetailFkRefno))
            {
                this.ProductIdentifier = string.Empty;
            }
            else if (!string.IsNullOrEmpty(this.LocationIdentifier))
            {
                this.ProductIdentifier = string.Empty;
            }
            else if (!string.IsNullOrEmpty(this.Sscc18))
            {
                this.ProductIdentifier = string.Empty;
            }
            else
            {
                this.ProductIdentifier = tempData.Length > 14 ? tempData[..14] : tempData;
            }

            return tempData;
        }

        /// <summary>
        ///     Identify the company prefix.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifyCompanyPrefix(string barCodeData)
        {
            var tempData = barCodeData;
            if (!string.IsNullOrEmpty(this.SystemGtinCompanyPrefix))
            {
                if (tempData.Contains(this.SystemGtinCompanyPrefix))
                {
                    this.CompanyPrefix = this.SystemGtinCompanyPrefix;
                }
            }

            return tempData;
        }

        /// <summary>
        ///     Identify the GTIN.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifyGtin(string barCodeData)
        {
            var tempData = barCodeData;
            if (tempData.StartsWith("01") && tempData.Length >= 16)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // assign the GTIN
                this.Gtin = tempData[..14];

                // remove the GTIN from the bar code data
                tempData = tempData.Remove(0, 14);

                this.IdentifyCompanyPrefix(this.Gtin);
            }
            else
            {
                if (this.Settings.IsRuleEnabled_ParseWOOLWICH_Code_128_RemoveSpaces)
                {
                    // If the data length is 18 and there are 4 spaces in it format '4 00 62482 00603 9'
                    if (tempData.Length == 18 && tempData.Length - tempData.Replace(" ", string.Empty).Length == 4)
                    {
                        this.Gtin = tempData.Replace(" ", string.Empty);

                        // remove the GTIN from the bar code data
                        tempData = tempData.Remove(0, 18);
                    }
                    else if (tempData.Length == 17 && tempData.Length - tempData.Replace(" ", string.Empty).Length == 3)
                    {
                        //// If the data length is 17 and there are 3 spaces in it
                        // format '400 62482 00603 9'
                        this.Gtin = tempData.Replace(" ", string.Empty);
                    }
                }

                if (this.Settings.IsRuleEnabled_Parse_Renees_Code_128_RemoveSpaces)
                {
                    // If the data length is 18 and there are 4 spaces in it format '7 00 62287 31202 2'
                    if (tempData.Length == 18 && tempData.Length - tempData.Replace(" ", string.Empty).Length == 4)
                    {
                        this.Gtin = tempData.Replace(" ", string.Empty);

                        // remove the GTIN from the bar code data
                        tempData = tempData.Remove(0, 18);
                    }
                }
            }

            return tempData;
        }

        /// <summary>
        ///     Identify the GTIN logistic unit.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifyGtiNinLogisticUnit(string barCodeData)
        {
            var tempData = barCodeData;
            if (tempData.StartsWith("02") && tempData.Length >= 16)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // assign the GTIN Contained in a Logistic Unit
                this.GtinLogisticUnit = tempData[..14];

                // remove the GTIN from the bar code data
                tempData = tempData.Remove(0, 14);
            }

            return tempData;
        }

        /// <summary>
        ///     Identify the SSCC18.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifySscc18(string barCodeData)
        {
            var tempData = barCodeData;
            if (tempData.StartsWith("00") && tempData.Length >= 20)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // assign the SSCC
                this.Sscc18 = tempData[..18];

                // remove the extension digit
                tempData = tempData.Remove(0, 1);

                // remove the SSCC from the bar code data
                tempData = tempData.Remove(0, 16);

                // remove the check digit
                tempData = tempData.Remove(0, 1);

                this.IdentifyCompanyPrefix(this.Sscc18);
            }

            return tempData;
        }

        /// <summary>
        ///     Identify the logistic unit batch lot number.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifyLogisticUnitBatchLotNumber(string barCodeData)
        {
            var tempData = barCodeData;

            // check length of 2 only because AI data is variable
            if (tempData.StartsWith("10") && tempData.Length >= 2)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // variable AI look for group separator
                var aiIndex = this.FindGroupSeparatorInBarCode(tempData);
                var dataLength = 0;

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

                    if (this.Settings.IsRuleEnabled_Parse_LanzMeats_Code_128_AI10_as_AI21 &&
                        this.Gtin.Contains("99419701"))
                    {
                        this.SingleTradeItemSerialNumber = tempData[..dataLength];
                    }
                    else
                    {
                        // assign the BATCH LOT NUMBER
                        this.LogisticUnitBatchLotNumber = tempData[..dataLength];
                    }
                }
                catch (Exception e)
                {
                    this.AddParseErrorDetail(
                        BarCodeExceptionReason.BatchLotNumber_InvalidFormat,
                        new BarCodeParseException(
                            $"Failed to resize string '{tempData}' to length {dataLength}.",
                            "Batch Lot Number",
                            e));
                }

                // remove the Batch / Lot Number from the bar code data
                tempData = tempData.Remove(0, aiIndex);
            }

            return tempData;
        }

        /// <summary>
        ///     Identify the single trade item serial number.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifySingleTradeItemSerialNumber(string barCodeData)
        {
            var tempData = barCodeData;

            // check length of 2 only because AI data is variable
            if (tempData.StartsWith("21") && tempData.Length >= 2)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // variable AI look for group separator
                var aiIndex = this.FindGroupSeparatorInBarCode(tempData);
                var dataLength = 0;

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

                    if (this.Gtin.Contains("95060000093134") &&
                        this.Settings.IsRuleEnabled_ParseEAN_UCC128_BourvyExports_ProductionDate)
                    {
                        // Parse and assign the production date from the serial number data.

                        // year
                        var year = Convert.ToInt32(tempData[..2]);
                        if (year < 80)
                        {
                            year += 2000;
                        }
                        else
                        {
                            year += 1900;
                        }

                        var dt = new DateTime(year, 1, 1);

                        // Julian days from and including Jan 01 of the year.
                        // Assumption is that the string length will always be three digits. 0 filled???
                        var days = Convert.ToInt32(tempData.Substring(2, 3));

                        // Subtract one day, because the calculated year date begins at day 1.
                        this.LogisticUnitProductionDate = dt.AddDays(days - 1);
                    }
                    else
                    {
                        // assign the SERIAL NUMBER
                        this.SingleTradeItemSerialNumber = tempData[..dataLength];
                    }
                }
                catch (Exception e)
                {
                    this.AddParseErrorDetail(
                        BarCodeExceptionReason.SingleTradeItemSerialNumber_InvalidFormat,
                        new BarCodeParseException(
                            $"Failed to resize string '{tempData}' to length {dataLength}.",
                            "Serial Number",
                            e));
                }

                // remove the Batch / Lot Number from the bar code data
                tempData = tempData.Remove(0, aiIndex);
            }

            return tempData;
        }

        /// <summary>
        ///     Identify the logistic unit production date.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifyLogisticUnitProductionDate(string barCodeData)
        {
            var tempData = barCodeData;
            if (tempData.StartsWith("11") && tempData.Length >= 8)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                try
                {
                    // assign the PRODUCTION DATE
                    var year = Convert.ToInt32(tempData[..2]);
                    if (year < 80)
                    {
                        year += 2000;
                    }
                    else
                    {
                        year += 1900;
                    }

                    var dt = new DateTime(
                        year,
                        Convert.ToInt32(tempData.Substring(2, 2)),
                        Convert.ToInt32(tempData.Substring(4, 2))); // 101026

                    this.LogisticUnitProductionDate = dt;
                }
                catch (Exception e)
                {
                    this.AddParseErrorDetail(
                        BarCodeExceptionReason.ProductionDate_InvalidDate,
                        new BarCodeParseException(
                            $"Cannot convert {barCodeData.Substring(2, 6)} to a valid YYMMDD DateTime.",
                            "Production Date",
                            e));
                }

                // remove the PRODUCTION DATE from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }

        /// <summary>
        ///     Identify the logistic unit due date.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifyLogisticUnitDueDate(string barCodeData)
        {
            var tempData = barCodeData;
            if (tempData.StartsWith("12") && tempData.Length >= 8)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                try
                {
                    // assign the DUE DATE
                    var year = Convert.ToInt32(tempData[..2]);
                    if (year < 80)
                    {
                        year += 2000;
                    }
                    else
                    {
                        year += 1900;
                    }

                    var dt = new DateTime(
                        year,
                        Convert.ToInt32(tempData.Substring(2, 2)),
                        Convert.ToInt32(tempData.Substring(4, 2))); // 101026
                    this.LogisticUnitDueDate = dt;
                }
                catch (Exception e)
                {
                    this.AddParseErrorDetail(
                        BarCodeExceptionReason.DueDate_InvalidDate,
                        new BarCodeParseException(
                            $"Cannot convert {barCodeData.Substring(2, 6)} to a valid YYMMDD DateTime.",
                            "Due Date",
                            e));
                }

                // remove the DUE DATE from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }

        /// <summary>
        ///     Identify the logistic unit packaging date.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifyLogisticUnitPackagingDate(string barCodeData)
        {
            var tempData = barCodeData;
            if (tempData.StartsWith("13") && tempData.Length >= 8)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);
                try
                {
                    // assign the PACKAGING DATE
                    var year = Convert.ToInt32(tempData[..2]);
                    if (year < 80)
                    {
                        year += 2000;
                    }
                    else
                    {
                        year += 1900;
                    }

                    var dt = new DateTime(
                        year,
                        Convert.ToInt32(tempData.Substring(2, 2)),
                        Convert.ToInt32(tempData.Substring(4, 2))); // 101026

                    if (this.Settings
                        .IsRuleEnabled_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate)
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
                    if (this.Settings
                        .IsRuleEnabled_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate)
                    {
                        this.AddParseErrorDetail(
                            BarCodeExceptionReason.PackagingDateAsProductionDate_InvalidDate,
                            new BarCodeParseException(
                                string.Format(
                                    "Cannot convert {0} to a valid YYMMDD DateTime." +
                                    Environment.NewLine +
                                    "Parse 'Packaging Date' as 'Production Date' enabled.",
                                    barCodeData.Substring(2, 6)),
                                "Production Date",
                                e));
                    }
                    else
                    {
                        this.AddParseErrorDetail(
                            BarCodeExceptionReason.PackagingDate_InvalidDate,
                            new BarCodeParseException(
                                $"Cannot convert {barCodeData.Substring(2, 6)} to a valid YYMMDD DateTime.",
                                "Packaging Date",
                                e));
                    }
                }

                // remove the PACKAGING DATE from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }

        /// <summary>
        ///     Identify the logistic unit best before sell by date.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifyLogisticUnitBestBeforeSellByDate(string barCodeData)
        {
            var tempData = barCodeData;
            if (tempData.StartsWith("15") && tempData.Length >= 8)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                try
                {
                    // assign the BEST BEFORE SELL BY DATE
                    var year = Convert.ToInt32(tempData[..2]);
                    if (year < 80)
                    {
                        year += 2000;
                    }
                    else
                    {
                        year += 1900;
                    }

                    var dt = new DateTime(
                        year,
                        Convert.ToInt32(tempData.Substring(2, 2)),
                        Convert.ToInt32(tempData.Substring(4, 2))); // 101026

                    this.LogisticUnitBestBeforeSellByDate = dt;
                }
                catch (Exception e)
                {
                    this.AddParseErrorDetail(
                        BarCodeExceptionReason.BestBeforeSellByDate_InvalidDate,
                        new BarCodeParseException(
                            $"Cannot convert {barCodeData.Substring(2, 6)} to a valid YYMMDD DateTime.",
                            "Best Before Sell By Date",
                            e));
                }

                // remove the BEST BEFORE SELL BY DATE from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }

        /// <summary>
        ///     Identify the logistic unit expiry use by date.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifyLogisticUnitExpiryUseByDate(string barCodeData)
        {
            var tempData = barCodeData;
            if (tempData.StartsWith("17") && tempData.Length >= 8)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                try
                {
                    // assign the EXPIRY USE BY SELL BY DATE
                    var year = Convert.ToInt32(tempData[..2]);
                    if (year < 80)
                    {
                        year += 2000;
                    }
                    else
                    {
                        year += 1900;
                    }

                    var dt = new DateTime(
                        year,
                        Convert.ToInt32(tempData.Substring(2, 2)),
                        Convert.ToInt32(tempData.Substring(4, 2))); // 101026

                    if (this.Settings
                        .IsRuleEnabled_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate)
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
                            $"Cannot convert {barCodeData.Substring(2, 6)} to a valid YYMMDD DateTime.",
                            "Expiry Use By Sell By Date",
                            e));
                }

                // remove the EXPIRY USE BY DATE from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }

        /// <summary>
        ///     Identify the logistic unit product variant.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifyLogisticUnitProductVariant(string barCodeData)
        {
            var tempData = barCodeData;
            if (tempData.StartsWith("20") && tempData.Length >= 4)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);
                try
                {
                    // assign the PRODUCT VARIANT
                    this.LogisticUnitProductVariant = Convert.ToInt32(tempData[..2]);
                }
                catch (Exception e)
                {
                    this.AddParseErrorDetail(
                        BarCodeExceptionReason.ProductVariant_InvalidInteger,
                        new BarCodeParseException(
                            $"Cannot convert {barCodeData.Substring(2, 2)} to an integer.",
                            "Product Variant",
                            e));
                }

                // remove the PRODUCT VARIANT from the bar code data
                tempData = tempData.Remove(0, 2);
            }

            return tempData;
        }

        /// <summary>
        ///     Identify the logistic unit count of trade items.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifyLogisticUnitCountOfTradeItems(string barCodeData)
        {
            var tempData = barCodeData;
            if (tempData.StartsWith("37") && tempData.Length >= 10)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);
                try
                {
                    // assign the Count of Trade Items
                    this.LogisticUnitCountOfTradeItems = Convert.ToInt32(tempData[..8]);
                }
                catch (Exception e)
                {
                    this.AddParseErrorDetail(
                        BarCodeExceptionReason.CountOfTradeItems_InvalidInteger,
                        new BarCodeParseException(
                            $"Cannot convert {barCodeData.Substring(2, 8)} to an integer.",
                            "Count of Trade Items",
                            e));
                }

                // remove the COUNT OF TRADE ITEMS from the bar code data
                tempData = tempData.Remove(0, 8);
            }

            return tempData;
        }

        /// <summary>
        ///     Identify the logistic unit variable count.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifyLogisticUnitVariableCount(string barCodeData)
        {
            var codeData = barCodeData;

            // Check the entire length (this.Data) not the barCodeData.length because it could have been parsed and altered.
            if (this.Settings.IsRuleEnabled_Parse_Code128_China_Seafood &&
                barCodeData.StartsWith("30") &&
                this.Length == 14)
            {
                // do not parse the AI
            }
            else
            {
                if (codeData.StartsWith("30") &&
                    codeData.Length >= 3)
                {
                    var parseIt = true;

                    if (this.Settings.IsRuleEnabled_Parse_Code_128_AI01_BC_AI30_exclusive)
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
                        var aiIndex = this.FindGroupSeparatorInBarCode(codeData);
                        var dataLength = 0;

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
                            this.LogisticUnitVariableCount = Convert.ToInt32(codeData[..dataLength]);
                        }
                        catch (Exception e)
                        {
                            this.AddParseErrorDetail(
                                BarCodeExceptionReason.VariableCount_InvalidInteger,
                                new BarCodeParseException(
                                    $"Cannot convert {codeData[..dataLength]} to an integer.",
                                    "Variable Count",
                                    e));
                        }

                        // remove the Variable Count from the bar code data
                        codeData = codeData.Remove(0, aiIndex);
                    }
                }
            }

            return codeData;
        }

        /// <summary>
        ///     Identify the location identifier.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifyLocationIdentifier(string barCodeData)
        {
            var tempData = barCodeData;

            // <revision date="20160928" />
            // Used to parse 414 && length >= 16
            // Changing to not care about length because some barcodes have smaller lengths due to legacy bugs.
            if (tempData.StartsWith("414"))
            {
                // remove the AI
                tempData = tempData.Remove(0, 3);

                var locationLength = Math.Min(13, tempData.Length);

                // assign the LOCATION IDENTIFIER
                // allowing alpha is not UCC/EAN compliant. But we need to support
                this.LocationIdentifier = tempData[..locationLength];

                // remove the LOCATION IDENTIFIER from the bar code data
                tempData = tempData.Remove(0, locationLength);

                this.IdentifyCompanyPrefix(this.LocationIdentifier);
            }

            return tempData;
        }

        /// <summary>
        ///     Identify the net weight metric kg.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifyNetWeightMetricKg(string barCodeData)
        {
            var tempData = barCodeData;
            if (tempData.StartsWith("310") && tempData.Length >= 10)
            {
                this.NetWeightMetricKgFoundAi = true;

                // get the number of decimal places
                var placesFound = false;
                try
                {
                    var places = Convert.ToInt32(tempData.Substring(3, 1));
                    placesFound = true;

                    // remove the AI
                    tempData = tempData.Remove(0, 4);

                    // assign the METRIC NET WEIGHT KG
                    var strWholeNumber = tempData[..6];
                    if (places != 0)
                    {
                        var s = strWholeNumber[..^places] + "." + strWholeNumber[^places..];
                        this.NetWeightMetricKg = Convert.ToDecimal(s);
                    }
                    else
                    {
                        this.NetWeightMetricKg = Convert.ToDecimal(strWholeNumber);
                    }
                }
                catch (Exception e)
                {
                    if (placesFound)
                    {
                        this.AddParseErrorDetail(
                            BarCodeExceptionReason.NetWeightMetricKG_InvalidDecimal,
                            new BarCodeParseException(
                                $"Failed to parse '{barCodeData}'",
                                "Metric Net Weight KG",
                                e));
                    }
                    else
                    {
                        this.AddParseErrorDetail(
                            BarCodeExceptionReason.NetWeightMetricKG_UnknownDecimalPlaces,
                            new BarCodeParseException(
                                $"Failed to parse '{barCodeData}'. Could not determine number of decimal places.",
                                "Metric Net Weight KG",
                                e));
                    }
                }

                // remove the METRIC NET WEIGHT KG from the bar code data
                tempData = barCodeData[10..];
            }

            return tempData;
        }

        /// <summary>
        ///     Identify the net weight non-metric lb.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifyNetWeightNonMetricLb(string barCodeData)
        {
            var tempData = barCodeData;
            if (tempData.StartsWith("320") && tempData.Length >= 10)
            {
                this.NetWeightNonMetricLbFoundAi = true;
                var placesFound = false;
                try
                {
                    // get the number of decimal places
                    var places = Convert.ToInt32(tempData.Substring(3, 1));
                    placesFound = true;

                    // remove the AI
                    tempData = tempData.Remove(0, 4);

                    // assign the NON-METRIC NET WEIGHT LB
                    var strWholeNumber = tempData[..6];
                    if (places != 0)
                    {
                        var s = strWholeNumber[..^places] + "." + strWholeNumber[^places..];
                        this.NetWeightNonMetricLb = Convert.ToDecimal(s);
                    }
                    else
                    {
                        this.NetWeightNonMetricLb = Convert.ToDecimal(strWholeNumber);
                    }
                }
                catch (Exception e)
                {
                    if (placesFound)
                    {
                        this.AddParseErrorDetail(
                            BarCodeExceptionReason.NetWeightNonMetricLB_InvalidDecimal,
                            new BarCodeParseException(
                                $"Failed to parse '{barCodeData}'.",
                                "NON-METRIC Net Weight LB",
                                e));
                    }
                    else
                    {
                        this.AddParseErrorDetail(
                            BarCodeExceptionReason.NetWeightNonMetricLB_UnknownDecimalPlaces,
                            new BarCodeParseException(
                                $"Failed to parse '{barCodeData}'. Could not determine number of" +
                                    " decimal places.",
                                "NON-METRIC Net Weight LB",
                                e));
                    }
                }

                // remove the NON-METRIC NET WEIGHT LB from the bar code data
                tempData = barCodeData[10..];
            }

            return tempData;
        }

        /// <summary>
        ///     Identify the gross weight metric kg.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifyGrossWeightMetricKg(string barCodeData)
        {
            var tempData = barCodeData;
            if (tempData.StartsWith("330") && tempData.Length >= 10)
            {
                this.GrossWeightMetricKgFoundAi = true;
                var placesFound = false;
                try
                {
                    // get the number of decimal places
                    var places = Convert.ToInt32(tempData.Substring(3, 1));
                    placesFound = true;

                    // remove the AI
                    tempData = tempData.Remove(0, 4);

                    // assign the NON-METRIC GROSS WEIGHT LB
                    var strWholeNumber = tempData[..6];
                    if (places != 0)
                    {
                        var s = strWholeNumber[..^places] + "." + strWholeNumber[^places..];
                        this.GrossWeightMetricKg = Convert.ToDecimal(s);
                    }
                    else
                    {
                        this.GrossWeightMetricKg = Convert.ToDecimal(strWholeNumber);
                    }
                }
                catch (Exception e)
                {
                    if (placesFound)
                    {
                        this.AddParseErrorDetail(
                            BarCodeExceptionReason.GrossWeightMetricKG_InvalidDecimal,
                            new BarCodeParseException(
                                $"Failed to parse '{barCodeData}'",
                                "Metric Gross Weight KG",
                                e));
                    }
                    else
                    {
                        this.AddParseErrorDetail(
                            BarCodeExceptionReason.GrossWeightMetricKG_UnknownDecimalPlaces,
                            new BarCodeParseException(
                                $"Failed to parse '{barCodeData}'. Could not determine number of decimal places.",
                                "Metric Gross Weight KG",
                                e));
                    }
                }

                // remove the NON-METRIC WEIGHT LB from the bar code data

                // Change to substring of original to remove what was consumed.
                // This works even if an exception was thrown prior which would've removed the leading 4
                tempData = barCodeData[10..];
            }

            return tempData;
        }

        /// <summary>
        ///     Identify the gross weight non-metric lb.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifyGrossWeightNonMetricLb(string barCodeData)
        {
            var tempData = barCodeData;
            if (tempData.StartsWith("340") && tempData.Length >= 10)
            {
                this.GrossWeightNonMetricLbFoundAi = true;
                var placesFound = false;
                try
                {
                    // get the number of decimal places
                    var places = Convert.ToInt32(tempData.Substring(3, 1));
                    placesFound = true;

                    // remove the AI
                    tempData = tempData.Remove(0, 4);

                    // assign the NON-METRIC GROSS WEIGHT LB
                    var strWholeNumber = tempData[..6];
                    if (places != 0)
                    {
                        var s = strWholeNumber[..^places] + "." + strWholeNumber[^places..];
                        this.GrossWeightNonMetricLb = Convert.ToDecimal(s);
                    }
                    else
                    {
                        this.GrossWeightNonMetricLb = Convert.ToDecimal(strWholeNumber);
                    }
                }
                catch (Exception e)
                {
                    if (placesFound)
                    {
                        this.AddParseErrorDetail(
                            BarCodeExceptionReason.GrossWeightNonMetricLB_InvalidDecimal,
                            new BarCodeParseException(
                                $"Failed to parse '{barCodeData}'",
                                "NON-METRIC Gross Weight LB",
                                e));
                    }
                    else
                    {
                        this.AddParseErrorDetail(
                            BarCodeExceptionReason.GrossWeightNonMetricLB_UnknownDecimalPlaces,
                            new BarCodeParseException(
                                $"Failed to parse '{barCodeData}'. Could not determine number of decimal places.",
                                "NON-METRIC Gross Weight LB",
                                e));
                    }
                }

                // remove the NON-METRIC GROSS WEIGHT LB from the bar code data
                tempData = barCodeData[10..];
            }

            return tempData;
        }

        /// <summary>
        ///     Identify the length or 1st dimension meters.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifyLengthOr1StDimensionMeters(string barCodeData)
        {
            var tempData = barCodeData;
            if (tempData.StartsWith("311") && tempData.Length >= 10)
            {
                var placesFound = false;
                try
                {
                    // get the number of decimal places
                    var places = Convert.ToInt32(tempData.Substring(3, 1));
                    placesFound = true;

                    // remove the AI
                    tempData = tempData.Remove(0, 4);

                    // assign the LENGTH IN METERS
                    var strWholeNumber = tempData[..6];
                    if (places != 0)
                    {
                        var s = strWholeNumber[..^places] + "." + strWholeNumber[^places..];
                        this.LengthOr1StDimensionMeters = Convert.ToDecimal(s);
                    }
                    else
                    {
                        this.LengthOr1StDimensionMeters = Convert.ToDecimal(strWholeNumber);
                    }
                }
                catch (Exception e)
                {
                    if (placesFound)
                    {
                        this.AddParseErrorDetail(
                            BarCodeExceptionReason.DimensionMeters_InvalidDecimal,
                            new BarCodeParseException(
                                $"Failed to parse '{barCodeData}'",
                                "Length in Meters",
                                e));
                    }
                    else
                    {
                        this.AddParseErrorDetail(
                            BarCodeExceptionReason.DimensionMeters_UnknownDecimalPlaces,
                            new BarCodeParseException(
                                $"Failed to parse '{barCodeData}'. Could not determine number of decimal places.",
                                "Length in Meters",
                                e));
                    }
                }

                // remove the LENGTH IN METERS from the bar code data
                tempData = barCodeData[10..];
            }

            return tempData;
        }

        /// <summary>
        ///     Identify the activity FK Refno.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifyActivityFkRefno(string barCodeData)
        {
            var tempData = barCodeData;

            // check length of 2 only because AI data is variable
            if (tempData.StartsWith("92") && tempData.Length >= 2)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // variable AI look for group separator
                var aiIndex = this.FindGroupSeparatorInBarCode(tempData);
                var dataLength = 0;

                try
                {
                    if (aiIndex > -1)
                    {
                        aiIndex++;
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
                    this.ActFkRefno = tempData[..dataLength];
                }
                catch (Exception e)
                {
                    this.AddParseErrorDetail(
                        BarCodeExceptionReason.ActivityFKRefno_InvalidFormat,
                        new BarCodeParseException(
                            $"Failed to parse '{barCodeData}'",
                            "Activity FK Refno",
                            e));
                }

                // remove the Activity FK Refno data from the bar code data
                tempData = tempData.Remove(0, aiIndex);
            }

            return tempData;
        }

        /// <summary>
        ///     Identify the activity item FK Refno.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifyActivityItemFkRefno(string barCodeData)
        {
            var tempData = barCodeData;

            // check length of 2 only because AI data is variable
            if (tempData.StartsWith("91") && tempData.Length >= 2)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // variable AI look for group separator
                var aiIndex = this.FindGroupSeparatorInBarCode(tempData);
                var dataLength = 0;

                try
                {
                    if (aiIndex > -1)
                    {
                        aiIndex++;
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
                    this.ActItemFkRefno = tempData[..dataLength];
                }
                catch (Exception e)
                {
                    this.AddParseErrorDetail(
                        BarCodeExceptionReason.ActivityItemFKRefno_InvalidFormat,
                        new BarCodeParseException(
                            $"Failed to parse '{barCodeData}'",
                            "Activity Item FK Refno",
                            e));
                }

                // remove the Activity Item FK Refno data from the bar code data
                tempData = tempData.Remove(0, aiIndex);
            }

            return tempData;
        }

        /// <summary>
        ///     Identify the activity detail FK Refno.
        /// </summary>
        /// <param name="barCodeData">Barcode data to find the company prefix in.</param>
        /// <returns>Modified barcode data.</returns>
        protected override string IdentifyActivityDetailFkRefno(string barCodeData)
        {
            var tempData = barCodeData;

            // check length of 2 only because AI data is variable
            if (tempData.StartsWith("93") && tempData.Length >= 2)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // variable AI look for group separator
                var aiIndex = this.FindGroupSeparatorInBarCode(tempData);
                var dataLength = 0;

                try
                {
                    if (aiIndex > -1)
                    {
                        aiIndex++;
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
                    this.ActDetailFkRefno = tempData[..dataLength];
                }
                catch (Exception e)
                {
                    this.AddParseErrorDetail(
                        BarCodeExceptionReason.ActivityDetailFKRefno_InvalidFormat,
                        new BarCodeParseException(
                            $"Failed to parse '{barCodeData}'",
                            "Activity Detail FK Refno",
                            e));
                }

                // remove the Activity Detail FK Refno data from the bar code data
                tempData = tempData.Remove(0, aiIndex);
            }

            return tempData;
        }
    }
}