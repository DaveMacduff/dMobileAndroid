// <copyright file="tActivityPickingModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityPicking")]
    public class tActivityPickingModel
    {
        [Indexed(Name = "UQ_ActivityPickingOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(tActivityModel))]
        public int ActivityRefno { get; set; }

        [Indexed(Name = "UQ_ActivityPickingOnActivityRefnoAndActivitySubType", Order = 2, Unique = true)]
        [NotNull]
        public int ActivitySubType { get; set; }

        [Indexed(Name = "UQ_ActivityPickingOnActivityFKRefno", Order = 1, Unique = true)]
        [NotNull]
        public int ActivityFKRefno { get; set; }

        [NotNull]
        public string ActivityStringFKRefno { get; set; }

        [Indexed(Name = "FK_ActivityPickingOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        [ForeignKey(typeof(tLocationMapWarehouseModel))]
        [NotNull]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        public DateTime ActivityPickingOrderedDateTime { get; set; }

        [NotNull]
        public DateTime ActivityPickingDeliveryDateTime { get; set; }

        [NotNull]
        public string ActivityPickingCustomerContactName { get; set; }

        [NotNull]
        public string ActivityPickingCustomerShipToInfo { get; set; }

        [NotNull]
        public bool ActivityPickingIsReviewed { get; set; }
    }
}
