// <copyright file="tLocationMapTypeModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tLocationMapType")]
    public class tLocationMapTypeModel
    {
        [NotNull]
        [ForeignKey(typeof(tLocationMapModel))]
        [Indexed(Name = "FK_LocationMapTypeOnLocationMapRefno", Order = 1, Unique = false)]
        public int LocationMapRefno { get; set; }

        [NotNull]
        [Indexed(Name = "PK_LocationMapTypeOnLocationMapTypeEnum", Order = 1, Unique = false)]
        public int LocationMapTypeEnum { get; set; }
    }
}
