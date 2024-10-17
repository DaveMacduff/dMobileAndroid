// <copyright file="BarCodeParseCode_128_BrokerMultiNationalFromChili.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    /// <summary>
    /// Represents a parser for Code 128 barcodes specific to Broker MultiNational from Chili.
    /// </summary>
    public class BarCodeParseCode_128_BrokerMultiNationalFromChili : BarCodeParse
    {
        /// <summary>
        /// Parses the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <remarks>Bar code GTIN was only 12 digits not 14.
        /// Also, the production date was dd/mm/yy instead of yy/mm/dd, changed all dates.</remarks>
        public override void ParseBarCodeData(string barCodeData)
        {
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
            var parse = barCodeData;
            try
            {
                string compare;
                do
                {
                    compare = parse;
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
                }
                while (string.CompareOrdinal(parse, compare) != 0);
            }
            catch (Exception)
            {
                // suppress handler, CANNOT PARSE THE UNPARSABLE
                // perhaps learning Mandarin would be easier.
                // string appDir = Path.GetDirectoryName(
                //    Assembly.GetExecutingAssembly().GetName().CodeBase);
                //// elevator music
                // string fullPathToMyFile = Path.Combine(appDir, "dMobileResource01.wav");
                // SysSound snd = new SysSound(fullPathToMyFile);
                // snd.Play();
            }

            this.IdentifyProductIdentifier(barCodeData);
            this.Remainder = parse;
        }

        /// <summary>
        /// Identifies the product identifier from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the product identifier.</returns>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
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
                if (tempData.Length > 14)
                {
                    this.ProductIdentifier = tempData[..14];
                }
                else
                {
                    this.ProductIdentifier = tempData;
                }
            }

            return tempData;
        }

        /// <summary>
        /// Identifies the company prefix from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the company prefix.</returns>
        protected override string IdentifyCompanyPrefix(string barCodeData)
        {
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
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
        /// Identifies the GTIN (Global Trade Item Number) from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the GTIN.</returns>
        protected override string IdentifyGtin(string barCodeData)
        {
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
            var tempData = barCodeData;

            // Rule change GTIN is only 12 digits
            if (tempData.StartsWith("01") && tempData.Length >= 14)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // assign the GTIN
                this.Gtin = tempData[..12];

                // remove the GTIN from the bar code data
                tempData = tempData.Remove(0, 12);

                this.IdentifyCompanyPrefix(this.Gtin);
            }

            return tempData;
        }

        /// <summary>
        /// Identifies the GTIN in logistic unit from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the GTIN in logistic unit.</returns>
        protected override string IdentifyGtiNinLogisticUnit(string barCodeData)
        {
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
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
        /// Identifies the logistic unit batch/lot number from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the logistic unit batch/lot number.</returns>
        protected override string IdentifyLogisticUnitBatchLotNumber(string barCodeData)
        {
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
            var tempData = barCodeData;

            // check length of 2 only because AI data is variable
            if (tempData.StartsWith("10") && tempData.Length >= 2)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // variable AI look for group separator
                var aiIndex = this.FindGroupSeparatorInBarCode(tempData);
                int dataLength;
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

                if (this.Settings.IsRuleEnabled_Parse_LanzMeats_Code_128_AI10_as_AI21 && this.Gtin.Contains("99419701"))
                {
                    this.SingleTradeItemSerialNumber = tempData[..dataLength];
                }
                else
                {
                    // assign the BATCH LOT NUMBER
                    this.LogisticUnitBatchLotNumber = tempData[..dataLength];
                }

                // remove the Batch / Lot Number from the bar code data
                tempData = tempData.Remove(0, aiIndex);
            }

            return tempData;
        }

        /// <summary>
        /// Identifies the single trade item serial number from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the single trade item serial number.</returns>
        protected override string IdentifySingleTradeItemSerialNumber(string barCodeData)
        {
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
            var tempData = barCodeData;

            // check length of 2 only because AI data is variable
            if (tempData.StartsWith("21") && tempData.Length >= 2)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // variable AI look for group separator
                var aiIndex = this.FindGroupSeparatorInBarCode(tempData);
                int dataLength;
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
                this.SingleTradeItemSerialNumber = tempData[..dataLength];

                // remove the Batch / Lot Number from the bar code data
                tempData = tempData.Remove(0, aiIndex);
            }

            return tempData;
        }

        /// <summary>
        /// Identifies the logistic unit production date from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the logistic unit production date.</returns>
        protected override string IdentifyLogisticUnitProductionDate(string barCodeData)
        {
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
            var tempData = barCodeData;

            if (tempData.StartsWith("11") && tempData.Length >= 8)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // assign the PRODUCTION DATE
                var day = Convert.ToInt32(tempData[..2]);
                var month = Convert.ToInt32(tempData.Substring(2, 2));
                var year = Convert.ToInt32(tempData.Substring(4, 2));
                if (year < 80)
                {
                    year += 2000;
                }
                else
                {
                    year += 1900;
                }

                var dt = new DateTime(year, month, day);
                this.LogisticUnitProductionDate = dt;

                // remove the PRODUCTION DATE from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }

        /// <summary>
        /// Identifies the logistic unit due date from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the logistic unit due date.</returns>
        protected override string IdentifyLogisticUnitDueDate(string barCodeData)
        {
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
            var tempData = barCodeData;
            if (tempData.StartsWith("12") && tempData.Length >= 8)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // assign the DUE DATE
                var day = Convert.ToInt32(tempData[..2]);
                var month = Convert.ToInt32(tempData.Substring(2, 2));
                var year = Convert.ToInt32(tempData.Substring(4, 2));
                if (year < 80)
                {
                    year += 2000;
                }
                else
                {
                    year += 1900;
                }

                var dt = new DateTime(year, month, day);
                this.LogisticUnitDueDate = dt;

                // remove the DUE DATE from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }

        /// <summary>
        /// Identifies the logistic unit packaging date from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the logistic unit packaging date.</returns>
        protected override string IdentifyLogisticUnitPackagingDate(string barCodeData)
        {
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
            var tempData = barCodeData;
            if (tempData.StartsWith("13") && tempData.Length >= 8)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // assign the PACKAGING DATE
                var day = Convert.ToInt32(tempData[..2]);
                var month = Convert.ToInt32(tempData.Substring(2, 2));
                var year = Convert.ToInt32(tempData.Substring(4, 2));
                if (year < 80)
                {
                    year += 2000;
                }
                else
                {
                    year += 1900;
                }

                var dt = new DateTime(year, month, day);

                if (this.Settings.IsRuleEnabled_Recognize_Code_128_AI13_PackagingDate_As_AI11_ProductionDate)
                {
                    this.LogisticUnitProductionDate = dt;
                }
                else
                {
                    this.LogisticUnitPackagingDate = dt;
                }

                // remove the PACKAGING DATE from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }

        /// <summary>
        /// Identifies the logistic unit best before/sell by date from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the logistic unit best before/sell by date.</returns>
        protected override string IdentifyLogisticUnitBestBeforeSellByDate(string barCodeData)
        {
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
            var tempData = barCodeData;
            if (tempData.StartsWith("15") && tempData.Length >= 8)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // assign the BEST BEFORE SELL BY DATE
                var day = Convert.ToInt32(tempData[..2]);
                var month = Convert.ToInt32(tempData.Substring(2, 2));
                var year = Convert.ToInt32(tempData.Substring(4, 2));
                if (year < 80)
                {
                    year += 2000;
                }
                else
                {
                    year += 1900;
                }

                var dt = new DateTime(year, month, day);

                this.LogisticUnitBestBeforeSellByDate = dt;

                // remove the BEST BEFORE SELL BY DATE from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }

        /// <summary>
        /// Identifies the logistic unit expiry/use by date from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the logistic unit expiry/use by date.</returns>
        protected override string IdentifyLogisticUnitExpiryUseByDate(string barCodeData)
        {
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
            var tempData = barCodeData;
            if (tempData.StartsWith("17") && tempData.Length >= 8)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // assign the EXPIRY USE BY SELL BY DATE
                var day = Convert.ToInt32(tempData[..2]);
                var month = Convert.ToInt32(tempData.Substring(2, 2));
                var year = Convert.ToInt32(tempData.Substring(4, 2));
                if (year < 80)
                {
                    year += 2000;
                }
                else
                {
                    year += 1900;
                }

                var dt = new DateTime(year, month, day);

                if (this.Settings.IsRuleEnabled_Recognize_Code_128_AI17_ExpiryDate_As_AI15_BestBeforeDate)
                {
                    this.LogisticUnitBestBeforeSellByDate = dt;
                }
                else
                {
                    this.LogisticUnitExpiryUseByDate = dt;
                }

                // remove the EXPIRY USE BY DATE from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }

        /// <summary>
        /// Identifies the logistic unit product variant from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the logistic unit product variant.</returns>
        protected override string IdentifyLogisticUnitProductVariant(string barCodeData)
        {
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
            var tempData = barCodeData;
            if (tempData.StartsWith("20") && tempData.Length >= 4)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // assign the PRODUCT VARIANT
                this.LogisticUnitProductVariant = Convert.ToInt32(tempData[..2]);

                // remove the PRODUCT VARIANT from the bar code data
                tempData = tempData.Remove(0, 2);
            }

            return tempData;
        }

        /// <summary>
        /// Identifies the logistic unit count of trade items from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the logistic unit count of trade items.</returns>
        protected override string IdentifyLogisticUnitCountOfTradeItems(string barCodeData)
        {
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
            var tempData = barCodeData;
            if (tempData.StartsWith("37") && tempData.Length >= 10)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // assign the Count of Trade Items
                this.LogisticUnitCountOfTradeItems = Convert.ToInt32(
                    tempData[..8]);

                // remove the COUNT OF TRADE ITEMS from the bar code data
                tempData = tempData.Remove(0, 8);
            }

            return tempData;
        }

        /// <summary>
        /// Identifies the logistic unit variable count from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the logistic unit variable count.</returns>
        protected override string IdentifyLogisticUnitVariableCount(string barCodeData)
        {
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
            var codeData = barCodeData;

            // Check the entire length (this.Data) not the barCodeData.length because it could have been parsed and altered.
            if (this.Settings.IsRuleEnabled_Parse_Code128_China_Seafood
                && barCodeData.StartsWith("30")
                && this.Length == 14)
            {
                // do not parse the AI
            }
            else
            {
                if (codeData.StartsWith("30") && codeData.Length >= 3)
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
                        int dataLength;
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

                        // remove the Variable Count from the bar code data
                        codeData = codeData.Remove(0, aiIndex);
                    }
                }
            }

            return codeData;
        }

        /// <summary>
        /// Identifies the net weight in metric kilograms from the bar code data.
        /// </summary>
        /// <param name="barCodeData">The bar code data to parse.</param>
        /// <returns>The remaining bar code data after extracting the net weight in metric kilograms.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="barCodeData"/> is null.</exception>
        /// <inheritdoc/>
        protected override string IdentifyNetWeightMetricKg(string barCodeData)
        {
            // Ensure barCodeData is not null
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
            var tempData = barCodeData;

            // Check if the bar code data starts with "310" and has a minimum length of 10
            if (tempData.StartsWith("310") && tempData.Length >= 10)
            {
                // Mark that the net weight in metric kilograms AI (Application Identifier) was found
                this.NetWeightMetricKgFoundAi = true;

                // Get the number of decimal places from the 4th character
                var places = Convert.ToInt32(tempData.Substring(3, 1));

                // Remove the AI (Application Identifier) from the bar code data
                tempData = tempData.Remove(0, 4);

                // Extract the net weight in metric kilograms as a whole number
                var strWholeNumber = tempData[..6];
                if (places != 0)
                {
                    // Insert the decimal point at the correct position
                    var s = strWholeNumber[..^places] + "." + strWholeNumber[^places..];
                    this.NetWeightMetricKg = Convert.ToDecimal(s);
                }
                else
                {
                    // Convert the whole number directly to decimal
                    this.NetWeightMetricKg = Convert.ToDecimal(strWholeNumber);
                }

                // Remove the net weight in metric kilograms from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            // Return the remaining bar code data
            return tempData;
        }

        /// <summary>
        /// Identifies the net weight in non-metric pounds from the bar code data.
        /// </summary>
        /// <param name="barCodeData">The bar code data to parse.</param>
        /// <returns>The remaining bar code data after extracting the net weight in non-metric pounds.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="barCodeData"/> is null.</exception>
        /// <inheritdoc/>
        protected override string IdentifyNetWeightNonMetricLb(string barCodeData)
        {
            // Ensure barCodeData is not null
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
            var tempData = barCodeData;

            // Check if the bar code data starts with "320" and has a minimum length of 10
            if (tempData.StartsWith("320") && tempData.Length >= 10)
            {
                // Mark that the net weight in non-metric pounds AI (Application Identifier) was found
                this.NetWeightNonMetricLbFoundAi = true;

                // Get the number of decimal places from the 4th character
                var places = Convert.ToInt32(tempData.Substring(3, 1));

                // Remove the AI (Application Identifier) from the bar code data
                tempData = tempData.Remove(0, 4);

                // Extract the net weight in non-metric pounds as a whole number
                var strWholeNumber = tempData[..6];
                if (places != 0)
                {
                    // Insert the decimal point at the correct position
                    var s = strWholeNumber[..^places] + "." + strWholeNumber[^places..];
                    this.NetWeightNonMetricLb = Convert.ToDecimal(s);
                }
                else
                {
                    // Convert the whole number directly to decimal
                    this.NetWeightNonMetricLb = Convert.ToDecimal(strWholeNumber);
                }

                // Remove the net weight in non-metric pounds from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            // Return the remaining bar code data
            return tempData;
        }

        /// <summary>
        /// Identifies the gross weight in metric kilograms from the bar code data.
        /// </summary>
        /// <param name="barCodeData">The bar code data to parse.</param>
        /// <returns>The remaining bar code data after extracting the gross weight in metric kilograms.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="barCodeData"/> is null.</exception>
        /// <inheritdoc/>
        protected override string IdentifyGrossWeightMetricKg(string barCodeData)
        {
            // Ensure barCodeData is not null
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
            var tempData = barCodeData;

            // Check if the bar code data starts with "330" and has a minimum length of 10
            if (tempData.StartsWith("330") && tempData.Length >= 10)
            {
                // Mark that the gross weight in metric kilograms AI (Application Identifier) was found
                this.GrossWeightMetricKgFoundAi = true;

                // Get the number of decimal places from the 4th character
                var places = Convert.ToInt32(tempData.Substring(3, 1));

                // Remove the AI (Application Identifier) from the bar code data
                tempData = tempData.Remove(0, 4);

                // Extract the gross weight in metric kilograms as a whole number
                var strWholeNumber = tempData[..6];
                if (places != 0)
                {
                    // Insert the decimal point at the correct position
                    var s = strWholeNumber[..^places] + "." + strWholeNumber[^places..];
                    this.GrossWeightMetricKg = Convert.ToDecimal(s);
                }
                else
                {
                    // Convert the whole number directly to decimal
                    this.GrossWeightMetricKg = Convert.ToDecimal(strWholeNumber);
                }

                // Remove the gross weight in metric kilograms from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            // Return the remaining bar code data
            return tempData;
        }

        /// <summary>
        /// Identifies the gross weight in non-metric pounds from the bar code data.
        /// </summary>
        /// <param name="barCodeData">The bar code data to parse.</param>
        /// <returns>The remaining bar code data after extracting the gross weight in non-metric pounds.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="barCodeData"/> is null.</exception>
        /// <inheritdoc/>
        protected override string IdentifyGrossWeightNonMetricLb(string barCodeData)
        {
            // Ensure barCodeData is not null
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
            var tempData = barCodeData;

            // Check if the bar code data starts with "340" and has a minimum length of 10
            if (tempData.StartsWith("340") && tempData.Length >= 10)
            {
                // Mark that the gross weight in non-metric pounds AI (Application Identifier) was found
                this.GrossWeightNonMetricLbFoundAi = true;

                // Get the number of decimal places from the 4th character
                var places = Convert.ToInt32(tempData.Substring(3, 1));

                // Remove the AI (Application Identifier) from the bar code data
                tempData = tempData.Remove(0, 4);

                // Extract the gross weight in non-metric pounds as a whole number
                var strWholeNumber = tempData[..6];
                if (places != 0)
                {
                    // Insert the decimal point at the correct position
                    var s = strWholeNumber[..^places] + "." + strWholeNumber[^places..];
                    this.GrossWeightNonMetricLb = Convert.ToDecimal(s);
                }
                else
                {
                    // Convert the whole number directly to decimal
                    this.GrossWeightNonMetricLb = Convert.ToDecimal(strWholeNumber);
                }

                // Remove the gross weight in non-metric pounds from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            // Return the remaining bar code data
            return tempData;
        }

        /// <summary>
        /// Identifies the length or first dimension in meters from the bar code data.
        /// </summary>
        /// <param name="barCodeData">The bar code data to parse.</param>
        /// <returns>The remaining bar code data after extracting the length or first dimension in meters.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="barCodeData"/> is null.</exception>
        /// <inheritdoc/>
        protected override string IdentifyLengthOr1StDimensionMeters(string barCodeData)
        {
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
            var tempData = barCodeData;
            if (tempData.StartsWith("311") && tempData.Length >= 10)
            {
                // Get the number of decimal places
                var places = Convert.ToInt32(tempData.Substring(3, 1));

                // Remove the AI
                tempData = tempData.Remove(0, 4);

                // Assign the LENGTH IN METERS
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

                // Remove the LENGTH IN METERS from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }
    }
}
