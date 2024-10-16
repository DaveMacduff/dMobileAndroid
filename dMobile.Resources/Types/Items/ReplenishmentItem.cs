// <copyright file="ReplenishmentItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

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
        /// Gets or sets the activity item sub type.
        /// </summary>
        public int ActivityItemSubType { get; set; }

        /// <summary>
        /// Gets or sets the activity move type.
        /// </summary>
        public ReplenishmentMoveType.Code MoveType { get; set; }

        public int ActivityItemFKRefno { get; set; }

        public int ActivityItemParentFKRefno { get; set; }

        public int ProductUnitRefno { get; set; }

        public int LocationMapRefno { get; set; }

        public decimal ActivityItemReplenishmentQuantity { get; set; }

        public decimal ActivityItemReplenishmentBaseQuantity { get; set; }

        public decimal ActivityItemReplenishmentWeightQuantity { get; set; }
    }
}
