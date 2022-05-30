// <copyright file="SkidPutAwayDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;

    /// <summary>
    /// Define the skid put away detail model.
    /// </summary>
    public class SkidPutAwayDetail : ActivityDetail
    {
        /// <summary>
        /// Gets or sets the activity detail status.
        /// </summary>
        public int ActivityDetailStatus { get; set; } = 0;
    }
}
