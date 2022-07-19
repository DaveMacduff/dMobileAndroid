// <copyright file="tActivityReceivingModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityReceiving")]
    public class tActivityReceivingModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(tActivityModel))]
        [Indexed(Name = "UQ_ActivityReceivingOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityReceivingOnActivityRefnoAndActivitySubType", Order = 2, Unique = true)]
        public int ActivitySubType { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityReceivingOnActivityFKRefno", Order = 1, Unique = true)]
        public int ActivityFKRefno { get; set; }

        [NotNull]
        public string ActivityStringFKRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityReceivingOnActivityReceivingPONumber", Order = 1, Unique = true)]
        public string ActivityReceivingPONumber { get; set; }

        [NotNull]
        [ForeignKey(typeof(tLocationMapWarehouseModel))]
        [Indexed(Name = "FK_ActivityReceivingOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        public DateTime ActivityReceivingIssuedDateTime { get; set; }

        [NotNull]
        public DateTime ActivityReceivingArrivalDateTime { get; set; }

        [NotNull]
        public string ActivityReceivingVendorContactName { get; set; }

        [NotNull]
        public string ActivityReceivingVendorSupplierContactName { get; set; }

        [NotNull]
        public string ActivityReceivingVendorSupplierGTINCompanyPrefix { get; set; }
    }
}
