// <copyright file="CountDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    /// <summary>
    /// Define the count detail model.
    /// </summary>
    public class CountDetail : ActivityDetail
    {
        /// <summary>
        /// Gets or sets the product brand description.
        /// </summary>
        public string ProductBrandDescription { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity detail status.
        /// </summary>
        public int ActivityDetailStatus { get; set; } = 0;
    }
}
