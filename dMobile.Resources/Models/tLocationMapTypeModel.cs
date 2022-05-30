// <copyright file="tLocationMapTypeModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tLocationMapType"
    //+ "("
    //+ "LocationMapRefno INTEGER NOT NULL,"
    //+ "LocationMapTypeEnum INTEGER NOT NULL,"

    //+ "CONSTRAINT LocationMapTypeToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating location map type table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS PK_LocationMapTypeOnLocationMapRefnoAndLocationMapTypeEnum ON tLocationMapType(LocationMapRefno ASC, LocationMapTypeEnum ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_LocationMapTypeOnLocationMapRefno ON tLocationMapType(LocationMapRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tLocationMapType")]
    public class tLocationMapTypeModel
    {
        [NotNull, ForeignKey(typeof(tLocationMapModel)), Indexed(Name = "FK_LocationMapTypeOnLocationMapRefno", Order = 1, Unique = false)]
        public int LocationMapRefno { get; set; }

        [NotNull, Indexed(Name = "PK_LocationMapTypeOnLocationMapTypeEnum", Order = 1, Unique = false)]
        public int LocationMapTypeEnum { get; set; }
    }
}
