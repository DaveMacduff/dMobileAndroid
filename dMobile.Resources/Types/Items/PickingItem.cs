// <copyright file="PickingItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Types.Items
{
    /// <summary>
    /// Represents an item for picking in inventory management.
    /// </summary>
    public class PickingItem
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
        /// Gets or sets the brand description of the product.
        /// </summary>
        public string ProductBrandDescription { get; set; }

        /// <summary>
        /// Gets or sets the code of the unit.
        /// </summary>
        public string UnitCode { get; set; }

        /// <summary>
        /// Gets or sets the barcode of the product unit.
        /// </summary>
        public string ProductUnitBC { get; set; }

        /// <summary>
        /// Gets or sets the size description of the product unit.
        /// </summary>
        public string ProductUnitSizeDescription { get; set; }

        /// <summary>
        /// Gets or sets the ordered picking quantity of the activity item.
        /// </summary>
        public decimal ActivityItemPickingQuantityOrdered { get; set; }

        /// <summary>
        /// Gets or sets the ordered base quantity of the activity item.
        /// </summary>
        public decimal ActivityItemPickingBaseQuantityOrdered { get; set; }

        /// <summary>
        /// Gets or sets the ordered weight quantity of the activity item.
        /// </summary>
        public decimal ActivityItemPickingWeightQuantityOrdered { get; set; }

        /// <summary>
        /// Gets or sets the shipped picking quantity of the activity item.
        /// </summary>
        public decimal ActivityItemPickingQuantityShipped { get; set; }

        /// <summary>
        /// Gets or sets the shipped base quantity of the activity item.
        /// </summary>
        public decimal ActivityItemPickingBaseQuantityShipped { get; set; }

        /// <summary>
        /// Gets or sets the shipped weight quantity of the activity item.
        /// </summary>
        public decimal ActivityItemPickingWeightQuantityShipped { get; set; }

        /// <summary>
        /// Gets or sets the customer product unit code for the picking activity item.
        /// </summary>
        public string ActivityItemPickingCustomerProductUnitCode { get; set; }

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
        /// Gets or sets the available picking quantity of the activity item.
        /// </summary>
        public decimal ActivityItemPickingQuantityAvailable { get; set; }

        /// <summary>
        /// Gets or sets the available base quantity of the activity item.
        /// </summary>
        public decimal ActivityItemPickingBaseQuantityAvailable { get; set; }

        /// <summary>
        /// Gets or sets the available weight quantity of the activity item.
        /// </summary>
        public decimal ActivityItemPickingWeightQuantityAvailable { get; set; }

        /// <summary>
        /// Gets or sets the comment for the picking activity item.
        /// </summary>
        public string IsActivityItemPickingComment { get; set; }

        /// <summary>
        /// Gets or sets the status of the activity item.
        /// </summary>
        public int ActivityItemStatus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the picking activity item is reviewed.
        /// </summary>
        public bool ActivityItemPickingIsReviewed { get; set; }
    }
}