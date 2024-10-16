// <copyright file="PurchaseOrderWorkSheetItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Types.Items
{
    /// <summary>
    /// Represents an item in a purchase order worksheet.
    /// </summary>
    public class PurchaseOrderWorkSheetItem
    {
        /// <summary>
        /// Gets or sets the reference number of the activity item.
        /// </summary>
        public int ActivityItemRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the activity.
        /// </summary>
        public int ActivityRefno { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the activity item.
        /// </summary>
        public int ActivityItemFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the string foreign key reference number of the activity item.
        /// </summary>
        public string ActivityItemStringFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product unit.
        /// </summary>
        public int ProductUnitRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product.
        /// </summary>
        public int ProductRefno { get; set; }

        /// <summary>
        /// Gets or sets the code of the product.
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string ProductDescription { get; set; }

        /// <summary>
        /// Gets or sets the unit code.
        /// </summary>
        public string UnitCode { get; set; }

        /// <summary>
        /// Gets or sets the size description of the product unit.
        /// </summary>
        public string ProductUnitSizeDescription { get; set; }

        /// <summary>
        /// Gets or sets the quantity ordered in the purchase order worksheet.
        /// </summary>
        public decimal ActivityItemPurchaseOrderWorkSheetQuantityOrdered { get; set; }

        /// <summary>
        /// Gets or sets the base quantity ordered in the purchase order worksheet.
        /// </summary>
        public decimal ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered { get; set; }

        /// <summary>
        /// Gets or sets the weight quantity ordered in the purchase order worksheet.
        /// </summary>
        public decimal ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the location map.
        /// </summary>
        public int LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the code of the location.
        /// </summary>
        public int LocationCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the location.
        /// </summary>
        public string LocationName { get; set; }

        /// <summary>
        /// Gets or sets the status of the activity item.
        /// </summary>
        public int ActivityItemStatus { get; set; }
    }
}