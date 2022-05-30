// <copyright file="tActivityDetailPickingModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailPicking"
    //+ "("

    //+ "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailPickingOnActivityDetailRefno PRIMARY KEY,"
    //+ "ActivityDetailPickingFKRefno INTEGER NOT NULL,"
    //+ "ActivityDetailPickingIsConfirmed bit NOT NULL, "

    //+ "CONSTRAINT ActivityDetailPickingTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating picking detail table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityDetailPickingOnActivityDetailPickingFKRefno ON tActivityDetailPicking(ActivityDetailPickingFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tActivityDetailPicking")]
    public class tActivityDetailPickingModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityDetailPickingOnActivityDetailPickingFKRefno", Order = 1, Unique = true)]
        public int ActivityDetailPickingFKRefno { get; set; }

        [NotNull]
        public bool ActivityDetailPickingIsConfirmed { get; set; }
    }
}
