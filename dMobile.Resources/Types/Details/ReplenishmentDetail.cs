// <copyright file="ReplenishmentDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types.Details
{
    /// <summary>
    /// Define the replenishment activity detail model.
    /// </summary>
    public class ReplenishmentDetail : ActivityDetail
    {
        /// <summary>
        /// Gets or sets a value indicating whether the activity detail is confirmed.
        /// </summary>
        public bool ActivityDetailIsConfirmed { get; set; } = false;

        /// <summary>
        /// Gets or sets the activity detail is confirmed string.
        /// </summary>
        public string ActivityDetailIsConfirmedString { get; set; } = string.Empty;
    }
}
