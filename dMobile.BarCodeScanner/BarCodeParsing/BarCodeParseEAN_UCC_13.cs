// <copyright file="BarCodeParseEAN_UCC_13.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.BarCodeScanner.BarCodeParsing
{
    /// <summary>
    /// Provides parsing functionality for EAN/UCC-13 barcodes.
    /// </summary>
    public class BarCodeParseEAN_UCC_13 : BarCodeParse
    {
        /// <summary>
        /// Parses the barcode data according to the specific rules for EAN/UCC-13 barcodes.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Data structure is always 13 or 14 digits.
        /// Position 1 is an implied zero.
        /// Position 2-13 company prefix and item reference.
        /// Position 14 is a check digit.
        /// </para>
        /// <para>
        /// Because we do not know the length of either the company prefix or the item reference, they
        /// cannot be parsed.
        /// </para>
        /// </remarks>
        /// <param name="barCodeData">The barcode data to parse.</param>
        public override void ParseBarCodeData(string barCodeData)
        {
            if (this.Settings.IsRuleEnabled_Parse_UCC_13_as_UPC_12_UPC_E)
            {
                this.ClearResetBarCodeProperties(false);
                var barCodeParse = new BarCodeParseUCC_12_UPC_E();

                // Remove any leading extra zero padding, max bar code length will be 12.
                if (barCodeData.Length > 12)
                {
                    barCodeData = barCodeData[^12..];
                }

                barCodeParse.ParseBarCodeData(barCodeData);
                this.Gtin = barCodeParse.Gtin;
                this.ProductIdentifier = barCodeParse.ProductIdentifier;
                this.Remainder = barCodeParse.Remainder;
                this.ActDetailFkRefno = barCodeParse.ActDetailFkRefno;
                this.DeliScaleAmount = barCodeParse.DeliScaleAmount;
                this.CompanyPrefix = barCodeParse.CompanyPrefix;
                this.ItemReference = barCodeParse.ItemReference;
                this.ActFkRefno = barCodeParse.ActFkRefno;
                this.ActItemFkRefno = barCodeParse.ActItemFkRefno;
                this.LogisticUnitCountOfTradeItems = barCodeParse.LogisticUnitCountOfTradeItems;
                this.LocationIdentifier = barCodeParse.LocationIdentifier;
                this.LogisticUnitBatchLotNumber = barCodeParse.LogisticUnitBatchLotNumber;
                this.LogisticUnitProductionDate = barCodeParse.LogisticUnitProductionDate;
                this.LogisticUnitBestBeforeSellByDate = barCodeParse.LogisticUnitBestBeforeSellByDate;
                this.LogisticUnitExpiryUseByDate = barCodeParse.LogisticUnitExpiryUseByDate;
                this.PackagingType = barCodeParse.PackagingType;
                this.Sscc18 = barCodeParse.Sscc18;
                this.ItemReference = barCodeParse.ItemReference;
                this.GtinLogisticUnit = barCodeParse.GtinLogisticUnit;
                this.SingleTradeItemSerialNumber = barCodeParse.SingleTradeItemSerialNumber;
                this.LogisticUnitDueDate = barCodeParse.LogisticUnitDueDate;
                this.LogisticUnitPackagingDate = barCodeParse.LogisticUnitPackagingDate;
                this.LogisticUnitProductVariant = barCodeParse.LogisticUnitProductVariant;
                this.NetWeightMetricKg = barCodeParse.NetWeightMetricKg;
                this.NetWeightMetricKgFoundAi = barCodeParse.NetWeightMetricKgFoundAi;
                this.NetWeightNonMetricLb = barCodeParse.NetWeightNonMetricLb;
                this.NetWeightNonMetricLbFoundAi = barCodeParse.NetWeightNonMetricLbFoundAi;
                this.GrossWeightMetricKg = barCodeParse.GrossWeightMetricKg;
                this.GrossWeightMetricKgFoundAi = barCodeParse.GrossWeightMetricKgFoundAi;
                this.GrossWeightNonMetricLb = barCodeParse.GrossWeightNonMetricLb;
                this.GrossWeightNonMetricLbFoundAi = barCodeParse.GrossWeightNonMetricLbFoundAi;
                this.LogisticUnitVariableCount = barCodeParse.LogisticUnitVariableCount;
            }
            else
            {
                barCodeData = this.IdentifyGtin(barCodeData);     // does not remove data
                barCodeData = this.IdentifyProductIdentifier(barCodeData);   // does not remove data

                barCodeData = barCodeData.Remove(0, 1);   // remove implied zero

                barCodeData = this.IdentifyCompanyPrefix(barCodeData);
                barCodeData = this.IdentifyItemReference(barCodeData);

                if (barCodeData.Length > 0)
                {
                    barCodeData = barCodeData.Remove(0, 1);   // remove check digit
                }

                this.Remainder = barCodeData;
            }
        }

        /// <summary>
        /// Identifies the product identifier from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the product identifier.</returns>
        /// <inheritdoc/>
        protected override string IdentifyProductIdentifier(string barCodeData)
        {
            if (this.Settings.IsRuleEnabled_NoZeroPaddingOfProductIdentifierForUCC_EAN_Family)
            {
                this.ProductIdentifier = barCodeData;
            }
            else
            {
                this.ProductIdentifier = barCodeData.PadLeft(14, '0');
            }

            // remove the Product Identifier from the bar code data
            // barCodeData = barCodeData.Remove(0, strProductIdentifier.Length);
            return barCodeData;
        }

        /// <summary>
        /// Identifies the company prefix from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the company prefix.</returns>
        /// <inheritdoc/>
        protected override string IdentifyCompanyPrefix(string barCodeData)
        {
            this.CompanyPrefix = barCodeData[..5];

            // remove the Company Prefix from the bar code data
            barCodeData = barCodeData.Remove(0, this.CompanyPrefix.Length);

            return barCodeData;
        }

        /// <summary>
        /// Identifies the item reference from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the item reference.</returns>
        /// <inheritdoc/>
        protected override string IdentifyItemReference(string barCodeData)
        {
            this.ItemReference = barCodeData[..5];

            // remove the Item Reference from the bar code data
            barCodeData = barCodeData.Remove(0, this.ItemReference.Length);

            return barCodeData;
        }

        /// <summary>
        /// Identifies the GTIN (Global Trade Item Number) from the barcode data.
        /// </summary>
        /// <param name="barCodeData">The barcode data to parse.</param>
        /// <returns>The remaining barcode data after extracting the GTIN.</returns>
        /// <inheritdoc/>
        protected override string IdentifyGtin(string barCodeData)
        {
            var len = (barCodeData.Length > 14) ? 14 : barCodeData.Length;

            this.Gtin = barCodeData[..len].PadLeft(14, '0');

            // remove the GTIN from the bar code data
            // barCodeData = barCodeData.Remove(0, barCodeData.Length);
            return barCodeData;
        }
    }
}
