// <copyright file="tActivityItemReasonCodeXRefModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemReasonCodeXRef"
    //+ "("

    //+ "ActivityItemRefno INTEGER NOT NULL,"
    //+ "ReasonCodeRefno INTEGER NOT NULL, "

    //+ "CONSTRAINT ActivityItemReasonCodeXRefTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityItemReasonCodeXRefToReasonCodeOnReasonCodeRefno FOREIGN KEY (ReasonCodeRefno) REFERENCES tReasonCode(ReasonCodeRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating item reason code xref table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS PK_ActivityItemReasonCodeXRefOnActivityItemRefnoAndReasonCodeRefno ON tActivityItemReasonCodeXRef(ActivityItemRefno ASC, ReasonCodeRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemReasonCodeXRefOnActivityItemRefno ON tActivityItemReasonCodeXRef(ActivityItemRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemReasonCodeXRefOnReasonCodeRefno ON tActivityItemReasonCodeXRef(ReasonCodeRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tActivityItemReasonCodeXRef")]
    public class tActivityItemReasonCodeXRefModel
    {
        [NotNull, ForeignKey(typeof(tActivityItemModel)), Indexed(Name = "FK_ActivityItemReasonCodeXRefOnActivityItemRefno", Order = 1, Unique = false)]
        public int ActivityItemRefno { get; set; }

        [NotNull, ForeignKey(typeof(tReasonCodeModel)), Indexed(Name = "FK_ActivityItemReasonCodeXRefOnReasonCodeRefno", Order = 1, Unique = false)]
        public int ReasonCodeRefno { get; set; }
    }
}
