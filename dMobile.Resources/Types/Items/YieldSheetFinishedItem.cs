// <copyright file="YieldSheetFinishedItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Types.Items
{
    /// <summary>
    /// Represents a finished item in a yield sheet ticket.
    /// </summary>
    public class YieldSheetFinishedItem
    {
        /// <summary>
        /// Gets or sets the reference number of the activity item.
        /// </summary>
        public int ActivityItemRefno { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the activity item.
        /// </summary>
        public int ActivityItemFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product unit.
        /// </summary>
        public int ProductUnitRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product.
        /// </summary>
        public int ProductRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the location map.
        /// </summary>
        public int LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the code of the location.
        /// </summary>
        public int LocationCode { get; set; }

        /// <summary>
        /// Gets or sets the status of the activity item.
        /// </summary>
        public int ActivityItemStatus { get; set; }

        /// <summary>
        /// Gets or sets the yield sheet quantity of the activity item.
        /// </summary>
        public decimal ActivityItemYieldSheetQuantity { get; set; }

        /// <summary>
        /// Gets or sets the base yield sheet quantity of the activity item.
        /// </summary>
        public decimal ActivityItemYieldSheetBaseQuantity { get; set; }

        /// <summary>
        /// Gets or sets the weight yield sheet quantity of the activity item.
        /// </summary>
        public decimal ActivityItemYieldSheetWeightQuantity { get; set; }

        /// <summary>
        /// Gets or sets the expected yield sheet quantity of the activity item.
        /// </summary>
        public decimal ActivityItemYieldSheetQuantityExpected { get; set; }

        /// <summary>
        /// Gets or sets the expected base yield sheet quantity of the activity item.
        /// </summary>
        public decimal ActivityItemYieldSheetBaseQuantityExpected { get; set; }

        /// <summary>
        /// Gets or sets the expected weight yield sheet quantity of the activity item.
        /// </summary>
        public decimal ActivityItemYieldSheetWeightQuantityExpected { get; set; }

        /// <summary>
        /// Gets or sets the string foreign key reference number of the activity item.
        /// </summary>
        public string ActivityItemStringFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the barcode of the product unit.
        /// </summary>
        public string ProductUnitBC { get; set; }

        /// <summary>
        /// Gets or sets the size description of the product unit.
        /// </summary>
        public string ProductUnitSizeDescription { get; set; }

        /// <summary>
        /// Gets or sets the code of the product.
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string ProductDescription { get; set; }

        /// <summary>
        /// Gets or sets the code of the base unit.
        /// </summary>
        public string BaseUnitCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the location.
        /// </summary>
        public string LocationName { get; set; }
    }
}