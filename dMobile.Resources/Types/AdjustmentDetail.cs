// <copyright file="AdjustmentDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    /// <summary>
    /// Define the adjustment detail model.
    /// </summary>
    public class AdjustmentDetail : ActivityDetail
    {
        /// <summary>
        /// Gets or sets the activity detail status.
        /// </summary>
        public int ActivityDetailStatus { get; set; } = 0;
    }
}
