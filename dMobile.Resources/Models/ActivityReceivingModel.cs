// <copyright file="ActivityReceivingModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents an activity receiving.
    /// </summary>
    [Table("tActivityReceiving")]
    public class ActivityReceivingModel
    {
        /// <summary>
        /// Gets or sets the reference number of the activity.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityModel))]
        [Indexed(Name = "UQ_ActivityReceivingOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the activity.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityReceivingOnActivityRefnoAndActivitySubType", Order = 2, Unique = true)]
        public int ActivitySubType { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number of the activity.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityReceivingOnActivityFKRefno", Order = 1, Unique = true)]
        public int ActivityFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the string foreign key reference number of the activity.
        /// </summary>
        [NotNull]
        public string ActivityStringFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the purchase order number for the activity receiving.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityReceivingOnActivityReceivingPONumber", Order = 1, Unique = true)]
        public string ActivityReceivingPONumber { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the warehouse location map.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(LocationMapWarehouseModel))]
        [Indexed(Name = "FK_ActivityReceivingOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the activity receiving was issued.
        /// </summary>
        [NotNull]
        public DateTime ActivityReceivingIssuedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the activity receiving arrived.
        /// </summary>
        [NotNull]
        public DateTime ActivityReceivingArrivalDateTime { get; set; }

        /// <summary>
        /// Gets or sets the name of the vendor contact for the activity receiving.
        /// </summary>
        [NotNull]
        public string ActivityReceivingVendorContactName { get; set; }

        /// <summary>
        /// Gets or sets the name of the vendor supplier contact for the activity receiving.
        /// </summary>
        [NotNull]
        public string ActivityReceivingVendorSupplierContactName { get; set; }

        /// <summary>
        /// Gets or sets the GTIN company prefix of the vendor supplier for the activity receiving.
        /// </summary>
        [NotNull]
        public string ActivityReceivingVendorSupplierGTINCompanyPrefix { get; set; }
    }
}