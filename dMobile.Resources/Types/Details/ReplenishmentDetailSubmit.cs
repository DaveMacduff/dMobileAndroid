// <copyright file="ReplenishmentDetailSubmit.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types.Details
{
    using System;

    /// <summary>
    /// Used for submitting the replenishment detail.
    /// </summary>
    public class ReplenishmentDetailSubmit
    {
        /// <summary>
        /// Gets or sets the activity detail reference number.
        /// </summary>
        public int ActivityDetailRefno { get; set; }

        /// <summary>
        /// Gets or sets the product scheme variant number.
        /// </summary>
        public int ProductSchemeVariantNo { get; set; }

        /// <summary>
        /// Gets or sets the activity detail single trade item serial number.
        /// </summary>
        public string ActivityDetailSingleTradeItemSerialNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity detail logistic unit best before sell by date.
        /// </summary>
        public DateTime ActivityDetailLogisticUnitBestBeforeSellByDate { get; set; } = ResSysCommon.EmptyDateTime;

        /// <summary>
        /// Gets or sets the activity detail logistic unit batch lot number.
        /// </summary>
        public string ActivityDetailLogisticUnitBatchLotNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity detail logistic unit production date.
        /// </summary>
        public DateTime ActivityDetailLogisticUnitProductionDate { get; set; } = ResSysCommon.EmptyDateTime;

        /// <summary>
        /// Gets or sets the activity detail logistic unit skid number.
        /// </summary>
        public string ActivityDetailLogisticUnitSkidNumber { get; set; } = string.Empty;
    }
}
