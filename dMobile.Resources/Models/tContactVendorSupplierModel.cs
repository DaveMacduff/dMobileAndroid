// <copyright file="tContactVendorSupplierModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tContactVendorSupplier")]
    public class tContactVendorSupplierModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tContactModel)), Indexed(Name = "UQ_ContactVendorSupplierOnContactRefnoAndContactSubType", Order = 1, Unique = true)]
        public int ContactRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactVendorSupplierOnContactRefnoAndContactSubType", Order = 2, Unique = true)]
        public int ContactSubType { get; set; }

        [NotNull, ForeignKey(typeof(tContactVendorModel))]
        public int VendorContactRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactVendorSupplierOnContactVendorSupplierFKRefno", Order = 1, Unique = true)]
        public int ContactVendorSupplierFKRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactVendorSupplierOnContactVendorSupplierCode", Order = 1, Unique = true)]
        public string ContactVendorSupplierCode { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactVendorSupplierOnContactVendorSupplierName", Order = 1, Unique = true)]
        public string ContactVendorSupplierName { get; set; }
    }
}
