// <copyright file="ProductVariant.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types.Products
{
    /// <summary>
    /// Represents a variant of a product scheme.
    /// </summary>
    public class ProductVariant
    {
        /// <summary>
        /// Gets or sets the code of the product scheme variant.
        /// </summary>
        public string ProductSchemeVariantCode { get; set; }

        /// <summary>
        /// Gets or sets the number of the product scheme variant.
        /// </summary>
        public int ProductSchemeVariantNo { get; set; }

        /// <summary>
        /// Gets or sets the display name of the product scheme variant.
        /// </summary>
        public string ProductSchemeVariantDisplay { get; set; }
    }
}