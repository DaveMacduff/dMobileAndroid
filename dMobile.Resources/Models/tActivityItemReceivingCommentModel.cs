// <copyright file="tActivityItemReceivingCommentModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemReceivingComment"
    //+ "("
    //+ "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemReceivingCommentOnActivityItemRefno PRIMARY KEY,"
    //+ "ActivityItemReceivingCommentComment nvarchar(255) NOT NULL, "

    //+ "CONSTRAINT ActivityItemReceivingCommentTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItemReceiving(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating receiving comment item table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);
    [Table("tActivityItemReceivingComment")]
    public class tActivityItemReceivingCommentModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityItemReceivingModel))]
        public int ActivityItemRefno { get; set; }

        [NotNull]
        public string ActivityItemReceivingCommentComment { get; set; }
    }
}
