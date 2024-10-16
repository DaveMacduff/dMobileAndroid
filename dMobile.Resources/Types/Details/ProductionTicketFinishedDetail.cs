// <copyright file="ProductionTicketFinishedDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types.Details
{
    /// <summary>
    /// Define the production ticket finished detail model.
    /// </summary>
    public class ProductionTicketFinishedDetail : ActivityDetail
    {
        /// <summary>
        /// Gets or sets the activity detail status.
        /// </summary>
        public int ActivityDetailStatus { get; set; } = 0;
    }
}
