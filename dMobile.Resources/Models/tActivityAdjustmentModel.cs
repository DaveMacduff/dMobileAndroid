// <copyright file="tActivityAdjustmentModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityAdjustment"
    //+ "("
    //+ "ActivityRefno INTEGER NOT NULL CONSTRAINT PK_ActivityAdjustmentOnActivityRefno PRIMARY KEY,"
    //+ "ActivitySubType INTEGER NOT NULL,"
    //+ "ActivityAdjustmentFKRefno INTEGER NOT NULL,"
    //+ "ActivityAdjustmentStringFKRefno nvarchar(8) NOT NULL,"
    //+ "ActivityAdjustmentType INTEGER NOT NULL,"
    //+ "WarehouseLocationMapRefno INTEGER NOT NULL,"
    //+ "ActivityAdjustmentDateTime datetime NOT NULL,"
    //+ "ActivityAdjustmentDescription nvarchar(40) NOT NULL,"
    //+ "ActivityAdjustmentTypeDescription nvarchar(20) NOT NULL, "

    //+ "CONSTRAINT ActivityAdjustmentToActivityOnActivityRefnoAndActivitySubType FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityAdjustmentToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating adjustment table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityAdjustmentOnActivityRefnoAndActivitySubType ON tActivityAdjustment(ActivityRefno ASC, ActivitySubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityAdjustmentOnActivityAdjustmentFKRefno ON tActivityAdjustment(ActivityAdjustmentFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityAdjustmentOnWarehouseLocationMapRefno ON tActivityAdjustment(WarehouseLocationMapRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tActivityAdjustment")]
    public class tActivityAdjustmentModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityModel)), Indexed(Name = "UQ_ActivityAdjustmentOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityAdjustmentOnActivityRefnoAndActivitySubType", Order = 2, Unique = true)]
        public int ActivitySubType { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityAdjustmentOnActivityAdjustmentFKRefno", Order = 1, Unique = true)]
        public int ActivityAdjustmentFKRefno { get; set; }

        [NotNull]
        public string ActivityAdjustmentStringFKRefno { get; set; }

        [NotNull]
        public int ActivityAdjustmentType { get; set; }

        [NotNull, ForeignKey(typeof(tLocationMapWarehouseModel)), Indexed(Name = "FK_ActivityAdjustmentOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        public DateTime ActivityAdjustmentDateTime { get; set; }

        [NotNull]
        public string ActivityAdjustmentDescription { get; set; }

        [NotNull]
        public string ActivityAdjustmentTypeDescription { get; set; }
    }
}
