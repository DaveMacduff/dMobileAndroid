// <copyright file="tActivityDetailModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityDetail")]
    public class tActivityDetailModel
    {
        [Indexed(Name = "UQ_ActivityDetailOnActivityDetailRefnoAndActivityDetailSubType", Order = 1, Unique = true), PrimaryKey, NotNull]
        public int ActivityDetailRefno { get; set; }

        [Indexed(Name = "UQ_ActivityDetailOnActivityDetailRefnoAndActivityDetailSubType", Order = 2, Unique = true), NotNull]
        public int ActivityDetailSubType { get; set; }

        [NotNull]
        public int ActivityDetailStatus { get; set; }

        [Indexed(Name = "FK_ActivityDetailOnActivityItemRefno", Order = 1, Unique = false), ForeignKey(typeof(tContactModel)), NotNull]
        public int ActivityItemRefno { get; set; }

        [Indexed(Name = "FK_ActivityDetailOnLocationMapRefno", Order = 1, Unique = false), ForeignKey(typeof(tLocationMapModel)), NotNull]
        public int LocationMapRefno { get; set; }

        [ForeignKey(typeof(tProductUnitModel)), NotNull]

        [Indexed(Name = "FK_ActivityDetailOnProductUnitRefno", Order = 1, Unique = false)]
        public int ProductUnitRefno { get; set; }

        [Indexed(Name = "ActivityDetailOnActivityDetailUniqueSchemeID", Order = 1, Unique = false)]
        public string ActivityDetailUniqueSchemeID { get; set; }

        public string ActivityDetailSchemeID { get; set; }

        [NotNull]
        public decimal ActivityDetailQuantity { get; set; }

        [NotNull]
        public decimal ActivityDetailBaseQuantity { get; set; }

        [NotNull]
        public decimal ActivityDetailWeightQuantity { get; set; }

        [NotNull]
        public string ActivityDetailSingleTradeItemSerialNumber { get; set; }

        [NotNull]
        public string ActivityDetailLogisticUnitBatchLotNumber { get; set; }

        [NotNull]
        public DateTime ActivityDetailLogisticUnitBestBeforeSellByDate { get; set; }

        [NotNull]
        public DateTime ActivityDetailLogisticUnitExpiryUseByDate { get; set; }

        [NotNull]
        public DateTime ActivityDetailLogisticUnitProductionDate { get; set; }

        [NotNull]
        public DateTime ActivityDetailLogisticUnitDueDate { get; set; }

        [NotNull]
        public DateTime ActivityDetailLogisticUnitPackagingDate { get; set; }

        [NotNull]
        public string ActivityDetailLogisticUnitSkidNumber { get; set; }
    }
}
