// <copyright file="ActivityReturnsModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents an activity return.
    /// </summary>
    [Table("tActivityReturns")]
    public class ActivityReturnsModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityModel))]
        [Indexed(Name = "UQ_ActivityReturnsOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the activity.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityReturnsOnActivityRefnoAndActivitySubType", Order = 2, Unique = true)]
        public int ActivitySubType { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the activity.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityReturnsOnActivityFKRefno", Order = 1, Unique = true)]
        public int ActivityFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the string foreign key reference number of the activity.
        /// </summary>
        [NotNull]
        public string ActivityStringFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the warehouse location map.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(LocationMapWarehouseModel))]
        [Indexed(Name = "FK_ActivityReturnsOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the returns were ordered.
        /// </summary>
        [NotNull]
        public DateTime ActivityReturnsOrderedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the returns were delivered.
        /// </summary>
        [NotNull]
        public DateTime ActivityReturnsDeliveryDateTime { get; set; }

        /// <summary>
        /// Gets or sets the name of the customer contact for the returns.
        /// </summary>
        [NotNull]
        public string ActivityReturnsCustomerContactName { get; set; }

        /// <summary>
        /// Gets or sets the ship-to information for the customer returns.
        /// </summary>
        [NotNull]
        public string ActivityReturnsCustomerShipToInfo { get; set; }
    }
}