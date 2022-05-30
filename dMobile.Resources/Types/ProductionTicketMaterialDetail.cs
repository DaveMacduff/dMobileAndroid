// <copyright file="ProductionTicketMaterialDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    /// <summary>
    /// Define the production ticket material detail model.
    /// </summary>
    public class ProductionTicketMaterialDetail : ActivityDetail
    {
        /// <summary>
        /// Gets or sets the activity detail status.
        /// </summary>
        public int ActivityDetailStatus { get; set; } = 0;
    }
}
