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
        /// Also the production date was dd/mm/yy instead of yy/mm/dd, changed all dates.</remarks>
        public override void ParseBarCodeData(string barCodeData)
        {
            var parse = barCodeData;
            try
            {
                string compare;
                do
                {
                    compare = parse;
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
            var tempData = barCodeData;

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
        /// <inheritdoc/>
        protected override string IdentifyCompanyPrefix(string barCodeData)
        {
            var tempData = barCodeData;

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
        /// <inheritdoc/>
        protected override string IdentifyGTIN(string barCodeData)
        {
            var tempData = barCodeData;

            // Rule change GTIN is only 12 digits
            if (tempData.StartsWith("01") && tempData.Length >= 14)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // assign the GTIN
                this.GTIN = tempData.Substring(0, 12);

                // remove the GTIN from the bar code data
                tempData = tempData.Remove(0, 12);

                this.IdentifyCompanyPrefix(this.GTIN);
            }

            return tempData;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="barCodeData"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        protected override string IdentifyGTINinLogisticUnit(string barCodeData)
        {
            var tempData = barCodeData;
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
        /// <inheritdoc/>
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

                if (this.Settings.IsRuleEnabled_Parse_LanzMeats_Code_128_AI10_as_AI21 && this.GTIN.Contains("99419701"))
                {
                    this.SingleTradeItemSerialNumber = tempData.Substring(0, dataLength);
                }
                else
                {
                    // assign the BATCH LOT NUMBER
                    this.LogisticUnitBatchLotNumber = tempData.Substring(0, dataLength);
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
                this.SingleTradeItemSerialNumber = tempData.Substring(0, dataLength);

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
            var tempData = barCodeData;

            if (tempData.StartsWith("11") && tempData.Length >= 8)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // assign the PRODUCTION DATE
                var day = Convert.ToInt32(tempData.Substring(0, 2));
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
            var tempData = barCodeData;
            if (tempData.StartsWith("12") && tempData.Length >= 8)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // assign the DUE DATE
                var day = Convert.ToInt32(tempData.Substring(0, 2));
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
            var tempData = barCodeData;
            if (tempData.StartsWith("13") && tempData.Length >= 8)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // assign the PACKAGING DATE
                var day = Convert.ToInt32(tempData.Substring(0, 2));
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
            var tempData = barCodeData;
            if (tempData.StartsWith("15") && tempData.Length >= 8)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // assign the BEST BEFORE SELL BY DATE
                var day = Convert.ToInt32(tempData.Substring(0, 2));
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
            var tempData = barCodeData;
            if (tempData.StartsWith("17") && tempData.Length >= 8)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // assign the EXPIRY USE BY SELL BY DATE
                var day = Convert.ToInt32(tempData.Substring(0, 2));
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
            var tempData = barCodeData;
            if (tempData.StartsWith("20") && tempData.Length >= 4)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // assign the PRODUCT VARIANT
                this.LogisticUnitProductVariant = Convert.ToInt32(tempData.Substring(0, 2));

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
            var tempData = barCodeData;
            if (tempData.StartsWith("37") && tempData.Length >= 10)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // assign the Count of Trade Items
                this.LogisticUnitCountOfTradeItems = Convert.ToInt32(
                    tempData.Substring(0, 8));

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
                        this.LogisticUnitVariableCount = Convert.ToInt32(codeData.Substring(0, dataLength));

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
        protected override string IdentifyNetWeightMetricKG(string barCodeData)
        {
            var tempData = barCodeData;

            if (tempData.StartsWith("310") && tempData.Length >= 10)
            {
                this.NetWeightMetricKGFoundAI = true;
                // get the number of decimal places
                var places = Convert.ToInt32(tempData.Substring(3, 1));

                // remove the AI
                tempData = tempData.Remove(0, 4);

                // assign the METRIC NET WEIGHT KG
                var strWholeNumber = tempData.Substring(0, 6);
                if (places != 0)
                {
                    var s = strWholeNumber.Substring(0, strWholeNumber.Length - places) + "."
                        + strWholeNumber.Substring(strWholeNumber.Length - places);
                    this.NetWeightMetricKG = Convert.ToDecimal(s);
                }
                else
                {
                    this.NetWeightMetricKG = Convert.ToDecimal(strWholeNumber);
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
        protected override string IdentifyNetWeightNonMetricLB(string barCodeData)
        {
            var tempData = barCodeData;

            if (tempData.StartsWith("320") && tempData.Length >= 10)
            {
                this.NetWeightNonMetricLBFoundAI = true;
                // get the number of decimal places
                var places = Convert.ToInt32(tempData.Substring(3, 1));

                // remove the AI
                tempData = tempData.Remove(0, 4);

                // assign the NON METRIC NET WEIGHT LB
                var strWholeNumber = tempData.Substring(0, 6);
                if (places != 0)
                {
                    var s = strWholeNumber.Substring(0, strWholeNumber.Length - places) + "."
                        + strWholeNumber.Substring(strWholeNumber.Length - places);
                    this.NetWeightNonMetricLB = Convert.ToDecimal(s);
                }
                else
                {
                    this.NetWeightNonMetricLB = Convert.ToDecimal(strWholeNumber);
                }

                // remove the NON METRIC NET WEIGHT LB from the bar code data
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
        protected override string IdentifyGrossWeightMetricKG(string barCodeData)
        {
            var tempData = barCodeData;

            if (tempData.StartsWith("330") && tempData.Length >= 10)
            {
                this.GrossWeightMetricKGFoundAI = true;
                // get the number of decimal places
                var places = Convert.ToInt32(tempData.Substring(3, 1));

                // remove the AI
                tempData = tempData.Remove(0, 4);

                // assign the NON METRIC GROSS WEIGHT LB
                var strWholeNumber = tempData.Substring(0, 6);
                if (places != 0)
                {
                    var s = strWholeNumber.Substring(0, strWholeNumber.Length - places) + "."
                        + strWholeNumber.Substring(strWholeNumber.Length - places);
                    this.GrossWeightMetricKG = Convert.ToDecimal(s);
                }
                else
                {
                    this.GrossWeightMetricKG = Convert.ToDecimal(strWholeNumber);
                }

                // remove the NON METRIC WEIGHT LB from the bar code data
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
        protected override string IdentifyGrossWeightNonMetricLB(string barCodeData)
        {
            var tempData = barCodeData;
            if (tempData.StartsWith("340") && tempData.Length >= 10)
            {
                this.GrossWeightNonMetricLBFoundAI = true;
                // get the number of decimal places
                var places = Convert.ToInt32(tempData.Substring(3, 1));

                // remove the AI
                tempData = tempData.Remove(0, 4);

                // assign the NON METRIC GROSS WEIGHT LB
                var strWholeNumber = tempData.Substring(0, 6);
                if (places != 0)
                {
                    var s = strWholeNumber.Substring(0, strWholeNumber.Length - places) + "."
                        + strWholeNumber.Substring(strWholeNumber.Length - places);
                    this.GrossWeightNonMetricLB = Convert.ToDecimal(s);
                }
                else
                {
                    this.GrossWeightNonMetricLB = Convert.ToDecimal(strWholeNumber);
                }

                // remove the NON METRIC GROSS WEIGHT LB from the bar code data
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
        protected override string IdentifyLengthOr1stDimensionMeters(string barCodeData)
        {
            var tempData = barCodeData;
            if (tempData.StartsWith("311") && tempData.Length >= 10)
            {
                // get the number of decimal places
                var places = Convert.ToInt32(tempData.Substring(3, 1));

                // remove the AI
                tempData = tempData.Remove(0, 4);

                // assign the LENGTH IN METERS
                var strWholeNumber = tempData.Substring(0, 6);
                if (places != 0)
                {
                    var s = strWholeNumber.Substring(0, strWholeNumber.Length - places) + "."
                        + strWholeNumber.Substring(strWholeNumber.Length - places);
                    this.LengthOr1stDimensionMeters = Convert.ToDecimal(s);
                }
                else
                {
                    this.LengthOr1stDimensionMeters = Convert.ToDecimal(strWholeNumber);
                }

                // remove the LENGTH IN METERS from the bar code data
                tempData = tempData.Remove(0, 6);
            }

            return tempData;
        }
    }
}
