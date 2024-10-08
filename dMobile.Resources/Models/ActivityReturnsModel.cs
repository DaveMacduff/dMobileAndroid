// <copyright file="ActivityReturnsModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityReturns")]
    public class ActivityReturnsModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityModel))]
        [Indexed(Name = "UQ_ActivityReturnsOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityReturnsOnActivityRefnoAndActivitySubType", Order = 2, Unique = true)]
        public int ActivitySubType { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityReturnsOnActivityFKRefno", Order = 1, Unique = true)]
        public int ActivityFKRefno { get; set; }

        [NotNull]
        public string ActivityStringFKRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(LocationMapWarehouseModel))]
        [Indexed(Name = "FK_ActivityReturnsOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        public DateTime ActivityReturnsOrderedDateTime { get; set; }

        [NotNull]
        public DateTime ActivityReturnsDeliveryDateTime { get; set; }

        [NotNull]
        public string ActivityReturnsCustomerContactName { get; set; }

        [NotNull]
        public string ActivityReturnsCustomerShipToInfo { get; set; }
    }
}
