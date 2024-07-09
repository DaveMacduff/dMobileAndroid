// <copyright file="BarCodeParseCode_128_BrokerMultiNationalFromChili.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using System;

    public class BarCodeParseCode_128_BrokerMultiNationalFromChili : BarCodeParse
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
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
                while (string.Compare(parse, compare) != 0);
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
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
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
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
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
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
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
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
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
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
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
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
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
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
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
                    year = year + 2000;
                }
                else
                {
                    year = year + 1900;
                }

                var dt = new DateTime(year, month, day);
                this.LogisticUnitProductionDate = dt;

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
        /// <inheritdoc/>
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
                    year = year + 2000;
                }
                else
                {
                    year = year + 1900;
                }

                var dt = new DateTime(year, month, day);
                this.LogisticUnitDueDate = dt;

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
        /// <inheritdoc/>
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
                    year = year + 2000;
                }
                else
                {
                    year = year + 1900;
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
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
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
                    year = year + 2000;
                }
                else
                {
                    year = year + 1900;
                }

                var dt = new DateTime(year, month, day);

                this.LogisticUnitBestBeforeSellByDate = dt;

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
        /// <inheritdoc/>
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
                    year = year + 2000;
                }
                else
                {
                    year = year + 1900;
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
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
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
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
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
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
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
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        protected override string IdentifyNetWeightMetricKg(string barCodeData)
        {
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
            var tempData = barCodeData;

            if (tempData.StartsWith("310") && tempData.Length >= 10)
            {
                this.NetWeightMetricKgFoundAi = true;

                // get the number of decimal places
                var places = Convert.ToInt32(tempData.Substring(3, 1));

                // remove the AI
                tempData = tempData.Remove(0, 4);

                // assign the METRIC NET WEIGHT KG
                var strWholeNumber = tempData[..6];
                if (places != 0)
                {
                    var s = strWholeNumber[..^places] + "."
                                                      + strWholeNumber[^places..];
                    this.NetWeightMetricKg = Convert.ToDecimal(s);
                }
                else
                {
                    this.NetWeightMetricKg = Convert.ToDecimal(strWholeNumber);
                }

                // remove the METRIC NET WEIGHT KG from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        protected override string IdentifyNetWeightNonMetricLb(string barCodeData)
        {
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
            var tempData = barCodeData;

            if (tempData.StartsWith("320") && tempData.Length >= 10)
            {
                this.NetWeightNonMetricLbFoundAi = true;

                // get the number of decimal places
                var places = Convert.ToInt32(tempData.Substring(3, 1));

                // remove the AI
                tempData = tempData.Remove(0, 4);

                // assign the NON-METRIC NET WEIGHT LB
                var strWholeNumber = tempData[..6];
                if (places != 0)
                {
                    var s = strWholeNumber[..^places] + "."
                                                      + strWholeNumber[^places..];
                    this.NetWeightNonMetricLb = Convert.ToDecimal(s);
                }
                else
                {
                    this.NetWeightNonMetricLb = Convert.ToDecimal(strWholeNumber);
                }

                // remove the NON-METRIC NET WEIGHT LB from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        protected override string IdentifyGrossWeightMetricKg(string barCodeData)
        {
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
            var tempData = barCodeData;

            if (tempData.StartsWith("330") && tempData.Length >= 10)
            {
                this.GrossWeightMetricKgFoundAi = true;

                // get the number of decimal places
                var places = Convert.ToInt32(tempData.Substring(3, 1));

                // remove the AI
                tempData = tempData.Remove(0, 4);

                // assign the NON-METRIC GROSS WEIGHT LB
                var strWholeNumber = tempData[..6];
                if (places != 0)
                {
                    var s = strWholeNumber[..^places] + "."
                                                      + strWholeNumber[^places..];
                    this.GrossWeightMetricKg = Convert.ToDecimal(s);
                }
                else
                {
                    this.GrossWeightMetricKg = Convert.ToDecimal(strWholeNumber);
                }

                // remove the NON-METRIC WEIGHT LB from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        protected override string IdentifyGrossWeightNonMetricLb(string barCodeData)
        {
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
            var tempData = barCodeData;
            if (tempData.StartsWith("340") && tempData.Length >= 10)
            {
                this.GrossWeightNonMetricLbFoundAi = true;

                // get the number of decimal places
                var places = Convert.ToInt32(tempData.Substring(3, 1));

                // remove the AI
                tempData = tempData.Remove(0, 4);

                // assign the NON-METRIC GROSS WEIGHT LB
                var strWholeNumber = tempData[..6];
                if (places != 0)
                {
                    var s = strWholeNumber[..^places] + "."
                                                      + strWholeNumber[^places..];
                    this.GrossWeightNonMetricLb = Convert.ToDecimal(s);
                }
                else
                {
                    this.GrossWeightNonMetricLb = Convert.ToDecimal(strWholeNumber);
                }

                // remove the NON-METRIC GROSS WEIGHT LB from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        protected override string IdentifyLengthOr1StDimensionMeters(string barCodeData)
        {
            barCodeData = barCodeData ?? throw new ArgumentNullException(nameof(barCodeData));
            var tempData = barCodeData;
            if (tempData.StartsWith("311") && tempData.Length >= 10)
            {
                // get the number of decimal places
                var places = Convert.ToInt32(tempData.Substring(3, 1));

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

                // remove the LENGTH IN METERS from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }
    }
}
