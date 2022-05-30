// <copyright file="tActivityDetailProductionByProductModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailProductionByProduct"
    //+ "("
    //+ "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailProductionByProductOnActivityDetailRefno PRIMARY KEY,"
    //+ "ActivityDetailProductionByProductFKRefno INTEGER NOT NULL, "

    //+ "CONSTRAINT ActivityDetailProductionByProductTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE"
    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating detail production byproduct table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityDetailProductionByProductOnActivityDetailProductionByProductFKRefno ON tActivityDetailProductionByProduct(ActivityDetailProductionByProductFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tActivityDetailProductionByProduct")]
    public class tActivityDetailProductionByProductModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityDetailProductionByProductOnActivityDetailProductionByProductFKRefno", Order = 1, Unique = true)]
        public int ActivityDetailProductionByProductFKRefno { get; set; }
    }
}
