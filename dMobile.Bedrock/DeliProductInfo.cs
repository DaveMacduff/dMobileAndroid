// <copyright file="DeliProductInfo.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    /// <summary>
    /// Product information class containing some deli information.
    /// </summary>
    public class DeliProductInfo
    {
        /// <summary>
        /// Gets or sets the deli scale weight..
        /// </summary>
        public decimal DeliScaleWeight { get; set; } = 0;

        /// <summary>
        /// Gets or sets the deli scale weight unit.
        /// </summary>
        public string DeliScaleWeightUnit { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product information.
        /// </summary>
        public ProductInfo ProdInfo { get; set; } = new ProductInfo();
    }
}
