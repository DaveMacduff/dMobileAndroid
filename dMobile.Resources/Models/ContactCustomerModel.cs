// <copyright file="ContactCustomerModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tContactCustomer")]
    public class ContactCustomerModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ContactModel))]
        [Indexed(Name = "UQ_ContactCustomerOnContactRefnoAndContactSubType", Order = 1, Unique = true)]
        public int ContactRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ContactCustomerOnContactRefnoAndContactSubType", Order = 2, Unique = true)]
        public int ContactSubType { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ContactCustomerOnContactCustomerFKRefno", Order = 1, Unique = true)]
        public int ContactCustomerFKRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ContactCustomerOnContactCustomerCode", Order = 1, Unique = true)]
        public string ContactCustomerCode { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ContactCustomerOnContactCustomerName", Order = 1, Unique = true)]
        public string ContactCustomerName { get; set; }
    }
}
