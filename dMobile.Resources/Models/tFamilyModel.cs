// <copyright file="tFamilyModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tFamily")]
    public class tFamilyModel
    {
        [PrimaryKey]
        [NotNull]
        public int FamilyRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_FamilyOnFamilyCode", Order = 1, Unique = true)]
        public string FamilyCode { get; set; }
    }
}
