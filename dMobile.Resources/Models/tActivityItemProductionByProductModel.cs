// <copyright file="tActivityItemProductionByProductModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemProductionByProduct"
    //+ "("

    //+ "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemProductionByProductOnActivityItemRefno PRIMARY KEY,"
    //+ "ActivityItemProductionByProductFKRefno INTEGER NOT NULL,"
    //+ "ActivityItemProductionByProductStringFKRefno nvarchar(8) NOT NULL, "

    //+ "CONSTRAINT ActivityItemProductionByProductTactItemProductionOnActivityRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItemProduction(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating item production byproduct table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemProductionByProductOnActivityItemProductionByProductFKRefno ON tActivityItemProductionByProduct(ActivityItemProductionByProductFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tActivityItemProductionByProduct")]
    public class tActivityItemProductionByProductModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityItemProductionModel))]
        public int ActivityItemRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemProductionByProductOnActivityItemProductionByProductFKRefno", Order = 1, Unique = true)]
        public int ActivityItemProductionByProductFKRefno { get; set; }

        [NotNull]
        public string ActivityItemProductionByProductStringFKRefno { get; set; }
    }
}
