// <copyright file="tActivityDetailMoveInventoryModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailMoveInventory"
    //+ "("
    //+ "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailMoveInventoryOnActivityDetailRefno PRIMARY KEY,"
    //+ "ActivityDetailMoveInventoryFKRefno INTEGER NOT NULL "
    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating detail move inventory table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityDetailMoveInventoryOnActivityDetailMoveInventoryFKRefno ON tActivityDetailMoveInventory(ActivityDetailMoveInventoryFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //string alterTable = "ALTER TABLE tActivityDetailMoveInventory ADD CONSTRAINT ActivityDetailMoveInventoryTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE";
    //await this.RunDDLCommand(alterTable);
    [Table("tActivityDetailMoveInventory")]
    public class tActivityDetailMoveInventoryModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityDetailMoveInventoryOnActivityDetailMoveInventoryFKRefno", Order = 1, Unique = true)]
        public int ActivityDetailMoveInventoryFKRefno { get; set; }
    }
}
