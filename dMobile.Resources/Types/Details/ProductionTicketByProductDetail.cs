// <copyright file="ProductionTicketByProductDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types.Details
{
    /// <summary>
    /// Define the production ticket by product detail model.
    /// </summary>
    public class ProductionTicketByProductDetail : ActivityDetail
    {
        /// <summary>
        /// Gets or sets the activity detail status.
        /// </summary>
        public int ActivityDetailStatus { get; set; } = 0;
    }
}
