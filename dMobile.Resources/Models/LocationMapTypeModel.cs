// <copyright file="LocationMapTypeModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tLocationMapType")]
    public class LocationMapTypeModel
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int LocationMapTypeRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(LocationMapModel))]
        [Indexed(Name = "FK_LocationMapTypeOnLocationMapRefno", Order = 1, Unique = false)]
        public int LocationMapRefno { get; set; }

        [NotNull]
        [Indexed(Name = "PK_LocationMapTypeOnLocationMapTypeEnum", Order = 1, Unique = false)]
        public int LocationMapTypeEnum { get; set; }
    }
}
