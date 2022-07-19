// <copyright file="tActivityItemModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityItem")]
    public class tActivityItemModel
    {
        [Indexed(Name = "UQ_ActivityDetailOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        [PrimaryKey]
        [AutoIncrement]
        [NotNull]
        public int ActivityItemRefno { get; set; }

        [Indexed(Name = "UQ_ActivityDetailOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        [NotNull]
        public int ActivityItemSubType { get; set; }

        [NotNull]
        public int ActivityItemStatus { get; set; }

        [Indexed(Name = "FK_ActivityItemOnActivityRefno", Order = 1, Unique = false)]
        [NotNull]
        [ForeignKey(typeof(tActivityModel))]
        public int ActivityRefno { get; set; }
    }
}
