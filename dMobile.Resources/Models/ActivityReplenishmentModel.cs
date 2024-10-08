// <copyright file="ActivityReplenishmentModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Activity replenishment model.
    /// </summary>
    [Table("tActivityReplenishment")]
    public class ActivityReplenishmentModel
    {
        /// <summary>
        /// Gets or sets the activity reference number.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [Indexed(Name = "UQ_ActivityReplenishmentOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        [ForeignKey(typeof(ActivityModel))]
        public int ActivityRefno { get; set; }

        /// <summary>
        /// Gets or sets the activity sub type.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityReplenishmentOnActivityRefnoAndActivitySubType", Order = 2, Unique = true)]
        public int ActivitySubType { get; set; }

        /// <summary>
        /// Gets or sets the warehouse location map reference number. Login warehouse filtered.
        /// </summary>
        [NotNull]
        public int WarehouseLocationMapRefno { get; set; }
    }
}
