// <copyright file="ConvertProductItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Types.Items
{
    using System;

    /// <summary>
    /// Represents an item for converting products.
    /// </summary>
    public class ConvertProductItem
    {
        /// <summary>
        /// Gets or sets the foreign key reference number of the product to convert from.
        /// </summary>
        public int FromProdFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the product to convert to.
        /// </summary>
        public int ToProdFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the location map.
        /// </summary>
        public int LocMapRefno { get; set; }
    }
}