// <copyright file="LocationMapModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;

    [Table("tLocationMap")]
    public class LocationMapModel
    {
        [PrimaryKey, AutoIncrement, NotNull]
        [Indexed(Name = "UQ_LocationMapOnLocationMapRefnoAndLocationMapSubType", Order = 1, Unique = true)]
        public int LocationMapRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_LocationMapOnLocationMapRefnoAndLocationMapSubType", Order = 2, Unique = true)]
        public int LocationMapSubType { get; set; }
    }
}
