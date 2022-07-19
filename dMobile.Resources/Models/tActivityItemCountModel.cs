// <copyright file="tActivityItemCountModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    // Note: Cannot enforce uniqueness on LocationMapRefno and ProductUnitRefno without ActivityRefno in the table.

    [Table("tActivityItemCount")]
    public class tActivityItemCountModel
    {
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(tActivityItemModel))]
        [Indexed(Name = "UQ_ActivityItemCountOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityItemCountOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        public int ActivityItemSubType { get; set; }

        [NotNull]
        [Indexed(Name = "UQ_ActivityItemCountOnActivityItemFKRefno", Order = 1, Unique = true)]
        public int ActivityItemFKRefno { get; set; }

        [NotNull]
        public string ActivityItemStringFKRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(tProductUnitModel))]
        public int ProductUnitRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(tLocationMapModel))]
        public int LocationMapRefno { get; set; }

        [NotNull]
        public decimal ActivityItemCountQuantityOnHand { get; set; }

        [NotNull]
        public decimal ActivityItemCountBaseQuantityOnHand { get; set; }

        [NotNull]
        public decimal ActivityItemCountWeightQuantityOnHand { get; set; }

        [NotNull]
        public decimal ActivityItemCountQuantity { get; set; }

        [NotNull]
        public decimal ActivityItemCountBaseQuantity { get; set; }

        [NotNull]
        public decimal ActivityItemCountWeightQuantity { get; set; }
    }
}
