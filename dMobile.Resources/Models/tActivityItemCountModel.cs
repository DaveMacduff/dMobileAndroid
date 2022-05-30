// <copyright file="tActivityItemCountModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemCount"
    //+ "("
    //+ "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemCountOnActivityItemRefno PRIMARY KEY,"
    //+ "ActivityItemSubType INTEGER NOT NULL,"
    //+ "ActivityItemCountFKRefno INTEGER NOT NULL,"
    //+ "ActivityItemCountStringFKRefno nvarchar(8) NOT NULL,"
    //+ "ProductUnitRefno INTEGER NOT NULL,"
    //+ "LocationMapRefno INTEGER NOT NULL,"
    //+ "ActivityItemCountQuantityOnHand numeric(11, 3) NOT NULL,"
    //+ "ActivityItemCountBaseQuantityOnHand numeric(11, 3) NOT NULL,"
    //+ "ActivityItemCountWeightQuantityOnHand numeric(11, 3) NOT NULL,"
    //+ "ActivityItemCountQuantity numeric(11, 3) NOT NULL,"
    //+ "ActivityItemCountBaseQuantity numeric(11, 3) NOT NULL,"
    //+ "ActivityItemCountWeightQuantity numeric(11, 3) NOT NULL, "

    //+ "CONSTRAINT ActivityItemCountTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityItemCountToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityItemCountTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    
    //    this.MsgEventArgs.StatusOutMsg = "Creating count item table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemCountOnActivityItemRefnoAndActivityItemSubType ON tActivityItemCount(ActivityItemRefno ASC, ActivityItemSubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemCountOnActivityItemCountFKRefno ON tActivityItemCount(ActivityItemCountFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    ////// Note: Cannot enforce uniqueness on LocationMapRefno and ProductUnitRefno without ActivityRefno in the table.

    [Table("tActivityItemCount")]
    public class tActivityItemCountModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityItemModel)), Indexed(Name = "UQ_ActivityItemCountOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemCountOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        public int ActivityItemSubType { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemCountOnActivityItemCountFKRefno", Order = 1, Unique = true)]
        public int ActivityItemCountFKRefno { get; set; }

        [NotNull]
        public string ActivityItemCountStringFKRefno { get; set; }

        [NotNull, ForeignKey(typeof(tProductUnitModel))]
        public int ProductUnitRefno { get; set; }

        [NotNull, ForeignKey(typeof(tLocationMapModel))]
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
