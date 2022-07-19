// <copyright file="tContactCustomerShipToModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tContactCustomerShipTo")]
    public class tContactCustomerShipToModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(tContactModel))]
        [Indexed(Name = "UQ_ContactCustomerShipToOnContactRefnoAndContactSubType", Order = 1, Unique = true)]
        public int ContactRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ContactCustomerShipToOnContactRefnoAndContactSubType", Order = 2, Unique = true)]
        public int ContactSubType { get; set; }

        [NotNull]
        [ForeignKey(typeof(tContactCustomerModel))]
        public int CustomerContactRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ContactCustomerShipToOnContactCustomerShipToFKRefno", Order = 1, Unique = true)]
        public int ContactCustomerShipToFKRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ContactCustomerShipToOnContactCustomerShipToCode", Order = 1, Unique = true)]
        public string ContactCustomerShipToCode { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ContactCustomerShipToOnContactCustomerShipToName", Order = 1, Unique = true)]
        public string ContactCustomerShipToName { get; set; }
    }
}
