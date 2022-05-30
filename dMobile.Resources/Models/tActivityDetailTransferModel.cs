// <copyright file="tActivityDetailTransferModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailTransfer"
    //+ "("

    //+ "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailTransferOnActivityDetailRefno PRIMARY KEY,"
    //+ "ActivityDetailTransferFKRefno INTEGER NOT NULL,"
    //+ "ActivityDetailTransferIsConfirmed bit NOT NULL, "
    //+ "ActivityDetailTransferTransModeEnum INTEGER NOT NULL, "

    //+ "CONSTRAINT ActivityDetailTransferTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating transfer detail table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityDetailTransferOnActivityDetailTransferFKRefnoAndTransModeEnum ON tActivityDetailTransfer(ActivityDetailTransferFKRefno ASC, ActivityDetailTransferTransModeEnum ASC)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tActivityDetailTransfer")]
    public class tActivityDetailTransferModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityDetailTransferOnActivityDetailTransferFKRefnoAndTransModeEnum", Order = 1, Unique = true)]
        public int ActivityDetailTransferFKRefno { get; set; }

        [NotNull]
        public bool ActivityDetailTransferIsConfirmed { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityDetailTransferOnActivityDetailTransferFKRefnoAndTransModeEnum", Order = 2, Unique = true)]
        public int ActivityDetailTransferTransModeEnum { get; set; }
    }
}
