// <copyright file="ReceiveItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Types.Items
{
    /// <summary>
    /// Represents an item received in an activity.
    /// </summary>
    public class ReceiveItem
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
        /// Gets or sets the unit code.
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
        /// Gets or sets the quantity ordered for receiving.
        /// </summary>
        public decimal ActivityItemReceivingQuantityOrdered { get; set; }

        /// <summary>
        /// Gets or sets the base quantity ordered for receiving.
        /// </summary>
        public decimal ActivityItemReceivingBaseQuantityOrdered { get; set; }

        /// <summary>
        /// Gets or sets the weight quantity ordered for receiving.
        /// </summary>
        public decimal ActivityItemReceivingWeightQuantityOrdered { get; set; }

        /// <summary>
        /// Gets or sets the quantity received.
        /// </summary>
        public decimal ActivityItemReceivingQuantityReceived { get; set; }

        /// <summary>
        /// Gets or sets the base quantity received.
        /// </summary>
        public decimal ActivityItemReceivingBaseQuantityReceived { get; set; }

        /// <summary>
        /// Gets or sets the weight quantity received.
        /// </summary>
        public decimal ActivityItemReceivingWeightQuantityReceived { get; set; }

        /// <summary>
        /// Gets or sets the vendor product unit code for receiving.
        /// </summary>
        public string ActivityItemReceivingVendorProductUnitCode { get; set; }

        /// <summary>
        /// Gets or sets the description for receiving.
        /// </summary>
        public string ActivityItemReceivingDescription { get; set; }

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
        /// Gets or sets the quantity received to date.
        /// </summary>
        public decimal ActivityItemReceivingQuantityReceivedToDate { get; set; }

        /// <summary>
        /// Gets or sets the weight quantity received to date.
        /// </summary>
        public decimal ActivityItemReceivingWeightQuantityReceivedToDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether there is a comment for receiving.
        /// </summary>
        public string IsActivityItemReceivingComment { get; set; }
    }
}