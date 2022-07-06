// <copyright file="tUnitModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tUnit")]
    public class tUnitModel
    {
        [PrimaryKey, NotNull]
        public int UnitRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_UnitOnUnitCode", Order = 1, Unique = true)]
        public string UnitCode { get; set; }
    }
}
