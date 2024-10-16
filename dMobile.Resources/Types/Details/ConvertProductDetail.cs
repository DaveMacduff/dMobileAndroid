// <copyright file="ConvertProductDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types.Details
{
    /// <summary>
    /// Define the convert product detail model.
    /// </summary>
    public class ConvertProductDetail : ActivityDetail
    {
        /// <summary>
        /// Gets or sets the activity item foreign key reference number.
        /// </summary>
        public int ActivityItemFKRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the activity header foreign key reference number.
        /// </summary>
        public int ActivityHdrFKRefno { get; set; } = 0;
    }
}
