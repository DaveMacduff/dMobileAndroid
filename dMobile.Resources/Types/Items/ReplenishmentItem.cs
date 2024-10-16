// <copyright file="ReplenishmentItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Types.Items
{
    /// <summary>
    /// Replenishment Item type.
    /// </summary>
    public class ReplenishmentItem
    {
        /// <summary>
        /// Gets or sets the activity item reference number.
        /// </summary>
        public int ActivityItemRefno { get; set; }

        /// <summary>
        /// Gets or sets the activity item subtype.
        /// </summary>
        public int ActivityItemSubType { get; set; }

        /// <summary>
        /// Gets or sets the activity move type.
        /// </summary>
        public ReplenishmentMoveType.Code MoveType { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the activity item.
        /// </summary>
        public int ActivityItemFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the parent foreign key reference number of the activity item.
        /// </summary>
        public int ActivityItemParentFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the product unit.
        /// </summary>
        public int ProductUnitRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the location map.
        /// </summary>
        public int LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the replenishment quantity of the activity item.
        /// </summary>
        public decimal ActivityItemReplenishmentQuantity { get; set; }

        /// <summary>
        /// Gets or sets the base replenishment quantity of the activity item.
        /// </summary>
        public decimal ActivityItemReplenishmentBaseQuantity { get; set; }

        /// <summary>
        /// Gets or sets the weight replenishment quantity of the activity item.
        /// </summary>
        public decimal ActivityItemReplenishmentWeightQuantity { get; set; }
    }
}