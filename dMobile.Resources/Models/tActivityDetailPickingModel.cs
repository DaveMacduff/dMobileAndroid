// <copyright file="tActivityDetailPickingModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityDetailPicking")]
    public class tActivityDetailPickingModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(tActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityDetailPickingOnActivityDetailFKRefno", Order = 1, Unique = true)]
        public int ActivityDetailFKRefno { get; set; }

        [NotNull]
        public bool ActivityDetailPickingIsConfirmed { get; set; }
    }
}
