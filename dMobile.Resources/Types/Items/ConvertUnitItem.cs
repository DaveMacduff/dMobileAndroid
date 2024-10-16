// <copyright file="ConvertUnitItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Types.Items
{
    using System;

    /// <summary>
    /// Represents an item for converting units.
    /// </summary>
    public class ConvertUnitItem
    {
        /// <summary>
        /// Gets or sets the reference number of the product unit.
        /// </summary>
        public int ProductUnitRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product.
        /// </summary>
        public int ProductRefno { get; set; }

        /// <summary>
        /// Gets or sets the scheme variant number of the product.
        /// </summary>
        public int ProductSchemeVariantNumber { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the activity detail.
        /// </summary>
        public int ActivityDetailFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the activity detail.
        /// </summary>
        public int ActivityDetailRefno { get; set; }

        /// <summary>
        /// Gets or sets the code of the product.
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the barcode of the product unit.
        /// </summary>
        public string ProductUnitBC { get; set; }

        /// <summary>
        /// Gets or sets the code of the unit.
        /// </summary>
        public string UnitCode { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string ProductDescription { get; set; }

        /// <summary>
        /// Gets or sets the size description of the product unit.
        /// </summary>
        public string ProductUnitSizeDescription { get; set; }

        /// <summary>
        /// Gets or sets the scheme ID of the activity detail.
        /// </summary>
        public string ActivityDetailSchemeID { get; set; }

        /// <summary>
        /// Gets or sets the unique scheme ID of the activity detail.
        /// </summary>
        public string ActivityDetailUniqueSchemeID { get; set; }

        /// <summary>
        /// Gets or sets the single trade item serial number of the activity detail.
        /// </summary>
        public string ActivityDetailSingleTradeItemSerialNumber { get; set; }

        /// <summary>
        /// Gets or sets the batch/lot number of the logistic unit in the activity detail.
        /// </summary>
        public string ActivityDetailLogisticUnitBatchLotNumber { get; set; }

        /// <summary>
        /// Gets or sets the scheme variant code of the product.
        /// </summary>
        public string ProductSchemeVariantCode { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the activity detail.
        /// </summary>
        public decimal ActivityDetailQuantity { get; set; }

        /// <summary>
        /// Gets or sets the base quantity of the activity detail.
        /// </summary>
        public decimal ActivityDetailBaseQuantity { get; set; }

        /// <summary>
        /// Gets or sets the weight quantity of the activity detail.
        /// </summary>
        public decimal ActivityDetailWeightQuantity { get; set; }

        /// <summary>
        /// Gets or sets the best before/sell by date of the logistic unit in the activity detail.
        /// </summary>
        public DateTime ActivityDetailLogisticUnitBestBeforeSellByDate { get; set; }

        /// <summary>
        /// Gets or sets the production date of the logistic unit in the activity detail.
        /// </summary>
        public DateTime ActivityDetailLogisticUnitProductionDate { get; set; }
    }
}