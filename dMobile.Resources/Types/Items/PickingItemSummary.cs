// <copyright file="PickingItemSummary.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types.Items
{
    /// <summary>
    /// Represents a summary of picking items.
    /// </summary>
    public class PickingItemSummary
    {
        /// <summary>
        /// Gets or sets the total quantity ordered.
        /// </summary>
        public decimal Ordered { get; set; }

        /// <summary>
        /// Gets or sets the total weight ordered.
        /// </summary>
        public decimal WeightOrdered { get; set; }

        /// <summary>
        /// Gets or sets the total quantity shipped.
        /// </summary>
        public decimal Shipped { get; set; }

        /// <summary>
        /// Gets or sets the total weight shipped.
        /// </summary>
        public decimal WeightShipped { get; set; }

        /// <summary>
        /// Gets or sets the weight unit code.
        /// </summary>
        public string WtUnitCode { get; set; }
    }
}