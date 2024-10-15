// <copyright file="ActivityPickingModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents an activity picking.
    /// </summary>
    [Table("tActivityPicking")]
    public class ActivityPickingModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity.
        /// </summary>
        [Indexed(Name = "UQ_ActivityPickingOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityModel))]
        public int ActivityRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the activity.
        /// </summary>
        [Indexed(Name = "UQ_ActivityPickingOnActivityRefnoAndActivitySubType", Order = 2, Unique = true)]
        [NotNull]
        public int ActivitySubType { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the activity.
        /// </summary>
        [Indexed(Name = "UQ_ActivityPickingOnActivityFKRefno", Order = 1, Unique = true)]
        [NotNull]
        public int ActivityFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the string foreign key reference number of the activity.
        /// </summary>
        [NotNull]
        public string ActivityStringFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the warehouse location map.
        /// </summary>
        [Indexed(Name = "FK_ActivityPickingOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        [ForeignKey(typeof(LocationMapWarehouseModel))]
        [NotNull]
        public int WarehouseLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the activity picking was ordered.
        /// </summary>
        [NotNull]
        public DateTime ActivityPickingOrderedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the activity picking is scheduled for delivery.
        /// </summary>
        [NotNull]
        public DateTime ActivityPickingDeliveryDateTime { get; set; }

        /// <summary>
        /// Gets or sets the name of the customer contact for the activity picking.
        /// </summary>
        [NotNull]
        public string ActivityPickingCustomerContactName { get; set; }

        /// <summary>
        /// Gets or sets the shipping information for the customer.
        /// </summary>
        [NotNull]
        public string ActivityPickingCustomerShipToInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the activity picking has been reviewed.
        /// </summary>
        [NotNull]
        public bool ActivityPickingIsReviewed { get; set; }
    }
}