// <copyright file="ProductLocationMapType.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types.Products
{
    /// <summary>
    /// Represents the mapping of a product to a location type.
    /// </summary>
    public class ProductLocationMapType
    {
        /// <summary>
        /// Gets or sets the code of the product.
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the type of the location map.
        /// </summary>
        public string LocationMapType { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string ProductDescription { get; set; }

        /// <summary>
        /// Gets or sets the brand description of the product.
        /// </summary>
        public string ProductBrandDescription { get; set; }
    }
}