// <copyright file="tContactVendorModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tContactVendor")]
    public class tContactVendorModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tContactModel)), Indexed(Name = "UQ_ContactVendorOnContactRefnoAndContactSubType", Order = 1, Unique = true)]
        public int ContactRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactVendorOnContactRefnoAndContactSubType", Order = 2, Unique = true)]
        public int ContactSubType { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactVendorOnContactVendorFKRefno", Order = 1, Unique = true)]
        public int ContactVendorFKRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactVendorOnContactVendorCode", Order = 1, Unique = true)]
        public string ContactVendorCode { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactVendorOnContactVendorName", Order = 1, Unique = true)]
        public string ContactVendorName { get; set; }
    }
}
