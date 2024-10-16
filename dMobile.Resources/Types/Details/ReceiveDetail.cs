// <copyright file="ReceiveDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types.Details
{
    /// <summary>
    /// Define the receive detail model.
    /// </summary>
    public class ReceiveDetail : ActivityDetail
    {
        /// <summary>
        /// Gets or sets the status to track if, changed, inserted, updated.
        /// </summary>
        public int ActivityDetailStatus { get; set; } = 0;
    }
}
