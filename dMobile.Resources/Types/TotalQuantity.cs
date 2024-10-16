// <copyright file="TotalQuantity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    /// <summary>
    /// Represents the total quantity of items ordered and received.
    /// </summary>
    public class TotalQuantity
    {
        /// <summary>
        /// Gets or sets the total quantity of items ordered.
        /// </summary>
        public decimal Ordered { get; set; }

        /// <summary>
        /// Gets or sets the total quantity of items received.
        /// </summary>
        public decimal Received { get; set; }
    }
}