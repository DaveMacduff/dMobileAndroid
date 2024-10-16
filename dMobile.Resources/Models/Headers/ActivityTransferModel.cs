// <copyright file="ActivityTransferModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models.Headers
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents an activity transfer.
    /// </summary>
    [Table("tActivityTransfer")]
    public class ActivityTransferModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityModel))]
        [Indexed(Name = "UQ_ActivityTransferOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the activity.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityTransferOnActivityRefnoAndActivitySubType", Order = 2, Unique = true)]
        public int ActivitySubType { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the activity.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityTransferOnActivityFKRefno", Order = 1, Unique = false)]
        public int ActivityFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the string foreign key reference number of the activity.
        /// </summary>
        [NotNull]
        public string ActivityStringFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the transfer warehouse location map.
        /// </summary>
        [NotNull]
        public int TransferWarehouseLocationMapFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the description of the transfer warehouse location map.
        /// </summary>
        [NotNull]
        public string TransferWarehouseLocationMapDescription { get; set; }

        /// <summary>
        /// Gets or sets the description of the activity transfer.
        /// </summary>
        [NotNull]
        public string ActivityTransferDescription { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the activity transfer.
        /// </summary>
        [NotNull]
        public DateTime ActivityTransferDateTime { get; set; }

        /// <summary>
        /// Gets or sets the arrival date and time of the activity transfer.
        /// </summary>
        [NotNull]
        public DateTime ActivityTransferArrivalDateTime { get; set; }
    }
}