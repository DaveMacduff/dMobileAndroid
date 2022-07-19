// <copyright file="tActivityItemMoveInventoryModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityItemMoveInventory")]
    public class tActivityItemMoveInventoryModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(tActivityItemModel))]
        [Indexed(Name = "UQ_ActivityItemMoveInventoryOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityItemMoveInventoryOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        public int ActivityItemSubType { get; set; }

        [NotNull]
        public int ActivityItemMoveInventoryType { get; set; }

        [NotNull]
        [ForeignKey(typeof(tProductUnitModel))]
        public int ProductUnitRefno { get; set; }
    }
}
