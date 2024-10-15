// <copyright file="ActivityDetailTransferModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a transfer detail activity.
    /// </summary>
    [Table("tActivityDetailTransfer")]
    public class ActivityDetailTransferModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity detail.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the activity detail.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityDetailTransferOnActivityDetailFKRefnoAndTransModeEnum", Order = 1, Unique = true)]
        public int ActivityDetailFKRefno { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the transfer is confirmed.
        /// </summary>
        [NotNull]
        public bool ActivityDetailTransferIsConfirmed { get; set; }

        /// <summary>
        /// Gets or sets the transfer mode enumeration.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityDetailTransferOnActivityDetailFKRefnoAndTransModeEnum", Order = 2, Unique = true)]
        public int ActivityDetailTransferTransModeEnum { get; set; }
    }
}