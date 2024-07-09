// <copyright file="BarCodeParseUCC_EAN_128_Symbol_NossackProduct94600_ChileanBackRibs.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.BarCodeScanner.BarCodeParsing
{
    using dMobile.Bedrock;
    using System;

    /// <summary>
    /// Define bar code parsing for UCC/EAN-128 symbology for Nossack Product 94600 Chilean Back Ribs.
    /// </summary>
    public class BarCodeParseUCC_EAN_128_Symbol_NossackProduct94600_ChileanBackRibs : BarCodeParseUCC_EAN_128_Symbol
    {
        /// <summary>
        /// The ParseBarCodeData.
        /// </summary>
        /// <param name="barCodeData">The barCodeData<see cref="string"/>.</param>
        public override void ParseBarCodeData(string barCodeData)
        {
            try
            {
                var parse = barCodeData;
                string compare;

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
                this.SuppressErrorsRule();
            }
            catch (BarCodeParseException)
            {
                throw;
            }
            catch (Exception e)
            {
                // General Barcode parsing failure.
                throw new BarCodeParseException(
                    $"Failed to parse '{barCodeData}'.",
                    e);
            }
            finally
            {
                this.CleanupRules();
            }
        }

        /// <summary>
        /// The IdentifyGTIN.
        /// </summary>
        /// <param name="barCodeData">.</param>
        /// <returns>.</returns>
        protected override string IdentifyGtin(string barCodeData)
        {
            var tempData = barCodeData;

            if (tempData.StartsWith("01") && tempData.Length >= 14)
            {
                // remove the AI
                tempData = tempData.Remove(0, 2);

                // Barcode AI01 only for this rule has a length 12 for the GTIN.
                // <WO ID="14938" Comment="Assume they fixed their 12 digit to a 14 digit GTIN" />
                // assign the GTIN
                this.Gtin = tempData[..14];// .PadLeft(14, '0');

                // remove the GTIN from the bar code data
                tempData = tempData.Remove(0, 14);

                this.IdentifyCompanyPrefix(this.Gtin);
            }

            return tempData;
        }

        /// <summary>
        /// The IdentifyProductIdentifier.
        /// </summary>
        /// <param name="barCodeData">.</param>
        /// <returns>.</returns>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            var tempData = barCodeData;

            if (!string.IsNullOrEmpty(this.Gtin))
            {
                this.ProductIdentifier = this.Gtin;
            }
            else
            {
                if (tempData.Length > 12)
                {
                    this.ProductIdentifier = tempData[..12].PadLeft(14, '0');
                }
                else
                {
                    this.ProductIdentifier = tempData;
                }
            }

            return tempData;
        }

        /// <summary>
        /// They want to read DDMMYY instead of the standard YYMMDD.
        /// </summary>
        /// <param name="barCodeData">.</param>
        /// <returns>.</returns>
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
                    // <issue="13609" comment="DDMMYY"/>
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

                    var dt = new DateTime(
                        year,
                        month,
                        day);   // 101026

                    this.LogisticUnitProductionDate = dt;
                }
                catch (Exception e)
                {
                    this.AddParseErrorDetail(
                        BarCodeExceptionReason.ProductionDate_InvalidDate,
                        new BarCodeParseException(
                            $"Cannot convert {barCodeData.Substring(2, 6)} to a valid DDMMYY DateTime.",
#if MULTILANGUAGE
                        Translator.GetInstance().Translate(TranslatorVocabulary.ProductionDatezz), e));
#else
                        "Production Date", e));
#endif
                }
                finally
                {
                    // remove the PRODUCTION DATE from the bar code data
                    tempData = tempData.Remove(0, 6);
                }
            }

            return tempData;
        }
    }
}
