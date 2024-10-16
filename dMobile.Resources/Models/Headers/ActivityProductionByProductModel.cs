// <copyright file="ActivityProductionByProductModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models.Headers
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents an activity production by-product.
    /// </summary>
    [Table("tActivityProductionByProduct")]
    public class ActivityProductionByProductModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity production.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(ActivityProductionModel))]
        [Indexed(Name = "PK_ActivityProductionByProductOnActivityRefno", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the activity.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityProductionByProductOnActivityFKRefno", Order = 1, Unique = true)]
        public int ActivityFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the string foreign key reference number of the activity.
        /// </summary>
        [NotNull]
        public string ActivityStringFKRefno { get; set; }
    }
}