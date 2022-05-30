// <copyright file="SFillQuantity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    /// <summary>
    /// Structure to hold quantity.
    /// </summary>
    /// <remarks>Used in FillQuantityStructureFromQuantity.</remarks>
    public struct SFillQuantity
    {

        /// <summary>
        /// Gets or sets the alt quantity.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets the base quantity.
        /// </summary>
        public decimal BaseQuantity { get; set; }

        /// <summary>
        /// Gets or sets the weight quantity.
        /// </summary>
        public decimal WeightQuantity { get; set; }
    }
}
