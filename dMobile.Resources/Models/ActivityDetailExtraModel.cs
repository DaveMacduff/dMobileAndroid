// <copyright file="ActivityDetailExtraModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents extra details for an activity.
    /// </summary>
    [Table("tActivityDetailExtra")]
    public class ActivityDetailExtraModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity detail.
        /// </summary>
        [PrimaryKey]
        [Indexed(Name = "UQ_ActivityDetailExtraOnActivityDetailRefnoAndActivityDetailSubType", Order = 1, Unique = true)]
        [NotNull]
        [ForeignKey(typeof(ActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the activity detail.
        /// </summary>
        [Indexed(Name = "UQ_ActivityDetailExtraOnActivityDetailRefnoAndActivityDetailSubType", Order = 2, Unique = true)]
        [NotNull]
        public int ActivityDetailSubType { get; set; }

        /// <summary>
        /// Gets or sets the first extra text detail.
        /// </summary>
        [NotNull]
        public string ActivityDetailExtra1Text { get; set; }

        /// <summary>
        /// Gets or sets the second extra text detail.
        /// </summary>
        [NotNull]
        public string ActivityDetailExtra2Text { get; set; }

        /// <summary>
        /// Gets or sets the third extra text detail.
        /// </summary>
        [NotNull]
        public string ActivityDetailExtra3Text { get; set; }

        /// <summary>
        /// Gets or sets the fourth extra text detail.
        /// </summary>
        [NotNull]
        public string ActivityDetailExtra4Text { get; set; }

        /// <summary>
        /// Gets or sets the date for the extra detail.
        /// </summary>
        [NotNull]
        public DateTime ActivityDetailExtra5Date { get; set; }
    }
}