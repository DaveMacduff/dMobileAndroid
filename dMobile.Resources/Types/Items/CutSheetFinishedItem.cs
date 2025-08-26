// <copyright file="CutSheetFinishedItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Types.Items
{
    /// <summary>
    /// Represents a finished item in a cut sheet ticket.
    /// </summary>
    public class CutSheetFinishedItem
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
        /// Gets or sets the cut sheet quantity of the activity item.
        /// </summary>
        public decimal ActivityItemCutSheetQuantity { get; set; }

        /// <summary>
        /// Gets or sets the base cut sheet quantity of the activity item.
        /// </summary>
        public decimal ActivityItemCutSheetBaseQuantity { get; set; }

        /// <summary>
        /// Gets or sets the weight cut sheet quantity of the activity item.
        /// </summary>
        public decimal ActivityItemCutSheetWeightQuantity { get; set; }

        /// <summary>
        /// Gets or sets the expected cut sheet quantity of the activity item.
        /// </summary>
        public decimal ActivityItemCutSheetQuantityExpected { get; set; }

        /// <summary>
        /// Gets or sets the expected base cut sheet quantity of the activity item.
        /// </summary>
        public decimal ActivityItemCutSheetBaseQuantityExpected { get; set; }

        /// <summary>
        /// Gets or sets the expected weight cut sheet quantity of the activity item.
        /// </summary>
        public decimal ActivityItemCutSheetWeightQuantityExpected { get; set; }

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