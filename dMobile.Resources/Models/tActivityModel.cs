// <copyright file="tActivityModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivity"
    //+ "("
    //+ "ActivityRefno INTEGER NOT NULL CONSTRAINT PK_ActivityOnActivityRefno PRIMARY KEY,"
    //+ "ActivityStatus INTEGER NOT NULL,"
    //+ "ActivitySubType INTEGER NOT NULL"
    //+ ")";

    //        string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityOnActivityRefnoAndActivitySubType "
    //            + "ON tActivity(ActivityRefno ASC, ActivitySubType ASC)";
    //        this.RunDDLCommand(sqlNdx);
    [Table("tActivity")]
    public class tActivityModel
    {
        [PrimaryKey]
        [AutoIncrement]
        [NotNull]
        [Indexed(Name = "UQ_ActivityOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        [NotNull]
        public int ActivityStatus { get; set; }

        [Indexed(Name = "UQ_ActivityOnActivityRefnoAndActivitySubType", Order = 2, Unique = true)]
        [NotNull]
        public int ActivitySubType { get; set; }
    }
}