// <copyright file="tActivityDetailCountModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailCount"
    //+ "("
    //+ "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailCountOnActivityDetailRefno PRIMARY KEY,"
    //+ "ActivityDetailCountFKRefno INTEGER NOT NULL, "

    //+ "CONSTRAINT ActivityDetailCountTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating count detail table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityDetailCountOnActivityDetailCountFKRefno ON tActivityDetailCount(ActivityDetailCountFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tActivityDetailCount")]
    public class tActivityDetailCountModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }

        [Indexed(Name = "UQ_ActivityDetailCountOnActivityDetailCountFKRefno", Order = 1, Unique = true), NotNull]
        public int ActivityDetailCountFKRefno { get; set; }
    }
}
