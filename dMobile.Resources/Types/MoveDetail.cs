// <copyright file="MoveDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    /// <summary>
    /// Define the move detail model.
    /// </summary>
    public class MoveDetail : ActivityDetail
    {
        /// <summary>
        /// Gets or sets the activity detail status.
        /// </summary>
        public int ActivityDetailStatus { get; set; } = 0;
    }
}
