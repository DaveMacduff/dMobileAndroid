// <copyright file="ActivityItemReasonCodeXRefModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models.Items
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents a cross-reference between an activity item and a reason code.
    /// </summary>
    [Table("tActivityItemReasonCodeXRef")]
    public class ActivityItemReasonCodeXRefModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity item.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(ActivityItemModel))]
        [Indexed(Name = "FK_ActivityItemReasonCodeXRefOnActivityItemRefno", Order = 1, Unique = false)]
        public int ActivityItemRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the reason code.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(ReasonCodeModel))]
        [Indexed(Name = "FK_ActivityItemReasonCodeXRefOnReasonCodeRefno", Order = 1, Unique = false)]
        public int ReasonCodeRefno { get; set; }
    }
}