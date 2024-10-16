// <copyright file="Product.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types.Products
{
    /// <summary>
    /// Represents a product.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Gets or sets the reference number of the product.
        /// </summary>
        public int ProductRefno { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string ProductDescription { get; set; }

        /// <summary>
        /// Gets or sets the code of the product.
        /// </summary>
        public string ProductCode { get; set; }
    }
}