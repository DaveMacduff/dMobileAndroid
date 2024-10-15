// <copyright file="ActivityProductionFinishedModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents an activity production finished.
    /// </summary>
    [Table("tActivityProductionFinished")]
    public class ActivityProductionFinishedModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity production.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(ActivityProductionModel))]
        [Indexed(Name = "PK_ActivityProductionFinishedOnActivityRefno", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the activity.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityProductionFinishedOnActivityFKRefno", Order = 1, Unique = true)]
        public int ActivityFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the string foreign key reference number of the activity.
        /// </summary>
        [NotNull]
        public string ActivityStringFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the default logistic unit batch lot number for the finished production.
        /// </summary>
        [NotNull]
        public string ActivityProductionFinishedDefaultLogisticUnitBatchLotNumber { get; set; }
    }
}