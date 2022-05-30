// <copyright file="tActivityDetailModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetail"
    //+ "("
    //+ "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailOnActivityDetailRefno PRIMARY KEY,"
    //+ "ActivityDetailSubType INTEGER NOT NULL,"
    //+ "ActivityDetailStatus INTEGER NOT NULL,"
    //+ "ActivityItemRefno INTEGER NOT NULL,"
    //+ "LocationMapRefno INTEGER NOT NULL,"
    //+ "ProductUnitRefno INTEGER NOT NULL,"
    //+ "ActivityDetailUniqueSchemeID nvarchar(80),"
    //+ "ActivityDetailSchemeID nvarchar(80),"
    //+ "ActivityDetailQuantity numeric(11, 3) NOT NULL,"
    //+ "ActivityDetailBaseQuantity numeric(11, 3) NOT NULL,"
    //+ "ActivityDetailWeightQuantity numeric(11, 3) NOT NULL,"
    //+ "ActivityDetailSingleTradeItemSerialNumber nvarchar(20) NOT NULL,"
    //+ "ActivityDetailLogisticUnitBatchLotNumber nvarchar(20) NOT NULL,"
    //+ "ActivityDetailLogisticUnitBestBeforeSellByDate datetime NOT NULL,"
    //+ "ActivityDetailLogisticUnitExpiryUseByDate datetime NOT NULL,"
    //+ "ActivityDetailLogisticUnitProductionDate datetime NOT NULL,"
    //+ "ActivityDetailLogisticUnitDueDate datetime NOT NULL,"
    //+ "ActivityDetailLogisticUnitPackagingDate datetime NOT NULL,"
    //+ "ActivityDetailLogisticUnitSkidNumber nvarchar(18) NOT NULL, "

    //+ "CONSTRAINT ActivityDetailTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityDetailTactItemOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityDetailTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE"
    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating activity detail table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //        string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityDetailOnActivityDetailRefnoAndActivityDetailSubType "
    //            + "ON tActivityDetail(ActivityDetailRefno ASC, ActivityDetailSubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS ActivityDetailOnActivityDetailUniqueSchemeID ON "
    //            + "tActivityDetail(ActivityDetailUniqueSchemeID ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityDetailOnActivityItemRefno ON "
    //            + "tActivityDetail(ActivityItemRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityDetailOnLocationMapRefno ON "
    //            + "tActivityDetail(LocationMapRefno ASC)";                                                             
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityDetailOnProductUnitRefno ON "
    //            + "tActivityDetail(ProductUnitRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);
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
