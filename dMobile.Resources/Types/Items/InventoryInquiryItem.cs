// <copyright file="InventoryInquiryItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Types.Items
{
    using System;

    /// <summary>
    /// Represents an item for inventory inquiry.
    /// </summary>
    public class InventoryInquiryItem
    {
        /// <summary>
        /// Gets or sets the code of the product.
        /// </summary>
        public string Productcode { get; set; }

        /// <summary>
        /// Gets or sets the barcode of the product unit.
        /// </summary>
        public string Productunitbc { get; set; }

        /// <summary>
        /// Gets or sets the code of the unit.
        /// </summary>
        public string UnitCode { get; set; }

        /// <summary>
        /// Gets or sets the code of the weight unit.
        /// </summary>
        public string WeightUnitCode { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the activity item.
        /// </summary>
        public decimal ActivityItemQuantity { get; set; }

        /// <summary>
        /// Gets or sets the weight quantity of the activity item.
        /// </summary>
        public decimal ActivityItemWeightQuantity { get; set; }

        /// <summary>
        /// Gets or sets the production date of the logistic unit in the activity detail.
        /// </summary>
        public string ActivityDetailLogisticUnitProductionDate { get; set; }

        /// <summary>
        /// Gets or sets the best before/sell by date of the logistic unit in the activity detail.
        /// </summary>
        public string ActivityDetailLogisticUnitBestBeforeSellByDate { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the warehouse location map.
        /// </summary>
        public string LocationMapWarehouseFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the section location map.
        /// </summary>
        public string LocationMapSectionFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the slot location map.
        /// </summary>
        public string LocationMapSlotFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the code of the skid.
        /// </summary>
        public string SkidCode { get; set; }
    }
}