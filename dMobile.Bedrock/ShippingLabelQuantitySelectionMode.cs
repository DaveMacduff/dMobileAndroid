// <copyright file="ShippingLabelQuantitySelectionMode.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    /// <summary>
    /// Defines the ShippingLabelQuantitySelectionMode.
    /// </summary>
    public enum ShippingLabelQuantitySelectionMode : int
    {
        /// <summary>
        /// Defines the BothFullRepack.
        /// </summary>
        BothFullRepack = 0,

        /// <summary>
        /// Defines the FullOnly.
        /// </summary>
        FullOnly = 1,

        /// <summary>
        /// Defines the RepackOnly.
        /// </summary>
        RepackOnly = 2,
    }
}
