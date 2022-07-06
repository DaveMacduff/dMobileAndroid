// <copyright file="tContactModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;

    [Table("tContact")]
    public class tContactModel
    {
        [PrimaryKey, Indexed(Name = "UQ_ContactOnContactRefnoAndContactSubType", Order = 1, Unique = true), NotNull]
        public int ContactRefno { get; set; }

        [Indexed(Name = "UQ_ContactOnContactRefnoAndContactSubType", Order = 2, Unique = true), NotNull]
        public int ContactSubType { get; set; }
    }
}