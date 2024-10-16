// <copyright file="ProductionTicketByProductItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Types.Items
{
    /// <summary>
    /// Represents a production ticket item by product.
    /// </summary>
    public class ProductionTicketByProductItem
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
        /// Gets or sets the production quantity of the activity item.
        /// </summary>
        public decimal ActivityItemProductionQuantity { get; set; }

        /// <summary>
        /// Gets or sets the base production quantity of the activity item.
        /// </summary>
        public decimal ActivityItemProductionBaseQuantity { get; set; }

        /// <summary>
        /// Gets or sets the weight production quantity of the activity item.
        /// </summary>
        public decimal ActivityItemProductionWeightQuantity { get; set; }

        /// <summary>
        /// Gets or sets the expected production quantity of the activity item.
        /// </summary>
        public decimal ActivityItemProductionQuantityExpected { get; set; }

        /// <summary>
        /// Gets or sets the expected base production quantity of the activity item.
        /// </summary>
        public decimal ActivityItemProductionBaseQuantityExpected { get; set; }

        /// <summary>
        /// Gets or sets the expected weight production quantity of the activity item.
        /// </summary>
        public decimal ActivityItemProductionWeightQuantityExpected { get; set; }

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