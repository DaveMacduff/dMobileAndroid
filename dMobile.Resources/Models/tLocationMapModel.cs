// <copyright file="tLocationMapModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tLocationMap"
    //   + "("
    //   + "LocationMapRefno INTEGER NOT NULL CONSTRAINT PK_LocationMapOnLocationMapRefno PRIMARY KEY,"
    //   + "LocationMapSubType INTEGER NOT NULL"
    //   + ")";

    //if (this.OnNubStatOut != null)
    //{
    //    
    //    this.MsgEventArgs.StatusOutMsg = "Creating location map table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //        string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapOnLocationMapRefnoAndLocationMapSubType ON tLocationMap(LocationMapRefno ASC, LocationMapSubType ASC)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tLocationMap")]
    public class tLocationMapModel
    {
        [PrimaryKey, NotNull, Indexed(Name = "UQ_LocationMapOnLocationMapRefnoAndLocationMapSubType", Order = 1, Unique = true)]
        public int LocationMapRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_LocationMapOnLocationMapRefnoAndLocationMapSubType", Order = 2, Unique = true)]
        public int LocationMapSubType { get; set; }
    }
}
