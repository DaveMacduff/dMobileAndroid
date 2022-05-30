// <copyright file="tActivityDetailReceivingModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailReceiving"
    //+ "("

    //+ "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailReceivingOnActivityDetailRefno PRIMARY KEY,"
    //+ "ActivityDetailReceivingFKRefno INTEGER NOT NULL, "

    //+ "CONSTRAINT ActivityDetailReceivingTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating receiving detail table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityDetailReceivingOnActivityDetailReceivingFKRefno ON tActivityDetailReceiving(ActivityDetailReceivingFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tActivityDetailReceiving")]
    public class tActivityDetailReceivingModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }

        [Indexed(Name = "UQ_ActivityDetailReceivingOnActivityDetailReceivingFKRefno", Order = 1, Unique = true), NotNull]
        public int ActivityDetailReceivingFKRefno { get; set; }
    }
}
