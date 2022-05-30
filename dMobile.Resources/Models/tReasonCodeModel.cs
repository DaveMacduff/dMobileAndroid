// <copyright file="tReasonCodeModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tReasonCode"
    //+ "("
    //+ "ReasonCodeRefno INTEGER NOT NULL CONSTRAINT PK_ReasonCodeOnReasonCodeRefno PRIMARY KEY,"
    //+ "ReasonCodeFKRefno INTEGER NOT NULL,"
    //+ "ReasonCodeCode nvarchar(4) NOT NULL,"
    //+ "ReasonCodeDescription nvarchar(25) NOT NULL"
    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating reason code table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityOnReasonCodeFKRefno ON tReasonCode(ReasonCodeFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityOnReasonCodeCode ON tReasonCode(ReasonCodeCode ASC)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tReasonCode")]
    public class tReasonCodeModel
    {
        [PrimaryKey, NotNull]
        public int ReasonCodeRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityOnReasonCodeFKRefno", Order = 1, Unique = true)]
        public int ReasonCodeFKRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityOnReasonCodeCode", Order = 1, Unique = true)]
        public string ReasonCodeCode { get; set; }

        [NotNull]
        public string ReasonCodeDescription { get; set; }
    }
}
