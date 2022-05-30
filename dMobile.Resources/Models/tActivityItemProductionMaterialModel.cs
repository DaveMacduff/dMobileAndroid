// <copyright file="tActivityItemProductionMaterialModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemProductionMaterial"
    //+ "("
    //+ "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemProductionMaterialOnActivityItemRefno PRIMARY KEY,"
    //+ "ActivityItemProductionMaterialFKRefno INTEGER NOT NULL,"
    //+ "ActivityItemProductionMaterialStringFKRefno nvarchar(8) NOT NULL, "

    //+ "CONSTRAINT ActivityItemProductionMaterialTactItemProductionOnActivityRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItemProduction(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating item production material table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemProductionMaterialOnActivityItemProductionMaterialFKRefno ON tActivityItemProductionMaterial(ActivityItemProductionMaterialFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tActivityItemProductionMaterial")]
    public class tActivityItemProductionMaterialModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityItemProductionModel))]
        public int ActivityItemRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemProductionMaterialOnActivityItemProductionMaterialFKRefno", Order = 1, Unique = true)]
        public int ActivityItemProductionMaterialFKRefno { get; set; }

        [NotNull]
        public string ActivityItemProductionMaterialStringFKRefno { get; set; }
    }
}
