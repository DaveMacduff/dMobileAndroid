// <copyright file="ReturnDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;

    /// <summary>
    /// Define the return detail model.
    /// </summary>
    public class ReturnDetail : ActivityDetail
    {
        /// <summary>
        /// Gets or sets the activity detail status.
        /// </summary>
        public int ActivityDetailStatus { get; set; } = 0;

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
