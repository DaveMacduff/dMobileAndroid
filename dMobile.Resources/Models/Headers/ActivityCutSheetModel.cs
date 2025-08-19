// <copyright file="ActivityCutSheetModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models.Headers
{
    using System;
    using dMobile.Resources.Models.LocationMap;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents an activity production.
    /// </summary>
    [Table("tActivityCutSheet")]
    public class ActivityCutSheetModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityModel))]
        [Indexed(Name = "UQ_ActivityCutSheetOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the activity.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityCutSheetOnActivityRefnoAndActivitySubType", Order = 2, Unique = true)]
        public int ActivitySubType { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the warehouse location map.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(LocationMapWarehouseModel))]
        [Indexed(Name = "FK_ActivityCutSheetOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the activity production.
        /// </summary>
        [NotNull]
        public DateTime ActivityCutSheetDateTime { get; set; }

        /// <summary>
        /// Gets or sets the description of the activity production.
        /// </summary>
        [NotNull]
        public string ActivityCutSheetDescription { get; set; }
    }
}