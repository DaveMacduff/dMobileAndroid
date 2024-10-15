// <copyright file="ActivityModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;

    /// <summary>
    /// Represents an activity.
    /// </summary>
    [Table("tActivity")]
    public class ActivityModel
    {
        /// <summary>
        /// Gets or sets the unique reference number for the activity.
        /// </summary>
        [PrimaryKey]
        [AutoIncrement]
        [NotNull]
        [Indexed(Name = "UQ_ActivityOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        /// <summary>
        /// Gets or sets the status of the activity.
        /// </summary>
        [NotNull]
        public int ActivityStatus { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the activity.
        /// </summary>
        [Indexed(Name = "UQ_ActivityOnActivityRefnoAndActivitySubType", Order = 2, Unique = true)]
        [NotNull]
        public int ActivitySubType { get; set; }
    }
}