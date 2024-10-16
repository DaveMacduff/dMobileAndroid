// <copyright file="ActivityItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Types.Items
{
    /// <summary>
    /// Defines the <see cref="ActivityItem" /> Activity item model.
    /// </summary>
    public class ActivityItem
    {
        /// <summary>
        /// Gets or sets the product code.
        /// </summary>
        public string ProductCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product unit bc character.
        /// </summary>
        public string ProductUnitBC { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product unit code.
        /// </summary>
        public string UnitCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the weight unit code.
        /// </summary>
        public string WeightUnitCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity detail logistic unit production date.
        /// </summary>
        public string ActivityDetailLogisticUnitProductionDate { get; set; }

        /// <summary>
        /// Gets or sets the activity detail logistic unit best before sell by date.
        /// </summary>
        public string ActivityDetailLogisticUnitBestBeforeSellByDate { get; set; }

        /// <summary>
        /// Gets or sets the location map warehouse foreign key reference number.
        /// </summary>
        public string LocationMapWarehouseFKRefno { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the location map section foreign key reference number.
        /// </summary>
        public string LocationMapSectionFKRefno { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the location map slot foreign key reference number.
        /// </summary>
        public string LocationMapSlotFKRefno { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the skid code.
        /// </summary>
        public string SkidCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity item quantity.
        /// </summary>
        public decimal ActivityItemQuantity { get; set; }

        /// <summary>
        /// Gets or sets the activity item weight quantity.
        /// </summary>
        public decimal ActivityItemWeightQuantity { get; set; }
    }
}
